using System;
using NUnit.Framework;

namespace TestProject
{
    public class NamingProblem
    {
        public static T Foo<T, V>(int x)
        {
            return default(T);
        }

        public static T Foo<T>(int x)
        {
            var q1 = 14;
            return default(T);
        }
    }
    public interface Handles<T>
    {
        void Handle(T item);
    }

    class Handler1 : Handles<int>
    {
        public void Handle(int item)
        {
            Console.WriteLine("foo");
        }
    }

    class Handler2 : Handles<string>, Handles<int>
    {
        public void Handle(string item)
        {
            Console.WriteLine("y");
        }

        public void Handle(int item)
        {
            throw new NotImplementedException();
        }
    }

    [TestFixture]
    public class StringTest : handlertests<int, string>
    {
        protected override Handles<string> Build()
        {
            return new Handler2();
        }

        protected override string Given()
        {
            return "";
        }

        [Test]
        public void stringnoexception()
        {
            Assert.IsNull(caught);
        }

        [Test]
        public void stringsomething_else()
        {
            Console.Write(12);
        }
    }
    [TestFixture]
    public class IntTest : handlertests<object, int>
    {
        protected override Handles<int> Build()
        {
            return new Handler1();
        }

        protected override int Given()
        {
            return 5;
        }

        [Test]
        public void noexception()
        {
            int x = 5;
            Assert.IsNull(caught);
        }

        [Test]
        public void something_else()
        {
            Console.Write(12);
        }
    }
    public abstract class handlertests<TSomething, TCommand>
    {
        private Handles<TCommand> handler;
        protected abstract Handles<TCommand> Build();
        protected Exception caught;

        [SetUp]
        public void Setup()
        {
            handler = Build();
            try
            {
                handler.Handle(Given());
            }
            catch (Exception ex)
            {
                caught = ex;
            }
        }

        protected abstract TCommand Given();
    }


    interface IFoo<T>
    {
        void Foo(T obj);
    }

    class FooImplementor : IFoo<int>
    {

        public void Foo(int obj)
        {
            int x = 22;
        }
    }

    class GenericFooImplementor<T> : IFoo<T>
    {
        public void Foo(T obj)
        {
            throw new NotImplementedException();
        }
    }


    class shitbird
    {
        private int x = 0;
        private int j = 0;

        public void Method3()
        {
            var c = "hello";
            x = 1;
        }

        public void Method2()
        {
            int l = 5;
            Console.WriteLine(j);
            Console.WriteLine("hello2");
        }

        public int Method()
        {
            var foo = "Salut Julien";
            return x;
        }

        public void Method4()
        {
            int y = 3;
            Console.WriteLine(y);
        }

        public void Method5()
        {
            Console.Write(x + 7);
        }
    }
}
