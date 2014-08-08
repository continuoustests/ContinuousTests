using System.Collections.Generic;
using NUnit.Framework;

namespace TestProject
{
    class Fubbar
    {
        public IEnumerable<int> data()
        {
            yield return 12;
            yield return 27;
            yield return 4;
        }
    }


    class Base
    {
        public int X { get; set; }

        protected Base(int i)
        {
            X = i;
        }
    }

    class Derived1 : Base
    {
        public Derived1(int x) : base(x)
        {
            x = 24;
        }
    }

    class Derived2 : Base
    {
        public Derived2(int x) : base(x)
        {
            
        }
    }

    class x
    {
        [Test]
        public void a_method_using_derived_2()
        {
            var y = new Derived2(12);
            Assert.IsTrue(y.X == 12);
        }
        [Test]
        public void a_method_using_derived_1()
        {
            var y = new Derived1(12);
            Assert.IsTrue(y.X == 12);
        }
    }

    public class EfferentInheritanceSample
    {
        
    }
}