using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using EnvDTE80;
using Northwoods.Go;

namespace AutoTest.VS.SequenceDiagramGenerators
{
    [Serializable]
    public class Lifeline : GoNode
    {

        public event EventHandler<LifeLineClickedArgs> Clicked;
        public string TypeToGoto;

        public void InvokeClicked(LifeLineClickedArgs e)
        {
            EventHandler<LifeLineClickedArgs> handler = Clicked;
            if (handler != null) handler(this, e);
        }

        public Lifeline(Color color, Color textColor)
        {
            Resizable = false;

            header = new GoTextNode();
            header.Selectable = false;
            header.TopPort = null; // don't need these ports
            header.LeftPort = null;
            header.RightPort = null;
            header.BottomPort = null;
            header.Text = "name : class";
            header.Label.TextColor = textColor;
            header.Label.Alignment = MiddleTop;
            header.Label.Wrapping = true;
            header.Label.WrappingWidth = 100;
            header.Label.Editable = false;
            header.Label.BackgroundColor = color;
            header.Shape.BrushColor = color;
            header.Shape.Pen = new Pen(textColor, 2);

            header.Position = new PointF(10, 10);
            Add(header); // will be this[0]

            GoStroke line = new GoStroke();
            line.Selectable = false;
            Pen pen = new Pen(textColor, 2);
            pen.DashStyle = DashStyle.Dash;
            line.Pen = pen;
            line.AddPoint(new PointF());
            line.AddPoint(new PointF());
            Add(line); // will be this[1]

            LifelinePort port = new LifelinePort();
            Add(port); // will be this[2]
        }




        // convenience constructor
        public Lifeline(String head, Color color, Color textColor)
            : this(color, textColor)
        {
            this.Text = head;
        }

        public override bool OnDoubleClick(GoInputEventArgs evt, GoView view)
        {
            InvokeClicked(new LifeLineClickedArgs());
            return base.OnDoubleClick(evt, view);
        }

        public GoPort Port
        {
            get { return this[2] as GoPort; }
        }

        public string GoToSpot { get; set; }

        // Assume the header is at the top, and the lifeline stroke and port extend down
        // from the middle of the bottom of the header.
        // The Lifeline's Activations are arranged vertically according to their Begin
        // and End step values.
        // The stroke and port are just tall enough to accomodate all Activations and all
        // Messages that connect to this Lifeline.
        public override void LayoutChildren(GoObject childchanged)
        {
            if (this.Initializing) return;
            if (this.Count < 3) return;
            GoObject header = this[0];
            GoStroke line = this[1] as GoStroke;
            GoObject port = this[2];
            PointF p = header.GetSpotLocation(MiddleBottom);

            // find last step of an Activation on this Lifeline or a Message connected to this Lifeline
            float maxend = 0;
            foreach (GoObject child in this)
            {
                Activation act = child as Activation;
                if (act != null)
                {
                    act.SetSpotLocation(MiddleTop, GetStepPoint(act.Begin));
                    act.Height = Math.Max(10, (act.End - act.Begin)*MessageSpacing);
                    maxend = Math.Max(maxend, act.End);
                }
            }
            foreach (IGoLink link in this.Port.Links)
            {
                Message msg = link as Message;
                if (msg != null)
                {
                    maxend = Math.Max(maxend, msg.Step);
                }
            }

            // line connects to bottom of header
            line.SetPoint(0, p);
            line.SetPoint(1, new PointF(p.X, LineStart + maxend*MessageSpacing));
            // port always starts at LineStart
            port.Bounds = new RectangleF(p.X - port.Width/2, LineStart, port.Width, maxend*MessageSpacing);
        }

        public override void OnGotSelection(GoSelection sel)
        {
            oldBrushColor = header.Shape.BrushColor;
            header.Shape.BrushColor = Color.LightGreen;
            base.OnGotSelection(sel);
        }

        public override void OnLostSelection(GoSelection sel)
        {
            header.Shape.BrushColor = oldBrushColor;
            base.OnGotSelection(sel);
        }

        // only permit moving to the left and to the right
        public override PointF ComputeMove(PointF origLoc, PointF newLoc)
        {
            return new PointF(newLoc.X, origLoc.Y);
        }

        // given a step number, return the point on the lifeline
        public PointF GetStepPoint(float step)
        {
            return new PointF(this[0].Center.X, LineStart + step*MessageSpacing);
        }

        // given a vertical point on the lifeline, what step is it?
        public float GetStep(float y)
        {
            return Math.Max(0, y - LineStart)/MessageSpacing;
        }

        // see if there's an Activation at a particular (vertical) step
        public Activation FindActivation(float step)
        {
            foreach (GoObject obj in this)
            {
                Activation act = obj as Activation;
                if (act != null && step >= act.Begin && step <= act.End) return act;
            }
            return null;
        }

        // some parameters
        public static float LineStart = 70; // vertical starting point in document for all Messages and Activations
        public static float MessageSpacing = 15; // vertical distance between Messages at different steps
        private GoTextNode header;
        private Color oldBrushColor;
    }

    public class LifeLineClickedArgs : EventArgs
    {
    }


    // The vertical position for the link points at a LifelinePort are determined
    // by the Message.Step property by the Lifeline.GetStepPoint method.
    // The horizontal position depends on whether there is an Activation at that point.
    // The direction for the link is always either 0 or 180, depending on where
    // the other Lifeline is relative to this port's Lifeline.
    [Serializable]
    public class LifelinePort : GoPort
    {
        public LifelinePort()
        {
            this.Style = GoPortStyle.None; // no visual representation
        }

        public override PointF GetFromLinkPoint(IGoLink link)
        {
            Message m = link as Message;
            Lifeline line = this.Parent as Lifeline;
            if (m != null && m.ToPort != null)
            {
                PointF p = line.GetStepPoint(m.Step);
                Activation act = line.FindActivation(m.Step);
                if (act != null)
                {
                    p.X += ((m.ToPort.GoObject.Center.X > p.X) ? act.Width/2 : -act.Width/2);
                }
                return p;
            }
            else
            {
                return base.GetFromLinkPoint(link);
            }
        }

        public override float GetFromLinkDir(IGoLink link)
        {
            if (link.ToPort == null || link.ToPort.GoObject.Center.X > this.Center.X)
                return 0;
            else
                return 180;
        }

        public override PointF GetToLinkPoint(IGoLink link)
        {
            Message m = link as Message;
            Lifeline line = this.Parent as Lifeline;
            if (m != null && m.FromPort != null)
            {
                PointF p = line.GetStepPoint(m.Step);
                Activation act = line.FindActivation(m.Step);
                if (act != null)
                {
                    p.X += ((m.FromPort.GoObject.Center.X > p.X) ? act.Width/2 : -act.Width/2);
                }
                return p;
            }
            else
            {
                return base.GetToLinkPoint(link);
            }
        }

        public override float GetToLinkDir(IGoLink link)
        {
            if (link.FromPort == null || link.FromPort.GoObject.Center.X > this.Center.X)
                return 0;
            else
                return 180;
        }
    }


    // this is a rectangle representing the duration of some task on a Lifeline
    [Serializable]
    public class Activation : GoRectangle
    {
        public Activation()
        {
            this.BrushColor = Color.White;
            this.PenColor = Color.Black;
            this.Size = new SizeF(10, Lifeline.MessageSpacing);
        }

        // only allow resizing vertically
        public override void AddSelectionHandles(GoSelection sel, GoObject selectedObj)
        {
            Lifeline line = this.Parent as Lifeline;
            if (line != null)
            {
                RemoveSelectionHandles(sel);
                sel.CreateResizeHandle(this, selectedObj, line.GetStepPoint(this.Begin), MiddleTop, true);
                sel.CreateResizeHandle(this, selectedObj, line.GetStepPoint(this.End), MiddleBottom, true);
            }
            else
            {
                base.AddSelectionHandles(sel, selectedObj);
            }
        }

        // resizing updates the values for Begin and End
        public override void DoResize(GoView view, RectangleF origRect, PointF newPoint, int whichHandle,
                                      GoInputState evttype, SizeF min, SizeF max)
        {
            var line = Parent as Lifeline;
            if (line != null)
            {
                RectangleF box = ComputeResize(origRect, newPoint, whichHandle, min, max, true);
                Rectangle viewbox = view.ConvertDocToView(box);
                view.DrawXorBox(viewbox, evttype != GoInputState.Finish);
                if (evttype == GoInputState.Finish)
                {
                    Begin = line.GetStep(box.Top);
                    End = line.GetStep(box.Bottom);
                    line.LayoutChildren(this);
                    line.Port.LinksOnPortChanged(ChangedBounds, 0, null, NullRect, 0, null, NullRect);
                }
            }
            else
            {
                base.DoResize(view, origRect, newPoint, whichHandle, evttype, min, max);
            }
        }

        public float Begin
        {
            // the starting Step
            get { return myBegin; }
            set
            {
                float old = myBegin;
                if (old != value)
                {
                    myBegin = value;
                    Changed(ChangedBegin, 0, null, MakeRect(old), 0, null, MakeRect(value));
                }
            }
        }

        public float End
        {
            // the ending Step
            get { return myEnd; }
            set
            {
                float old = myEnd;
                if (old != value)
                {
                    myEnd = value;
                    Changed(ChangedEnd, 0, null, MakeRect(old), 0, null, MakeRect(value));
                }
            }
        }

        // undo/redo support
        public override void ChangeValue(GoChangedEventArgs e, bool undo)
        {
            if (e.SubHint == ChangedBegin)
                this.Begin = e.GetFloat(undo);
            else if (e.SubHint == ChangedEnd)
                this.End = e.GetFloat(undo);
            else
                base.ChangeValue(e, undo);
        }

        public const int ChangedBegin = LastChangedHint + 1;
        public const int ChangedEnd = LastChangedHint + 2;
        private float myBegin;
        private float myEnd;
    }


    // a Message is a horizontal link with a label, at a particular vertical position (the Step)
    // The link will be laid out so that its points have a Y coordinate determined by Lifeline.GetStepPoint
    [Serializable]
    public class Message : GoLabeledLink
    {
        private readonly Color _color;
        private readonly DTE2 _dte;
        public event EventHandler<MessageClickedArgs> Clicked;


        public override string GetToolTip(GoView view)
        {
            return _msg;
        }

        public void TruncateLabel()
        {
            GoText lab = this.FromLabel as GoText;
            GoPort fp = this.FromPort as GoPort;
            GoPort tp = this.ToPort as GoPort;
            if (lab == null || fp == null || tp == null) return;
            string s = _msg;
            float w = Math.Abs(tp.Center.X - fp.Center.X) - 10;

            while (s.Length > 0)
            {
                lab.Text = s;
                if (lab.Width < w) break;
                s = s.Substring(1);
            }
        }


        public void InvokeClicked(MessageClickedArgs e)
        {
            EventHandler<MessageClickedArgs> handler = Clicked;
            if (handler != null) handler(this, e);
        }

        public Message(Color color)
        {
            _color = color;
            var lab = new GoText();
            lab.Selectable = false;
            lab.Text = "msg";
            lab.Editable = true;
            FromLabel = lab;
            ToArrow = true;
            ToArrowShaftLength = 0;
        }

        // convenience constructor
        public Message(float step, Lifeline from, Lifeline to, string msg, float actlen, DTE2 dte, Color color,
                       Color oppositeColor)
            : this(color)
        {
            _dte = dte;
            Step = step;
            var lab = FromLabel as GoText;
            Pen = new Pen(_color);
            _msg = msg;
            if (lab != null) lab.Text = msg;
            lab.TextColor = oppositeColor;
            if (from != null) FromPort = from.Port;
            if (to != null) ToPort = to.Port;
            if (actlen > 0 && to != null)
            {
                var act = new Activation {Begin = step, End = step + actlen};
                to.Add(act);
            }
        }


        public override void LayoutChildren(GoObject childchanged)
        {
            var lab = this.FromLabel as GoText;
            if (childchanged != lab && lab != null)
            {
                var fp = this.FromPort as GoPort;
                var tp = this.ToPort as GoPort;
                if (tp != null && fp != null)
                {
                    //lab.Width = Math.Abs(tp.Center.X - fp.Center.X) - 10;
                    TruncateLabel();
                }
            }
            base.LayoutChildren(childchanged);
        }

        public float Step
        {
            // the relative position of this link, vertically
            get { return myStep; }
            set
            {
                float old = myStep;
                if (old != value)
                {
                    myStep = value;
                    Changed(ChangedStep, 0, null, MakeRect(old), 0, null, MakeRect(value));
                }
            }
        }

        // undo/redo support
        public override void ChangeValue(GoChangedEventArgs e, bool undo)
        {
            if (e.SubHint == ChangedStep)
                this.Step = e.GetFloat(undo);
            else
                base.ChangeValue(e, undo);
        }

        public const int ChangedStep = LastChangedHint + 1;
        private float myStep;
        public string GoToSpot;
        private string _msg;

        public override bool OnDoubleClick(GoInputEventArgs evt, GoView view)
        {
            InvokeClicked(new MessageClickedArgs());
            return false;
        }
    }

    public class MessageClickedArgs : EventArgs
    {
    }
}
