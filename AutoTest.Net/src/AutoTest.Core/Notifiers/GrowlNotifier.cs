using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using AutoTest.Core.Configuration;

namespace AutoTest.Core.Notifiers
{
    public class GrowlNotifier : ISendNotifications
    {
		private IConfiguration _configuration;
        private string _growl_executable = null;

		public GrowlNotifier(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		
        #region ISendNotifications Members

        public void Notify(string msg, NotificationType type)
        {
            try
            {
                runNotification(msg, type);
            }
            catch (Exception ex)
            {
                DebugLog.Debug.WriteError(ex.ToString());
            }
        }

        public bool IsSupported()
        {
            try
            {
                if (_growl_executable == null)
                    locateGrowlExecutable();
                return File.Exists(_growl_executable);
            }
            catch (Exception ex)
            {
                DebugLog.Debug.WriteError(ex.ToString());
            }
            return false;
        }

        #endregion

        private void locateGrowlExecutable()
        {
			if (_configuration != null && _configuration.GrowlNotify != null && File.Exists(_configuration.GrowlNotify))
				_growl_executable = _configuration.GrowlNotify;
            else if (File.Exists(@"C:\Program Files\Growl for Windows\growlnotify.exe"))
                _growl_executable = @"C:\Program Files\Growl for Windows\growlnotify.exe";
            else if (File.Exists(@"C:\Program Files (x86)\Growl for Windows\growlnotify.exe")) 
                _growl_executable = @"C:\Program Files (x86)\Growl for Windows\growlnotify.exe";
			else if (File.Exists("/usr/local/bin/growlnotify"))
				_growl_executable = "/usr/local/bin/growlnotify";
        }

        private void runNotification(string msg, NotificationType type)
        {
            if (_growl_executable == null)
                locateGrowlExecutable();
            var bleh = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string icon = bleh;
            switch (type)
            {
                case NotificationType.Green:
                    icon = Path.Combine(icon, Path.Combine("Icons", "circleWIN.png"));
                    break;
                case NotificationType.Yellow:
                    icon = Path.Combine(icon, Path.Combine("Icons", "circleWAIL.png"));
                    break;
                case NotificationType.Red:
                    icon = Path.Combine(icon, Path.Combine("Icons", "circleFAIL.png"));
                    break;
            }
            //icon = icon.Replace(" ", "%20");
			string args;
			if (OS.IsOSX)
				args = string.Format("--image \"{0}\" --title \"AutoTest.Net\" --message \"{1}\"", icon, msg);
			else
            	args = string.Format("/t:\"AutoTest.Net\" /i:\"{0}\" \"{1}\"", icon, msg);
            DebugLog.Debug.WriteDebug("Sending Growl notification: {0} {1}", _growl_executable, args);
            var process = new Process();
            process.StartInfo = new ProcessStartInfo(_growl_executable, args);
            process.StartInfo.CreateNoWindow = true;
            process.Start();
        }
    }
}
