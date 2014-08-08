using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.Messages;
using System.IO;

namespace AutoTest.Messages
{
    public class OnDemandRun
    {
        public bool RunAllTestsInProject { get; private set; }
        public string Project { get; private set; }
        public string[] Tests { get; private set; }
        public string[] Members { get; private set; }
        public string[] Namespaces { get; private set; }

        public OnDemandRun(string project)
        {
            RunAllTestsInProject = false;
            Project = project;
            Tests = new string[] {};
            Members = new string[] { };
            Namespaces = new string[] { };
        }

        public OnDemandRun(string project, string[] tests, string[] members, string[] namespaces)
        {
            RunAllTestsInProject = false;
            Project = project;
            Tests = tests;
            Members = members;
            Namespaces = namespaces;
        }

        public OnDemandRun ShouldRunAllTestsInProject()
        {
            RunAllTestsInProject = true;
            return this;
        }

        public void JoinWith(OnDemandRun run)
        {
            Tests = join(Tests, run.Tests);
            Members = join(Members, run.Members);
            Namespaces = join(Namespaces, run.Namespaces);
        }

        public void JoinWith(IEnumerable<string> tests, IEnumerable<string> members, IEnumerable<string> namespaces)
        {
            Tests = join(Tests, tests.ToArray());
            Members = join(Members, members.ToArray());
            Namespaces = join(Namespaces, namespaces.ToArray());
        }

        private string[] join(string[] list1, string[] list2)
        {
            var list = new List<string>();
            list.AddRange(list1);
            list.AddRange(list2);
            return list.ToArray();
        }
    }

    public class OnDemandTestRunMessage : IMessage
    {
        private List<OnDemandRun> _runs;

        public IEnumerable<OnDemandRun> Runs { get { return _runs; } }

        public OnDemandTestRunMessage()
        {
            _runs = new List<OnDemandRun>();
        }

        public void AddRun(OnDemandRun run)
        {
            _runs.Add(run);
        }

        public void SetDataFrom(BinaryReader reader)
        {
            _runs = new List<OnDemandRun>();
            var count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var runAll = reader.ReadBoolean();
                var project = reader.ReadString();
                var tests = getStringList(reader);
                var members = getStringList(reader);
                var namespaces = getStringList(reader);
                var run = new OnDemandRun(project, tests, members, namespaces);
                if (runAll)
                    run.ShouldRunAllTestsInProject();
                _runs.Add(run);
            }
        }

        private string[] getStringList(BinaryReader reader)
        {
            var strings = new List<string>();
            var count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
                strings.Add(reader.ReadString());
            return strings.ToArray();
        }

        public void WriteDataTo(BinaryWriter writer)
        {
            writer.Write(_runs.Count);
            foreach (var run in _runs)
                writeRunner(writer, run);
        }

        private void writeRunner(BinaryWriter writer, OnDemandRun run)
        {
            writer.Write(run.RunAllTestsInProject);
            writer.Write(run.Project);
            writeStringList(writer, run.Tests);
            writeStringList(writer, run.Members);
            writeStringList(writer, run.Namespaces);
        }

        private void writeStringList(BinaryWriter writer, string[] list)
        {
            writer.Write(list.Length);
            foreach (var item in list)
                writer.Write(item);
        }
    }
}
