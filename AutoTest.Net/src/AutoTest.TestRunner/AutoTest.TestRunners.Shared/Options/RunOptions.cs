using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.TestRunners.Shared.Options
{
    [Serializable]
    public class RunOptions
    {
        private List<RunnerOptions> _runners = new List<RunnerOptions>();

        public IEnumerable<RunnerOptions> TestRuns { get { return _runners; } }

        public void AddTestRun(RunnerOptions runner)
        {
            _runners.Add(runner);
        }

        public void AddTestRun(IEnumerable<RunnerOptions> runners)
        {
            _runners.AddRange(runners);
        }
    }

    [Serializable]
    public class RunnerOptions
    {
        private List<AssemblyOptions> _assemblies = new List<AssemblyOptions>();
        private List<string> _categories = new List<string>();

        public string ID { get; private set; }

        public IEnumerable<AssemblyOptions> Assemblies { get { return _assemblies; } }
        public IEnumerable<string> Categories { get { return _categories; } }

        public RunnerOptions(string id)
        {
            ID = id;
        }

        public void AddAssembly(AssemblyOptions options)
        {
            _assemblies.Add(options);
        }

        public void AddAssemblies(AssemblyOptions[] options)
        {
            _assemblies.AddRange(options);
        }

        public void AddCategory(string category)
        {
            _categories.Add(category);
        }

        public void AddCategories(string[] categories)
        {
            _categories.AddRange(categories);
        }
    }

    [Serializable]
    public class AssemblyOptions
    {
        private List<string> _tests = new List<string>();
        private List<string> _members = new List<string>();
        private List<string> _namespaces = new List<string>();

        public bool IsVerified { get; private set; }
        public string Assembly { get; private set; }
        public IEnumerable<string> Tests { get { return _tests; } }
        public IEnumerable<string> Members { get { return _members; } }
        public IEnumerable<string> Namespaces { get { return _namespaces; } }

        public AssemblyOptions(string assembly)
        {
            Assembly = assembly;
        }

        public AssemblyOptions HasBeenVerified(bool verified)
        {
            IsVerified = verified;
            return this;
        }

        public void AddTest(string test) { _tests.Add(test); }
        public void AddTests(string[] tests) { _tests.AddRange(tests); }
        public void AddMember(string member) { _members.Add(member); }
        public void AddMembers(string[] members) { _members.AddRange(members); }
        public void AddNamespace(string ns) { _namespaces.Add(ns); }
        public void AddNamespaces(string[] namespaces) { _namespaces.AddRange(namespaces); }
    }
}
