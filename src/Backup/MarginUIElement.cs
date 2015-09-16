using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using AutoTest.Client.Logging;
using EnvDTE;
using Microsoft.VisualStudio.Text.Formatting;
using AutoTest.Client;
using System.Threading;

namespace AutoTest.VS.RiskClassifier
{
    public class MarginUIElement : Grid
    {
        private readonly RiskTag riskTag;
        private readonly IWpfTextViewLine _line;
        private readonly RiskEntry entry;
        private bool visible = true;
         
        static MarginUIElement()
        {
            try
            {
                Application.ResourceAssembly = Assembly.GetExecutingAssembly();
            }
            catch(Exception ex)
            {
                
            }
            ToolTipService.ShowOnDisabledProperty
                 .OverrideMetadata(typeof(UIElement),
                new FrameworkPropertyMetadata(true));             
        }

        public MarginUIElement(RiskTag riskTag, IWpfTextViewLine line)
        {
            this.riskTag = riskTag;
            _line = line;
            entry = EntryCache.GetRiskEntryFor(riskTag.Signature.StringSignature);
            entry.Changed += Refresh;
            entry.Invalidated += CheckUpdate;
            IsVisibleChanged += MarginUIElement_IsVisibleChanged;
            SetUIElements();
        }

        void MarginUIElement_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if (!(bool) e.NewValue)
                {
                    visible = false;
                }
            }
            catch(Exception ex)
            {
                Logger.Write("MarginUIElementVisibleChanged " + ex);
            }
        }

        private void CheckUpdate(object sender, RiskEntryInvalidatedArgs e)
        {
            try
            {
                if (!_line.IsValid)
                {
                    //TODO GFY RESEARCH THIS CODE IS IT NEEDED?
                    //entry.Invalidated -= CheckUpdate;
                    //entry.Changed -= Refresh;
                }

                if (visible)
                {
                    Logger.Write("Updating margin for " + riskTag.Signature);
                    entry.Update();
                } else
                {
                    Logger.Write("margin not visible for " + riskTag.Signature);
                }
            }catch(Exception q)
            {
                
            }
        }

        private void Refresh(object sender, RiskEntryChangedArgs e)
        {
            Application.Current.Dispatcher.Invoke(
                DispatcherPriority.Send,
                (DispatcherOperationCallback)(arg =>
                                                  {
                                                      SetUIElements();
                                                      InvalidateVisual();
                                                      return null;
                                                  }), null);
        }

        public void SetUIElements()
        {
            Children.Clear();
            var status = CurrentTestStatuses.GetStatusOf(entry.Signature);
            if(status != null && status.status == TestStatus.Fail)
            {
                DrawFailure(status.Name, status.text);
            } else if(status != null && status.status == TestStatus.Ignored)
            {
                DrawIgnored(status.Name, status.text);
            } else if(!entry.IsFilled)
            {
                DrawWaiting();
            } else
            {
                if (entry.IsTest)
                {
                    DrawTest();
                }
                else if(!entry.Exists)
                {
                    DrawMissing();
                }
                else
                {
                    DrawNormalMethod();
                }
            }
            InvalidateVisual();
        }

        string previous = "";

        private void DrawIgnored(string name, string text)
        {
            var icon = IconCache.GetImage("ignored.ico");
            if (icon == null) return;
            
            icon.ToolTip = GetStandardToolTip(name + " Ignored\n" + text);

            Children.Add(icon);
        }


        private void DrawFailure(string name,string text)
        {
            var icon = IconCache.GetImage("redx.ico");
            if (icon == null) return;

            icon.ToolTip = GetStandardToolTip(name + " Failed" + text); ;

            Children.Add(icon);
        }

        private void DrawWaiting()
        {
            //var ellipse = new Ellipse
            //{
            //    Height = Height - 1,
            //    Width = Width - 1,
            //    HorizontalAlignment = HorizontalAlignment.Center,
            //    VerticalAlignment = VerticalAlignment.Center,
            //    Fill = Brushes.Blue
            //};
            //Children.Add(ellipse);
        }

        void ShowAndCloseTip(string text) {
            var tip = BuildTestToolTip(text);
            tip.PlacementTarget = this;
            ToolTipService.SetShowDuration(tip, 3000);
            this.ToolTip = tip;
            tip.IsOpen = true;

            ThreadPool.QueueUserWorkItem(j =>
            {
                System.Threading.Thread.Sleep(3000);
                Application.Current.Dispatcher.BeginInvoke(new Action(() => tip.IsOpen = false), null);
            });
        }

        public void DrawTest()
        {
            var fast = double.IsNaN(entry.AverageTimeUnder) || entry.TimesCalled ==0 || entry.AverageTimeUnder < .800;
            //TODO GREG PUT ON FEATURE TOGGLE
            fast = true;
            var icon = fast ? IconCache.GetImage("check.ico")
                                                     : IconCache.GetImage("gary.ico");
            if (icon == null) icon = IconCache.GetImage("defaulttest.ico");
            if (icon == null) return;
            Children.Add(icon);
            if (previous == "FAST" && !fast)
            {
                ShowAndCloseTip("Sigh. It used to be fast.\nLook at sequence diagram to see issue."); 
            }
            if (previous == "SLOW" && fast)
            {
                ShowAndCloseTip("Looks better now.");
            }
            if (previous == "MISSING" && !fast)
            {
                ShowAndCloseTip("Meow, let's try not using nHibernate in the test this time");
            }
            previous = double.IsNaN(entry.AverageTimeUnder) || entry.TimesCalled ==0 || entry.AverageTimeUnder < .800 ? "FAST"
                                                     : "SLOW";
            this.ToolTip = BuildTestToolTip("") ;
        }

        private void DrawMissing()
        {
            previous = "Missing";
            Children.Add(IconCache.GetImage("question.ico"));
            this.ToolTip = GetStandardToolTip("Likely this item has not yet been saved.");
        }

        public void DrawNormalMethod()
        {
            if (this.riskTag.Signature.Element.Kind == vsCMElement.vsCMElementVariable) return;
            if (entry.RiskMetric < 20 && entry.NumberOfAssociatedTests < 3 && entry.NodeCount > 50)
            {
                previous = "DRAGON";
                Children.Add(IconCache.GetImage("dragon.ico"));
                this.ToolTip = GetStandardToolTip("Beware all ye that enter these realms.\n\nIn the old days they drew dragons on the map as warnings\nThis is yours.");
            }
            else
            {

                var brush = new RadialGradientBrush();
                brush.GradientOrigin = new Point(0.25, 0.15);
                var stops = new GradientStopCollection();

                var ellipse = new Ellipse
                                  {
                                      Height = Height + 1,
                                      Width = Width + 1,
                                      HorizontalAlignment = HorizontalAlignment.Center,
                                      VerticalAlignment = VerticalAlignment.Center
                                  };
                if (entry.RiskMetric >= 70)
                {
                    stops.Add(new GradientStop(Colors.LimeGreen, 0.2));
                    stops.Add(new GradientStop(Colors.Green, 0.9));
                    brush.GradientStops = stops;
                    ellipse.Fill = brush;
                    ellipse.Stroke = new SolidColorBrush(Colors.Green);
                }
                else if (entry.RiskMetric > 30 && entry.RiskMetric < 70)
                {
                    stops.Add(new GradientStop(Colors.LightGoldenrodYellow, 0.2));
                    stops.Add(new GradientStop(Colors.Yellow, 0.9));
                    brush.GradientStops = stops;
                    ellipse.Fill = brush;
                    ellipse.Stroke = new SolidColorBrush(Colors.Yellow);
                }
                else
                {
                    stops.Add(new GradientStop(Colors.DarkOrange, 0.2));
                    stops.Add(new GradientStop(Colors.Red, 0.9));
                    brush.GradientStops = stops;
                    ellipse.Fill = brush;
                    ellipse.Stroke = new SolidColorBrush(Colors.Red);
                }
                Children.Add(ellipse);
                var testOutput = entry.NumberOfAssociatedTests > 99 ? "+" : entry.NumberOfAssociatedTests.ToString();
                var text = new TextBlock
                               {
                                   HorizontalAlignment = HorizontalAlignment.Center,
                                   VerticalAlignment = VerticalAlignment.Center,
                                   Text = testOutput,
                                   Foreground = Brushes.White,
                                   FontFamily = new FontFamily("Tahoma"),
                                   FontSize = 10,
                                   FontWeight = FontWeights.Bold
                               };
                if (entry.RiskMetric > 30 && entry.RiskMetric < 70) text.Foreground = Brushes.Black;
                if (previous == "DRAGON")
                {
                    ShowAndCloseTip("You have vanquished the dragon.\n\nMay all bow before you.");
                }
                Children.Add(text);
                //TextBlock textBlock = new TextBlock();
                //textBlock.Foreground = Brushes.White;
                //textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                //textBlock.VerticalAlignment = VerticalAlignment.Center;
                //textBlock.Text = ;

                //Grid toolTipPanel = new Grid();
                //toolTipPanel.Children.Add(textBlock);
                
                //this.ToolTip = toolTipPanel;
                //ToolTipService.SetToolTip(this, toolTipPanel);
                
                this.ToolTip = BuildRegToolTip();
            }
        }

        private ToolTip BuildRegToolTip()
        {
            var t = new ToolTip
            {

                Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom
            };
            var s = "Called " + entry.TimesCalled + " times in Test Suite";
            if (entry.TimesCalled > 0)
            {
                s += "\nAverage Time:\t" + TimeFormatter.FormatTime(entry.AverageTime) + "\tAverage Time Below:\t" + TimeFormatter.FormatTime(entry.AverageTimeUnder);
            }
            s = s + "\nRisk Analysis:\t" + (100 - entry.RiskMetric) + "\tNodes Affected:\t\t" + entry.NodeCount;
            s = s + "\nTests Risk:\t" + entry.TestsScore + "\tGraph Risk:\t\t" + entry.GraphScore;
            t.Content = s;// +"\nComplexity:\t" + entry.Complexity;

            return t;
        }

        private ToolTip BuildTestToolTip(string text)
        {
            return new ToolTip
            {
                Content = text + entry.TestType + " test.\nExecution Time:\t" + TimeFormatter.FormatTime(entry.AverageTimeUnder),
                Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom,
            };
        }

        private ToolTip GetStandardToolTip(string text)
        {
            return new ToolTip
            {
                Content = text,
                Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom
            };
        }

        private ToolTip BuildSlowTestToolTip()
        {
            return new ToolTip
            {
                Content = "SIGH! Slow Test, and it was so good.\nExecution Time:\t" + TimeFormatter.FormatTime(entry.AverageTimeUnder),
                Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom
            };
        }

        public void Detach()
        {
            entry.Invalidated -= CheckUpdate;
            entry.Changed -= Refresh;            
        }
    }
}