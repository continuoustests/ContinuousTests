using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;
using EnvDTE80;
using AutoTest.VS.Util.CommandHandling;
using AutoTest.Client.Logging;
using ContinuousTests.VS;

namespace AutoTest.VS.CommandHandling
{
    public class LastTestsRanHandler : ICommandHandler
    {
        private readonly DTE2 _applicationObject;
        private Window _toolWindow;
        private readonly AddIn _addInInstance;

        public LastTestsRanHandler(DTE2 applicationObject, AddIn addInInstance)
        {
            _applicationObject = applicationObject;
            _addInInstance = addInInstance;
        }

        public void Exec(vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled)
        {
            object docObj = new object();
            if (Connect.LastRanTestsWindow == null)
            {
                try
                {
                    Logger.Write("Starting last run tests toolbox");
                    Connect.LastRanTestsWindow = _applicationObject.Windows.CreateToolWindow(_addInInstance, "ContinuousTests_ListOfRanTests", "ContinuousTests - Last Ran Tests", "{67663444-f874-401c-9e55-053bb0b5bd0b}", ref docObj);
                    Connect.LastRanTestsWindow.IsFloating = false;
                    Connect.LastRanTestsControl = (ContinuousTests_ListOfRanTests)docObj;
                    Connect.LastRanTestsControl.SetApplication(_applicationObject);
                    Connect.LastRanTestsWindow.Activate();
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            }
            else
            {
                // toggle window
                if (!Connect.LastRanTestsControl.IsInFocus())
                {
                    Connect.LastRanTestsWindow.Activate();
                }
                else
                {
                    Connect.LastRanTestsWindow.Close();
                }
            }
            Handled = true;
        }

        public void QueryStatus(vsCommandStatusTextWanted NeededText, ref vsCommandStatus StatusOption, ref object CommandText)
        {
            StatusOption = vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
        }

        public string Name
        {
            get { return "ContinuousTests.VS.Connect.ContinuousTests_LastRanTestsWindow"; }
        }
    }
}
