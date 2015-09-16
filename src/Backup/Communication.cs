using System;
using System.Collections.Generic;
using System.Threading;
using AutoTest.Client.Logging;
using AutoTest.Messages;
using AutoTest.VM.Messages;
using AutoTest.VM.Messages.Communication;

namespace AutoTest.VS.RiskClassifier
{
    public class Communication
    {
        private NetClient client;
        private readonly Stack<string> _queue = new Stack<string>();
        private Timer timeoutCheck;

        public Communication()
        {
            var thread = new Thread(ConnectToExistingVm);
            thread.Start();
            timeoutCheck = new Timer(x => SendIfNeeded(), null, 1000, 500);


        }

        private void ConnectToExistingVm()
        {
            VMHandle handle = null;
            while(handle == null)
            {
                handle = new VMConnectHandle().GetCurrent();
            }
            client = new NetClient(new DebugClientFeedbackProvider());
            client.MessageReceived += ClientMessageReceived;
            client.Connect(handle.IP, handle.Port);
        }

        void ClientMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            ProcessMessage(e);
        }

        public void ProcessMessage(MessageReceivedEventArgs e)
        {
            try
            {
                Logger.Write("Risk: Message Found of " + e.Message.GetType());
                if (e.Message.GetType() == typeof (RiskMetricGeneratedMessage))
                {
                    var message = (RiskMetricGeneratedMessage) e.Message;
                    Logger.Write("Received risk metric from backend for " + message.Signature + " count = " +
                                 message.NodeCount);
                    EntryCache.Update(message);
                }
                if (e.Message.GetType() == typeof (ProfilerCompletedMessage) ||
                    e.Message.GetType() == typeof (ProfilerInitializedMessage) ||
                    e.Message.GetType() == typeof (AssembliesMinimizedMessage) ||
                    e.Message.GetType() == typeof (MinimizerInitializedMessage))
                {
                    Logger.Write("LL:Received Profiler Message: Invalidating.");
                    EntryCache.Invalidate();
                }
                if (e.Message.GetType() == typeof (CacheMessages))
                {
                    CurrentTestStatuses.UpdateWith((CacheMessages) e.Message);
                }
            }
            catch(Exception ex)
            {
                Logger.Write("Process message" + ex);
            }
        }

        public void UpdateEntry(string signature)
        {
            SendIfNeeded();
            if (!IsConnected())
            {
                Enqueue(signature);
                Logger.Write("Enqueuing request for risk metric of " + signature + ". Not yet connected.");
                return;
            }
            else
            {
                Send(signature);
            }
        }

        private void SendIfNeeded()
        {
            lock (_queue)
            {
                if (!IsConnected()) return;
                if (HasEnqueuedSignatures())
                    EmptyQueue();
            }

        }

        private void Enqueue(string signature)
        {
            lock (_queue)
            {
                if (_queue.Count == 500)
                    _queue.Pop();
                _queue.Push(signature);
            }
        }

        private void EmptyQueue()
        {
            lock (_queue)
            {
                while (_queue.Count != 0)
                    Send(_queue.Pop());
            }
        }

        private bool HasEnqueuedSignatures()
        {
            lock (_queue)
            {
                return _queue.Count > 0;
            }
        }

        private void Send(string signature)
        {
            var message = new RequestRiskMetricsMessage(signature);
            Logger.Write("Sending request for risk metric of " + signature);
            client.Send(message);
        }

        private bool IsConnected()
        {
            if (client == null)
                return false;
            return client.IsConnected;
        }
    }
}