using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using AutoTest.VM.Messages;
using AutoTest.VS.Util;
using EnvDTE80;
using Northwoods.Go;

namespace AutoTest.VS.SequenceDiagramGenerators
{
    public partial class GoDiagramPlayWindow : Form
    {
        private readonly DTE2 _dte;
        private GoView goView1;
        private Label label1;
        private GoDocument doc;
        private Dictionary<string, Lifeline> lifelines;

        public GoDiagramPlayWindow(DTE2 dte)
        {
            _dte = dte;
            lifelines = new Dictionary<string, Lifeline>();
            InitializeComponent();
            goView1.DefaultTool = new GoToolManager(goView1);
        }

        public void CreateDiagramFor(TestInformationGeneratedMessage message)
        {
            doc = goView1.Document;
            doc.AllowLink = false;
            doc.AllowEdit = false;
            doc.AllowResize = false;

            RecurseElements(message.Test, null, false, false, false);
            label1.Text = "Sequence diagram for " + message.Item;

            doc.Bounds = doc.ComputeBounds();
            goView1.DocPosition = doc.TopLeft;

            goView1.GridUnboundedSpots = GoObject.BottomLeft | GoObject.BottomRight;
            goView1.Grid.Top = Lifeline.LineStart;
            goView1.GridOriginY = Lifeline.LineStart;
            goView1.GridCellSizeHeight = Lifeline.MessageSpacing;

            // support undo/redo
            doc.UndoManager = new GoUndoManager();
        }


        private Lifeline last;
        private Lifeline GetLifeLine(string name)
        {
            var typename = ParseType(name);
            if (!lifelines.ContainsKey(typename))
            {
                
                var lifeline = new Lifeline(typename);
               
                doc.Add(lifeline);
                lifeline.Left = last == null ? 30 : last.Right + 30;
                last = lifeline;
                lifelines.Add(typename, lifeline);
                return lifeline;
            }
            return lifelines[typename];
        }

        private string ParseType(string item)
        {
            int start = item.IndexOf(' ');
            var end = item.IndexOf("::");
            if (start == -1 || end == -1 || end < start) return "PARSE ERROR";
            var fulltype = item.Substring(start, end - start);
            
            var lastdot = fulltype.LastIndexOf(".") + 1;
            if (lastdot == -1) return fulltype;
            return fulltype.Substring(lastdot, fulltype.Length - lastdot);
        }

        private string GetName(string item)
        {
            int start = item.IndexOf("::") + 2;
            var end = item.IndexOf("(");
            if (start == -1 || end == -1 || end < start) return "PARSE ERROR";
            return item.Substring(start, end - start);
        }

        private int seq = 0;
        private Pen GetPenColor(bool inSetup, bool inTest, bool inTeardown)
        {
            return new Pen(Color.White);
            if (inSetup)
            {
                return new Pen(Color.Cyan);
            }
            if (inTest)
            {
                return new Pen(Color.Yellow);
            }
            if (inTeardown)
            {
                return new Pen(Color.LightPink);
            }
            return new Pen(Color.White);
        }

        private Lifeline RecurseElements(Chain item, Lifeline parent, bool inSetup, bool inTest, bool inTeardown)
        {
            var current = GetLifeLine(item.DisplayName);
            if (item.IsTest) inTest = true;
            if (item.IsSetup) inSetup = true;
            if (item.IsTeardown) inTeardown = true;
            if (parent != null)
            {
                var message = new Message(seq, parent, current, GetName(item.DisplayName), 1, _dte);
                message.Pen = GetPenColor(inSetup, inTest, inTeardown);
                message.Clicked += message_Clicked;
                message.GoToSpot = item.Name;
                doc.Add(message);
                seq++;
            }

            foreach(var child in item.Children)
            {
                var childline = RecurseElements(child, current, inSetup, inTest, inTeardown);
                if(current != null && childline != current)
                {
                    var message = new Message(seq, childline, current, "", 1, _dte);
                    message.Pen = GetPenColor(inSetup, inTest, inTeardown);
                    message.GoToSpot = item.Name;
                    doc.Add(message);
                    seq++;
                }
            }
            return current;
        }

        private void message_Clicked(object sender, MessageClickedArgs e)
        {
            MethodFinder_Slow.GotoMethodByFullname(((Message)sender).GoToSpot, _dte);
            this.Hide();
            BeginInvoke(new MethodInvoker(() => CloseForm()));
        }

        private IAsyncResult CloseForm()
        {
            this.Close();
            return null;
        }

        private void GoDiagramPlayWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Escape)
            {
                this.Hide();
                BeginInvoke(new MethodInvoker(() => CloseForm()));
            }
        }
    }
}
