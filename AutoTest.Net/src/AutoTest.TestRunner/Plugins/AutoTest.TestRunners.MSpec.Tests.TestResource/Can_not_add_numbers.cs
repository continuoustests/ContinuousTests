using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace AutoTest.TestRunners.MSpec.Tests.TestResource
{
    public class Can_not_add_numbers
    {
        static int sum = 0;

        Because of = () => { sum = new Calc().Add(2, 3); };

        It results_in_4 = () => { sum.ShouldEqual(4); };
    }

    public class Calc
    {
        public int Add(int a, int b)
        {
            throw new Exception("I'm failing here");
        }
    }
}
