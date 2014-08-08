using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AutoTest.VM.Messages.Communication;
using System.Threading;
using AutoTest.Messages;
using AutoTest.VM.Messages;
using System.IO;

namespace AutoTest.Diagnostics
{
    public partial class MessageMonitor : Form
    {
        private SynchronizationContext _syncContext;
        private List<DiagnosticNode> _nodes = new List<DiagnosticNode>();
        private Thread _serverInstanceConnector;
        private Thread _standaloneConnector;
        private Thread _invalidateHandler;

        public MessageMonitor()
        {
            _syncContext = AsyncOperationManager.SynchronizationContext;
            InitializeComponent();
        }

        private void MessageMonitor_Shown(object sender, EventArgs e)
        {
            _serverInstanceConnector = new Thread(connectToServerInstance);
            _serverInstanceConnector.Start();
            _standaloneConnector = new Thread(connectToStandAloneInstances);
            _standaloneConnector.Start();
            _invalidateHandler = new Thread(invalidateNodes);
            _invalidateHandler.Start();
        }

        private void connectToServerInstance()
        {
            DiagnosticNode server = null;
            var connected = false;
            while (!connected)
            {
                try
                {
                    server = new DiagnosticNode("127.0.0.1", 9070, 0, "Server");
                    if (server.IsConnected)
                    {
                        addListItem(new ListItemInfo<string>("Connected"));
                        server.MessageReceived += new EventHandler<VM.Messages.Communication.MessageReceivedEventArgs>(node_MessageReceived);
                        connected = true;
                    }
                    else
                    {
                        Thread.Sleep(5000);
                    }
                }
                catch
                {
                }
            }
        }

        private void invalidateNodes()
        {
            while (true)
            {
                removeAllNodes();
                Thread.Sleep(5000);
            }
        }

        

        private void connectToStandAloneInstances()
        {
            var handleManager = new VMConnectHandle();
            while (true)
            {
                var handles = handleManager.GetAll().Where(x => !_nodes.Exists(y => y.IP.Equals(x.IP) && y.Port.Equals(x.Port))).ToList();
                foreach (var handle in handles)
                {
                    var node = new DiagnosticNode(
                        handle.IP,
                        handle.Port,
                        handle.ProcessID,
                        handle.Token,
                        (m) => node_MessageReceived(handle.Token, new MessageReceivedEventArgs(m)));
                    if (node.IsConnected)
                    {
                        addNode(node);
                    }
                    else
                    {
                        try
                        {
                            File.Delete(handle.File);
                        }
                        catch
                        {
                        }
                    }
                }
                Thread.Sleep(5000);
            }
        }

        void node_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            _syncContext.Post((m) =>
                {
                    if (!sentFromActiveNode(sender.ToString()))
                        return;
                    addListItem(new ListItemInfo<object>(e.Message));

                    if (e.Message.GetType().Equals(typeof(VMSpawnedMessage)))
                    {
                        var message = (VMSpawnedMessage)e.Message;
                        var client = new DiagnosticNode("127.0.0.1", message.Port, 0, "Client");
                        addListItem(new ListItemInfo<string>("Connected"));
                        addNode(client);
                    }

                    if (e.Message.GetType().Equals(typeof(VMTerminating)))
                    {
                        var message = (VMTerminating)e.Message;
                        var client = _nodes.Where(n => n.IP.Equals("127.0.0.1") && n.Port.Equals(message.Port)).FirstOrDefault();
                        if (client != null)
                        {
                            addListItem(new ListItemInfo<string>("Disconnected"));
                            removeNode(client);
                        }
                    }
                }, null);
        }

        private bool sentFromActiveNode(string sender)
        {
            if (listBoxNodes.SelectedItem == null)
                return false;
            var selected = (DiagnosticNode)listBoxNodes.SelectedItem;
            if (selected.Handle.Equals(sender))
                return true;
            return false;
        }

        private void addListItem(ListItemInfo<string> info)
        {
            _syncContext.Post((y) =>
            {
                var item = (ListItemInfo<string>)y;
                var listItem = listView.Items.Insert(0, item.Message);
                listItem.Tag = item.Message;
            }, info);
        }

        private void addListItem(ListItemInfo<object> info)
        {
            _syncContext.Post((y) =>
            {
                var item = (ListItemInfo<object>)y;
                var time = DateTime.Now;
                var listItem = listView.Items.Insert(0, string.Format("{0}:{1}:{2}.{3}",
                    time.Hour.ToString().PadLeft(2, '0'),
                    time.Minute.ToString().PadLeft(2, '0'),
                    time.Second.ToString().PadLeft(2, '0'),
                    time.Millisecond));
                listItem.SubItems.Add(item.Message.GetType().ToString());
                listItem.Tag = item.Message;
            }, info);
        }

        private void listView_DoubleClick(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count != 1)
                return;
            var item = listView.SelectedItems[0].Tag;
            var form = new MessageReflector(item.GetType().ToString(), item);
            form.Show();
        }

        private void MessageMonitor_FormClosing(object sender, FormClosingEventArgs e)
        {
            _serverInstanceConnector.Abort();
            _standaloneConnector.Abort();
            _invalidateHandler.Abort();
        }

        private void removeNode(DiagnosticNode client)
        {
            lock (_nodes)
            {
                _nodes.Remove(client);
                _syncContext.Post((state) => listBoxNodes.Items.Remove(client), null);
            }
        }

        private void addNode(DiagnosticNode client)
        {
            lock (_nodes)
            {
                _nodes.Add(client);
                _syncContext.Post((state) => listBoxNodes.Items.Add(client), null);
            }
        }

        private void removeAllNodes()
        {
            lock (_nodes)
            {
                var nodes = _nodes.Where(x => x == null || !x.IsConnected);
                nodes.ToList().ForEach(x =>
                    {
                        _nodes.Remove(x);
                        _syncContext.Post((state) => listBoxNodes.Items.Remove(x), null);
                    });
            }
        }

        private void listBoxNodes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxNodes.SelectedItem == null)
                return;
            var item = (DiagnosticNode) listBoxNodes.SelectedItem;
            listView.Items.Clear();
            item.Messages.ToList().ForEach(x => addListItem(new ListItemInfo<object>(x)));
        }
    }
}
