using System;
using Simple.Testing.ClientFramework;

namespace AutoTest.TestRunners.SimpleTesting.Tests.Resources
{

    public class SomeClass
    {
        public void Foo()
        {

        }
    }

    public class SimpleTestingTests
    {
        public Specification a_passing_test()
        {
            return new QuerySpecification<Foo, int>
                       {
                           On = () => new Foo(),
                           When = x => x.Bar(),
                           Expect =
                               {
                                   q => q == 12
                               }
                       };
        }
        
        [MightyMooseIgnore]
        public Specification a_failing_test()
        {
            return new QuerySpecification<Foo, int>
                       {
                           On = () => new Foo(),
                           When = x => x.Bar(),
                           Expect =
                               {
                                   q => q == 15
                               }
                       };
        }

        [MightyMooseIgnore]
        public Specification a_failing_test_with_exception()
        {
            return new QuerySpecification<Foo, int>
            {
                On = () => { throw new Exception(); },
                When = x => x.Bar(),
                Expect =
                               {
                                   q => q == 15
                               }
            };
        }
    }


    public class Foo
    {
        public int Bar()
        {
            return 12;
        }
    }

    public class MightyMooseIgnoreAttribute : Attribute { }
    namespace more
    {
        public class MoreSimpleTestingTests
        {
            public class Nested
            {
                public Specification a_passing_test()
                {
                    return new QuerySpecification<Foo, int>
                    {
                        On = () => new Foo(),
                        When = x => x.Bar(),
                        Expect =
                                   {
                                       q => q == 12
                                   }
                    };
                }   
            }
            public Specification a_passing_test()
            {
                return new QuerySpecification<Foo, int>
                           {
                               On = () => new Foo(),
                               When = x => x.Bar(),
                               Expect =
                                   {
                                       q => q == 12
                                   }
                           };
            }
        }
    }

}
