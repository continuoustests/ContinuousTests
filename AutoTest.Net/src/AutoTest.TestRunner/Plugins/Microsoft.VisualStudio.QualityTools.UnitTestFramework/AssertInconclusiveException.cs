using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    [Serializable]
    public class AssertInconclusiveException : UnitTestAssertException
    {
        public AssertInconclusiveException() {}
        public AssertInconclusiveException(string msg) : base(msg) {}
        protected AssertInconclusiveException(SerializationInfo info, StreamingContext context) : base(info, context) {}
        public AssertInconclusiveException(string msg, Exception ex) : base(msg, ex) {}
    }
}
