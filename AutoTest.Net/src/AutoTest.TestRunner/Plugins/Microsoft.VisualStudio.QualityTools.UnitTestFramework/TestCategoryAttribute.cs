using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    public sealed class TestCategoryAttribute : Attribute
    {
        public IList<string>  TestCategories = new List<string>();

        public TestCategoryAttribute(string category)
        {
            TestCategories.Add(category);
        }
    }
}
