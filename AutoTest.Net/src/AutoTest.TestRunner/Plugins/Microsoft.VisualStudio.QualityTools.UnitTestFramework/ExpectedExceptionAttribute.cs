using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    public sealed class ExpectedExceptionAttribute : ExpectedExceptionBaseAttribute
    {
        private Type _exception = null;

        public ExpectedExceptionAttribute(Type exceptionType) { _exception = exceptionType; }
        public ExpectedExceptionAttribute(Type exceptionType, string noExceptionMessage) { _exception = exceptionType; }
        public bool AllowDerivedTypes { get; set; }
        public Type ExceptionType { get { return _exception; } }
    }
}
