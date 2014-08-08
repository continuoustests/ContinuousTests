using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using AutoTest.VM.Messages;
using AutoTest.VS.Util;
using EnvDTE80;
using Northwoods.Go;
using AutoTest.VS.Resources;
using AutoTest.Client;
namespace AutoTest.VS.SequenceDiagramGenerators
{
    public partial class SequenceDiagram : Form
    {
        private readonly DisplayMode _mode;
        private readonly DTE2 _dte;
        private readonly bool _transparent;
        private GoView _view;
        private Label _label1;
        private GoDocument _doc;
        private readonly Dictionary<string, Lifeline> _lifelines;

        public SequenceDiagram(DisplayMode mode, DTE2 dte, bool transparent)
        {
            _mode = mode;
            _dte = dte;
            _transparent = transparent;
            _lifelines = new Dictionary<string, Lifeline>();
            InitializeComponent();

            Icon = ResourceReader.GetIcon("MM-16.ico");
            InitForm(mode);
            _view.DefaultTool = new GoToolManager(_view);
        }

        private void InitForm(DisplayMode mode)
        {
            if (_transparent)
            {
                _view.Border3DStyle = Border3DStyle.Flat;
                _view.BorderStyle = BorderStyle.None;
                _view.GridLineColor = Color.Black;
                FormBorderStyle = FormBorderStyle.None;
                ShowInTaskbar = false;
                Opacity = 0.8D;
                TransparencyKey = Color.Transparent;
                _view.PrimarySelectionColor = Color.Transparent;
                _view.SecondarySelectionColor = Color.Transparent;
            }
            if (mode == DisplayMode.Dark)
            {
                BackColor = Color.FromArgb(1, 2, 3);
                ForeColor = Color.FromArgb(1, 2, 3);
                _view.BackColor = Color.Black;
            }
            if(mode ==DisplayMode.Light)
            {
                BackColor = Color.LightGray;
                TransparencyKey = Color.FromArgb(255, 254, 253);
                _view.BackColor = Color.LightGray;
            }

        }

        public void CreateDiagramFor(TestInformationGeneratedMessage message)
        {
            seq = 0;
            _lifelines.Clear();
            _doc = _view.Document;
            _doc.Clear();
            _doc.AllowLink = false;
            _doc.AllowEdit = false;
            _doc.AllowResize = false;

            RecurseElements(message.Test, null, false, false, false);
            _label1.Text = "Sequence diagram for " + message.Item;

            _doc.Bounds = _doc.ComputeBounds();
            _view.DocPosition = _doc.TopLeft;

            _view.GridUnboundedSpots = GoObject.BottomLeft | GoObject.BottomRight;
            _view.Grid.Top = Lifeline.LineStart;
            _view.GridOriginY = Lifeline.LineStart;
            _view.GridCellSizeHeight = Lifeline.MessageSpacing;

            // support undo/redo
            _doc.UndoManager = new GoUndoManager();
        }


        private Lifeline last;
        private Lifeline GetLifeLine(string name)
        {
            var typename = ParseType(name);
            if (!_lifelines.ContainsKey(typename))
            {
                
                var lifeline = new Lifeline(typename, GetColor(0), GetOppositeColor(0));
                lifeline.Selectable = false;
                lifeline.GoToSpot = ParseFullType(name);
                lifeline.Clicked += lifeline_Clicked;
                _doc.Add(lifeline);
                lifeline.Left = last == null ? 30 : last.Right + 30;
                last = lifeline;
                _lifelines.Add(typename, lifeline);
                return lifeline;
            }
            return _lifelines[typename];
        }

        void lifeline_Clicked(object sender, LifeLineClickedArgs e)
        {
            MethodFinder_Slow.GotoTypeByFullName(_dte, ((Lifeline)sender).GoToSpot);
            Hide();
            BeginInvoke(new MethodInvoker(() => CloseForm()));
        }

        private Color GetOppositeColor(double time)
        {
            //TODO GREG PUT ON FEATURE TOGGLE
            if (false && time > .5) return Color.Yellow;
            if (_mode == DisplayMode.Light) return Color.Black;
            return Color.White;
        }

        private Color GetColor(double time)
        {
            //TODO GREG PUT ON FEATURE TOGGLE
            if (time > .5) return Color.Red;
            if (_mode == DisplayMode.Light) return Color.White;
            return Color.Black;
        }

        private string ParseType(string item)
        {
            var fulltype = ParseFullType(item);
            
            var lastdot = fulltype.LastIndexOf(".") + 1;
            if (lastdot == -1) return fulltype;
            return fulltype.Substring(lastdot, fulltype.Length - lastdot);
        }

        private string ParseFullType(string item)
        {
            int start = item.IndexOf(' ');
            var end = item.IndexOf("::");
            if (start == -1 || end == -1 || end < start) return "PARSE ERROR";
            return item.Substring(start, end - start);
        }

        private string GetName(string item)
        {
            int start = item.IndexOf("::") + 2;
            var end = item.IndexOf("(");
            if (end == -1) end = item.Length;
            if (start == -1 || end == -1 || end < start) return "PARSE ERROR";
            return item.Substring(start, end - start);
        }

        private int seq = 0;
        private Pen GetPenColor(bool inSetup, bool inTest, bool inTeardown, double time)
        {
            //TODO GREG PUT ON FEATURE TOGGLE
            if (false && time > .8) return new Pen(Color.Red) { Width = 2 };
            if(_mode == DisplayMode.Light)
            {
                return new Pen(Color.Black);
            }
            return new Pen(Color.White);
        }

        private double GetChildrenTimes(Chain item)
        {
            double ret = 0;
            if (item.Children == null || item.Children.Count == 0) return 0;
            foreach (var child in item.Children)
            {
                ret += child.TimeEnd - child.TimeStart;
            }
            return ret;
        }

        private Lifeline RecurseElements(Chain item, Lifeline parent, bool inSetup, bool inTest, bool inTeardown)
        {

            var time = (item.TimeEnd - item.TimeStart);
            var intime = time - GetChildrenTimes(item);
            var current = GetLifeLine(item.DisplayName);
                  
            if (item.IsTest) inTest = true;
            if (item.IsSetup) inSetup = true;
            if (item.IsTeardown) inTeardown = true;
            if (!(item.DisplayName.Contains("::get_") || item.DisplayName.Contains("::set_")))
            {

                if (parent != null)
                {
                    var message = new Message(seq, parent, current, GetName(item.DisplayName) + " " + TimeFormatter.FormatTime(intime), 2, _dte, GetColor(time), GetOppositeColor(intime));
                    message.Pen = GetPenColor(inSetup, inTest, inTeardown, time);
                    message.Clicked += message_Clicked;
                    message.GoToSpot = item.Name;
                    _doc.Add(message);
                    seq++;
                }
            }
            foreach(var child in item.Children)
            {
                var childline = RecurseElements(child, current, inSetup, inTest, inTeardown);
                if (current != null && childline != current)
                {
                    if (!(item.DisplayName.Contains("::get_") || item.DisplayName.Contains("::set_")))
                    {

                        var childtime = child.TimeEnd - child.TimeStart;
                        var message = new Message(seq, childline, current, "", 1, _dte, GetColor(time), GetOppositeColor(time))
                                          {
                                              Pen = GetPenColor(inSetup, inTest, inTeardown, childtime),
                                              GoToSpot = item.Name
                                          };
                        _doc.Add(message);
                        seq++;
                    }
                }
            }
            return current;
        }

        private void message_Clicked(object sender, MessageClickedArgs e)
        {
            MethodFinder_Slow.GotoMethodByFullname(((Message)sender).GoToSpot, _dte);
            if (_transparent)
            {
                Hide();
                BeginInvoke(new MethodInvoker(() => CloseForm()));
            }
        }

        private IAsyncResult CloseForm()
        {
            Close();
            return null;
        }

        private void GoDiagramPlayWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Escape)
            {
                Hide();
                BeginInvoke(new MethodInvoker(() => CloseForm()));
            }
        }
    }

    public enum DisplayMode
    {
        Light,
        Dark
    }
}
