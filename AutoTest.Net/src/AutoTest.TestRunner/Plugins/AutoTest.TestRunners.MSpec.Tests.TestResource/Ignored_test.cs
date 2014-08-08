using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace AutoTest.TestRunners.MSpec.Tests.TestResource
{
    [Ignore("This test is ignored")]
    public class Ignored_test
    {
        It is_ignored = () => true.Equals(false);
    }
}
