using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace AutoTest.Diagnostics
{
    public partial class MessageReflector : Form
    {
        public MessageReflector(string text, object message)
        {
            InitializeComponent();
            Text = text;
            ReflectMessage(message);
        }

        private void ReflectMessage(object message)
        {
            var master = treeView.Nodes.Add(message.GetType().ToString());
            master.Tag = message;
            master.Expand();
            if (message.GetType().Namespace.Equals("System"))
                master.Nodes.Add(message.ToString());
            else
                addMembers(message, master);
        }

        private void addMembers(object message, TreeNode master)
        {
            var members = message.GetType().GetMembers();
            foreach (var member in members)
            {
                if (member.MemberType == MemberTypes.Property)
                    addNode(member, master, message);
            }
        }

        private void addNode(MemberInfo member, TreeNode parent, object parentObject)
        {
            var value = parentObject.GetType().InvokeMember(member.Name, BindingFlags.GetProperty, null, parentObject, null);
            if (value == null)
            {
                var nullNode = parent.Nodes.Add(string.Format("{0}:null", member.Name));
                return;
            }

            var node = parent.Nodes.Add(string.Format("{0}:{1}", member.Name, value.ToString()));
            node.Tag = value;
            if (value.GetType().Namespace.Equals("System"))
                return;
            if (value.GetType().IsArray)
            {
                addArrayNodes(node, value);
                return;
            }
            node.Nodes.Add("placeholder");
        }

        private void addArrayNodes(TreeNode node, object value)
        {
            Type arrayType = value.GetType().GetElementType();
	        object[] arr = value as object[];
	        for (int i = 0; i < arr.Length; i++)
	        {
                var child = node.Nodes.Add(i.ToString());
                child.Tag = arr[i];
                if (value.GetType().Namespace.Equals("System"))
                    continue;
                child.Nodes.Add("placeholder");
	        }
        }

        private void treeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Nodes.Count.Equals(1) && e.Node.Nodes[0].Text.Equals("placeholder"))
            {
                e.Node.Nodes.Clear();
                var parent = e.Node.Tag;
                addMembers(parent, e.Node);
            }
        }
    }
}
