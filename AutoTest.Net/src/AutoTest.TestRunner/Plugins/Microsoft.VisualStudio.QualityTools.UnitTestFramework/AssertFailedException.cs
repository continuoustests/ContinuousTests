using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    public class AssertFailedException : UnitTestAssertException
    {
        public AssertFailedException(string msg) : base(msg) {}
        public AssertFailedException(string msg, Exception ex) : base(msg, ex) {}
    }
}
