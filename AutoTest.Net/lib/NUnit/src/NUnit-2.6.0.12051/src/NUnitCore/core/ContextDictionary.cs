// ****************************************************************
// Copyright 2011, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Collections;

namespace NUnit.Core
{
    public class ContextDictionary : Hashtable
    {
        internal TestExecutionContext _ec;

        public override object this[object key]
        {
            get
            {
                // Get Result values dynamically, since
                // they may change as execution proceeds
                switch (key as string)
                {
                    case "Test.Name":
                        return _ec.CurrentTest.TestName.Name;
                    case "Test.FullName":
                        return _ec.CurrentTest.TestName.FullName;
                    case "Test.Properties":
                        return _ec.CurrentTest.Properties;
                    case "Result.State":
                        return (int)_ec.CurrentResult.ResultState;
                    case "TestDirectory":
                        return AssemblyHelper.GetDirectoryName(_ec.CurrentTest.FixtureType.Assembly);
                    case "WorkDirectory":
                        return _ec.TestPackage.Settings.Contains("WorkDirectory")
                            ? _ec.TestPackage.Settings["WorkDirectory"]
                            : Environment.CurrentDirectory;
                    default:
                        return base[key];
                }
            }
            set
            {
                base[key] = value;
            }
        }
    }
}
