using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ClassInitializeAttribute : Attribute
    {
    }
}
