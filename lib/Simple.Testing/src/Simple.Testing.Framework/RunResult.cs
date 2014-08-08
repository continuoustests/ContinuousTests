using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Simple.Testing.Framework
{
    public class RunResult
    {
        public bool Passed;
        public string Message;
        public Exception Thrown;
        public string SpecificationName;
        public List<ExpectationResult> Expectations = new List<ExpectationResult>();
        public MemberInfo FoundOnMemberInfo;
        public Delegate On;
        public object Result;

        public object GetOnResult()
        {
            return On.DynamicInvoke();
        }

        public string Name
        {
            get { return SpecificationName ?? FoundOnMemberInfo.Name; }
        }

        internal void MarkFailure(string message, Exception thrown)
        {
            Thrown = thrown;
            Message = message;
            Passed = false;
        }
    }


    public class ExpectationResult
    {
        public bool Passed;
        public string Text;
        public Exception Exception;
        public Expression OriginalExpression;
    }
}