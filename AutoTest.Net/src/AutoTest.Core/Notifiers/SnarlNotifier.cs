using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using AutoTest.Core.DebugLog;
using System.Threading;

namespace AutoTest.Core.Notifiers
{
    public class SnarlNotifier : ISendNotifications
    {

        private const string APP_NAME = "AutoTest.Net";
        private const string CLASS_INFORMATION = "Information";
        private const string CLASS_SUCCESS = "Success";
        private const string CLASS_IGNORE = "Ignored";
        private const string CLASS_FAIL = "Failed";
        private const string MSG_TITLE = "AutoTest.Net";
        private Socket _sock = null;

        #region ISendNotifications Members

        public void Notify(string msg, NotificationType type)
        {
            string classType = "";
            switch (type)
            {
                case NotificationType.Information:
                    classType = CLASS_INFORMATION;
                    break;
                case NotificationType.Green:
                    classType = CLASS_SUCCESS;
                    break;
                case NotificationType.Yellow:
                    classType = CLASS_IGNORE;
                    break;
                case NotificationType.Red:
                    classType = CLASS_FAIL;
                    break;
            }
            var message = string.Format("type=SNP#?version=1.0#?action=notification#?app={0}#?class={1}#?title={2}#?text={3}#?timeout=5", APP_NAME, classType, MSG_TITLE, msg);
            Debug.WriteDebug("Sending Snarl notification: {0}", message);
            send(message);
        }

        public bool IsSupported()
        {
            if (!send(string.Format("type=SNP#?version=1.0#?action=register#?app={0}", APP_NAME)))
                return false;
            if (!send(string.Format("type=SNP#?version=1.0#?action=add_class#?app={0}#?class={1}#?title=Run information", APP_NAME, CLASS_INFORMATION)))
                return false;
            if (!send(string.Format("type=SNP#?version=1.0#?action=add_class#?app={0}#?class={1}#?title=Run succeeded", APP_NAME, CLASS_SUCCESS)))
                return false;
            if (!send(string.Format("type=SNP#?version=1.0#?action=add_class#?app={0}#?class={1}#?title=Run succeeded with warnings or ignored tests", APP_NAME, CLASS_IGNORE)))
                return false;
            if (!send(string.Format("type=SNP#?version=1.0#?action=add_class#?app={0}#?class={1}#?title=Run failed", APP_NAME, CLASS_FAIL)))
                return false;
            return true;
        }

        #endregion

        private void connect()
        {
            var host = IPAddress.Parse("127.0.0.1");
            var hostep = new IPEndPoint(host, 9887);
            _sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                _sock.Connect(hostep);
            }
            catch
            {
                _sock.Close();
                _sock = null;
            }
        }

        private bool send(string message)
        {
            _sock = null;
            try
            {
                var thread = new Thread(connect);
                thread.Start();
                var timeout = DateTime.Now.AddSeconds(2);
                while (DateTime.Now < timeout)
                {
                    Thread.Sleep(10);
                    if (_sock == null)
                        continue;
                    if (thread.ThreadState == ThreadState.Stopped)
                        break;
                }
                if (DateTime.Now > timeout)
                    thread.Abort();
                if (_sock == null)
                    return false;

                _sock.Send(Encoding.ASCII.GetBytes(message));
                _sock.Send(Encoding.ASCII.GetBytes("\r\n"));
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteException(e);
            }
            finally
            {
                if (_sock != null)
                    _sock.Close();
            }
            return false;
        }
    }
}
