//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Machine.Specifications;

//namespace TestProject
//{
//    class Operator
//    {
//        internal decimal Add(decimal p, decimal p_2)
//        {
//            return p + p;
//        }
//    }
//    public class Foo
//    {
//        [Subject("adding two operands")]
//        public class when_adding_two_operands
//        {
//            static decimal value;

//            int i = 20;

//            Establish context = () =>
//                value = 0m;

//            Because of = () =>
//                value = new Operator().Add(42.0m, 42.0m);

//            private It should_add_both_operands = () =>
//                                                      {
//                                                          value.ShouldEqual(84.0m);
//                                                      };
//        }
//    }
//}
