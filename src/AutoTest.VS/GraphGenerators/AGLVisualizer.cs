using System;
using System.Drawing;
using System.Windows.Forms;
using AutoTest.Client;
using AutoTest.VM.Messages;
using AutoTest.VS.Util;
using EnvDTE80;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using Color = Microsoft.Msagl.Drawing.Color;
using Timer = System.Windows.Forms.Timer;
using AutoTest.VS.Resources;

namespace AutoTest.VS.GraphGenerators
{
    public partial class AGLVisualizer : Form
    {
        private readonly DisplayMode _mode;
        private readonly DTE2 _dte;
        private readonly bool _transparent;
        private readonly ATEClient _client;
        private GViewer _gViewer;
        private Node _selectedNode;
        private Timer t;

        public AGLVisualizer(DisplayMode mode, DTE2 dte, bool transparent, ATEClient client)
        {
            Icon = ResourceReader.GetIcon("MM-16.ico");
            if(transparent)
            {
                BackColor = System.Drawing.Color.FromArgb(1, 2, 3);
                ForeColor = System.Drawing.Color.FromArgb(1, 2, 3);
                TransparencyKey = System.Drawing.Color.FromArgb(1, 2, 9);
                Opacity = 0.8D;
                FormBorderStyle = FormBorderStyle.None;
            }
            _mode = mode;
            _dte = dte;
            _transparent = transparent;
            _client = client;
            InitializeComponent();
            if(_mode == DisplayMode.Light)
            {
                BackColor = System.Drawing.Color.LightGray;
                TransparencyKey = System.Drawing.Color.FromArgb(255, 254, 253);
            }
        }

        public void Display(VisualGraphGeneratedMessage message)
        {
            Controls.Clear();
            if(_gViewer != null) Controls.Remove(_gViewer); 
            _gViewer = new GViewer
                          {
                              Dock = DockStyle.Fill,
                              LayoutAlgorithmSettingsButtonVisible = false,
                              NavigationVisible = false,
                              ToolBarIsVisible = false,
                              SaveButtonVisible = false,
                              SaveGraphButtonVisible = false,
                              AutoSizeMode = AutoSizeMode.GrowOnly,
                              AutoScroll = false,
            };
            _gViewer.HorizontalScroll.Enabled = false;
            _gViewer.VerticalScroll.Enabled = false;
            _gViewer.HorizontalScroll.Visible = false;
            _gViewer.VerticalScroll.Visible = false;
            _gViewer.DoubleClick += AGLVisualizer_DoubleClick;
            if (_mode == DisplayMode.Dark)
            {

                BackColor = System.Drawing.Color.Black;
                _gViewer.OutsideAreaBrush = new SolidBrush(System.Drawing.Color.Black);
            }
            else
            {
                BackColor = System.Drawing.Color.LightGray;
                _gViewer.OutsideAreaBrush = new SolidBrush(System.Drawing.Color.LightGray);
            }
            _gViewer.CurrentLayoutMethod = LayoutMethod.UseSettingsOfTheGraph;
            _gViewer.Graph = BuildGraphFrom(message);
            Controls.Add(_gViewer);
            selectNode(FindSearchedNode(message).Id);
        }


        private void selectNode(string newSelectedNodeID)
        {
            //If there is allready a selected node then "deselect" it
            if (_selectedNode != null)
            {
                _selectedNode.Attr.Color = Color.Black;
            }
            _selectedNode = _gViewer.Graph.FindNode(newSelectedNodeID);
            //save new selected node
            if (_selectedNode != null)
            {
                _selectedNode.Attr.Color = Color.Red;
                //and "select" it
            }
            _gViewer.CenterToPoint(_selectedNode.BoundingBox.Center);
            _gViewer.Invalidate();
        }

        //direction is needed for the direct which should be checked for nodes
        public enum direction
        {
            up = 0,
            right = 1,
            down = 2,
            left = 3
        }

        private void selectNode(direction direction)
        {
            if (this._selectedNode != null)
            {
                Microsoft.Msagl.Point selectedNodePosition = default(Microsoft.Msagl.Point);
                selectedNodePosition = _selectedNode.Attr.Pos;

                Node nearestNode = null;
                double nearestRange = 0;
                double checkingRange = 0;

                //check each node
                foreach (Node checkingNode in _gViewer.Graph.NodeMap.Values)
                {
                    //except the selected one ;)
                    if (!ReferenceEquals(checkingNode, _selectedNode))
                    {
                        switch (direction)
                        {
                            case direction.up:
                            case direction.down:
                                //check for Y-axis
                                //check if the direction is up and the node which should be checked is upper
                                //or the direction is down and the node is lower
                                if ((direction == direction.up && checkingNode.Attr.Pos.Y > selectedNodePosition.Y) || (direction == direction.down && checkingNode.Attr.Pos.Y < selectedNodePosition.Y))
                                {
                                    //if the node is valid check fi there is allready a "nearest node"
                                    if (nearestNode == null)
                                    {
                                        //if not save the node and the range between the selected node and the checked node
                                        nearestNode = checkingNode;
                                        nearestRange = getRange(checkingNode.Attr.Pos, selectedNodePosition);
                                    }
                                    else
                                    {
                                        checkingRange = getRange(checkingNode.Attr.Pos, selectedNodePosition);
                                        //check if the selected node is nearer then the nearest node
                                        if (checkingRange < nearestRange)
                                        {
                                            nearestNode = checkingNode;
                                            nearestRange = checkingRange;
                                        }
                                    }
                                }
                                break;
                            case direction.right:
                            case direction.left:
                                //check X-axis same way
                                if ((direction == direction.right && checkingNode.Attr.Pos.X > selectedNodePosition.X) || (direction == direction.left && checkingNode.Attr.Pos.X < selectedNodePosition.X))
                                {
                                    if (nearestNode == null)
                                    {
                                        nearestNode = checkingNode;
                                        nearestRange = getRange(checkingNode.Attr.Pos, selectedNodePosition);
                                    }
                                    else
                                    {
                                        checkingRange = getRange(checkingNode.Attr.Pos, selectedNodePosition);
                                        if (checkingRange < nearestRange)
                                        {
                                            nearestNode = checkingNode;
                                            nearestRange = checkingRange;
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }

                //if there is a nearest node
                if (nearestNode != null)
                {
                    selectNode(nearestNode.Id);
                    //select it
                }
            }
        }

        private double getRange(Microsoft.Msagl.Point position1, Microsoft.Msagl.Point position2)
        {
            return Math.Max(position1.X, position2.X) - Math.Min(position1.X, position2.X) + Math.Max(position1.Y, position2.Y) - Math.Min(position1.Y, position2.Y);
        }




        private void AGLVisualizer_DoubleClick(object sender, EventArgs e)
        {
            var node = _gViewer.SelectedObject as Node;

            if (node != null)
            {
                MethodFinder_Slow.GotoMethod(node.Id, _dte);
                if (_transparent)
                {
                    ClickClose();
                }
            }
        }

        private void ClickClose()
        {
            t = new Timer(); 
            t.Tick += (a,b) =>
            {
                var x = (Timer) a; 
                x.Dispose(); 
                Close();
            }; 
            t.Interval = 20; 
            t.Start(); 
        } 


        private Node FindSearchedNode(VisualGraphGeneratedMessage graph)
        {
            foreach(var n in graph.Nodes)
            {
                if(n.IsRootNode)
                {
                    return _gViewer.Graph.FindNode(n.FullName);
                }
            }
            return null;
        }


        private Graph BuildGraphFrom(VisualGraphGeneratedMessage affectedGraph)
        {
            var graph = new Graph("graph")
                            {
                                Attr = {BackgroundColor = Color.Transparent}
                            };
            if (affectedGraph.Connections != null)
            {
                _gViewer.LayoutAlgorithmSettingsButtonVisible = true;
                _gViewer.ForeColor = System.Drawing.Color.FromArgb(1,2,3);
                foreach (var x in affectedGraph.Connections)
                {
                    var edge = graph.AddEdge(x.From, x.To);
                    if (_mode == DisplayMode.Dark)
                    {
                        edge.Attr.Color = Color.White;
                    }
                }
            }
            graph.Attr.LayerDirection = LayerDirection.LR;
            if (affectedGraph.Nodes != null)
                foreach (var y in affectedGraph.Nodes)
                {
                    var n = graph.FindNode(y.FullName) ?? graph.AddNode(y.FullName);
                    n.LabelText = y.DisplayName;
                    
                    n.Attr.Shape = Shape.Box;
                    if (y.IsRootNode)
                        n.Attr.FillColor = Color.LightGreen;
                    else if (y.IsChange)
                        n.Attr.FillColor = Color.Maroon;
                    else if (y.IsTest && y.IsProfiledTest)
                        n.Attr.FillColor = Color.Yellow;
                    else if (y.IsTest)
                        n.Attr.FillColor = Color.DarkGoldenrod;
                    else if (y.IsInterface)
                        n.Attr.FillColor = Color.LightBlue;
                        //TODO GREG PUT ON FEATURE TOGGLE
                    else if (false && y.Complexity > 15)
                        n.Attr.FillColor = Color.LightPink;
                    else
                        n.Attr.FillColor = Color.White;
                }
            return graph;
        }

        private void AGLVisualizer_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
           
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (_gViewer == null) return false;
            if (keyData == (Keys.Up | Keys.Control))
            {
                _gViewer.ZoomOutPressed();
            }
            if (keyData == (Keys.Down | Keys.Control))
            {
                _gViewer.ZoomInPressed();
            }
            if (keyData == Keys.P)
            {
                if (_selectedNode != null)
                    _client.GetProfiledGraphFor(_selectedNode.Id);
            }
            if ((keyData & Keys.S) == Keys.S)
            {
                //if (_selectedNode != null)
                //    _client.GetProfiledGraphFor(_selectedNode.Id);
                //_gViewer.SaveButtonPressed();
            }
            if(( keyData & Keys.G) == Keys.G)
            {
                if (_selectedNode != null)
                    _client.GetGraphFor(_selectedNode.Id);
            }
            
            if (keyData == Keys.PageDown || keyData == Keys.Oemplus)
            {
                _gViewer.ZoomInPressed();
            }
            if (keyData == Keys.PageUp || keyData == Keys.OemMinus)
            {
                _gViewer.ZoomOutPressed();
            }
            if(keyData == (Keys.Down | Keys.Alt))
            {
                _gViewer.Pan(0, _gViewer.GraphHeight * -.05);
            }
            if (keyData == (Keys.Up | Keys.Alt))
            {
                _gViewer.Pan(0, _gViewer.GraphHeight * .05);
            }
            if (keyData == (Keys.Left | Keys.Alt))
            {
                _gViewer.Pan(_gViewer.GraphHeight * -.1, 0);
            }
            if (keyData == (Keys.Right | Keys.Alt))
            {
                _gViewer.Pan(_gViewer.GraphHeight * .1, 0);
            }
            if (keyData == Keys.H || keyData == Keys.Left)
            {
                selectNode(direction.left);
            }
            if (keyData == Keys.L || keyData == Keys.Right)
            {
                selectNode(direction.right);
            }
            if (keyData == Keys.K || keyData == Keys.Up)
            {
                selectNode(direction.up);
            }
            if (keyData == Keys.J || keyData == Keys.Down)
            {
                selectNode(direction.down);
            }
            if (keyData == Keys.Escape)
            {
                Close();
            }
            if (keyData == Keys.Enter || keyData == Keys.Return || keyData == Keys.Space)
            {
                if(_selectedNode != null)
                   MethodFinder_Slow.GotoMethod(_selectedNode.Id, _dte);
                Close();
            }
            return true;
        }

        private void AGLVisualizer_KeyDown(object sender, KeyEventArgs e)
        {
                        
        }
    }

    public enum DisplayMode
    {
        Light,
        Dark
    }
}
