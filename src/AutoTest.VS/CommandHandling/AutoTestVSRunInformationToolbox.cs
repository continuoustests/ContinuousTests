using System;
using AutoTest.Client.Logging;
using EnvDTE;
using EnvDTE80;
using AutoTest.VS.Util.CommandHandling;
using ContinuousTests.VS;

namespace AutoTest.VS.CommandHandling
{
    public class AutoTestVSRunInformationToolbox : ICommandHandler
    {
        private readonly DTE2 _applicationObject;
        private Window _toolWindow;
        private readonly AddIn _addInInstance;
        
        public AutoTestVSRunInformationToolbox(DTE2 applicationObject, AddIn addInInstance)
        {
            _applicationObject = applicationObject;
            _addInInstance = addInInstance;
        }

        public void Exec(vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled)
        {
            Logger.Write("in exec of toolbox");
            object docObj = new object();
            if (_toolWindow == null)
            {
                try
                {
                    Logger.Write("Starting information toolbox");
                    _toolWindow = _applicationObject.Windows.CreateToolWindow(_addInInstance, "AutoTestVSRunInformationToolbox", "ContinuousTests - Run Output Window", "{67663444-f874-401c-9e55-053aa0b5bd0b}", ref docObj);
                    _toolWindow.IsFloating = false;
                    Connect._control = (AutoTestVSRunInformation)docObj;
                    Connect._control.SetApplication(_applicationObject);
                    Connect._control.PrepareForFocus();
                    Logger.Write("Information toolbox started");
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            }
            else
            {
                // toggle window
                if (!Connect._control.IsInFocus())
                {
                    Connect._control.PrepareForFocus();
                    _toolWindow.Activate();
                }
                else
                {
                    _toolWindow.Close();
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
            get { return "ContinuousTests.VS.Connect.ContinuousTests_FeedbackWindow"; }
        }
    }
}