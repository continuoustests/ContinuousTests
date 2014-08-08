using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace AutoTest.TestRunners.MSpec.Tests.TestResource
{
    [Subject("When adding two numbers")]
    public class Can_add_numbers
    {
        static int sum = 0;
        
        Because of = () => sum = 2 + 2;

        It results_in_4 = () => sum.Equals(4);
    }

    public class SomeClass
    {
    }
}
