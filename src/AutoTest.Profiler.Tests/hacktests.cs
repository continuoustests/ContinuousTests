using System.Threading;
using NUnit.Framework;

namespace AutoTest.Profiler.Tests
{
    [TestFixture]
    public class hacktests
    {
        [Test]
        public void hacks_generics()
        {
            Assert.AreEqual("System.Void Foo'2::Bar()", Hack.Generics("System.Void Foo'2<T,V>::Bar()"));
        }

        [Test]
        public void hacks_generic_arguments()
        {
            Assert.AreEqual("T Foo::Bar()", Hack.Generics("T Foo::Bar<T>()"));
        }

        [Test]
        public void string_didnt_parse_properly()
        {
            Assert.AreEqual(",System.Void ::_initterm_m)", Hack.Generics(",System.Void ::_initterm_m)"));
        }

        [Test]
        public void hack_multi_level_generics()
        {
            Assert.AreEqual(
                "Fohjin.DDD.CommandHandlers.ICommandHandler`1<T,V> Test.Fohjin.DDD.CommandTestFixture`3::BuildCommandHandler()",
                Hack.Generics(
                    "Fohjin.DDD.CommandHandlers.ICommandHandler`1<T,V> Test.Fohjin.DDD.CommandTestFixture`3<TCommand,TCommandHandler,TAggregateRoot>::BuildCommandHandler()"));
        }
        [Test]
        public void adds_void_to_empty()
        {
            Assert.AreEqual("System.Void Bar::Foo(String)", Hack.HackName("Bar.Foo(String)"));
        }

        [Test]
        public void adds_parens_when_missing()
        {
            Assert.AreEqual("System.Void Bar::Foo()", Hack.HackName("System.Void Bar.Foo"));
        }

        [Test]
        public void does_not_mangle_compiler_made_names()
        {
            Assert.AreEqual("System.Void Machine.Specifications.Example.when_transferring_an_amount_larger_than_the_balance_of_the_from_account::<.ctor>b__2()", Hack.Generics("System.Void Machine.Specifications.Example.when_transferring_an_amount_larger_than_the_balance_of_the_from_account::<.ctor>b__2()"));
        }
    }
}
