using System;
using NUnit.Framework;

namespace AutoTest.Test
{
    [TestFixture]
    public class TestingExtensionMethodsTests
    {
        [Test]
        public void Should_be_equal()
        {
            1.ShouldEqual(1);
            "asdf".ShouldEqual("asdf");
            new[] {"one", "two"}.ShouldEqual(new[] {"one", "two"});
        }

        [Test]
        public void Should_Be_False()
        {
            false.ShouldBeFalse();
        }

        [Test]
        public void Should_Be_True()
        {
            true.ShouldBeTrue();
        }

        [Test]
        public void Should_be_right_type()
        {
            "asdf".ShouldBeOfType<string>();
            new TestingExtensionMethodsTests().ShouldBeOfType<Object>();
        }
		
		[Test]
		public void Should_not_be_of_the_right_type()
		{
			new TestingExtensionMethodsTests().ShouldNotBeOfType<IServiceProvider>();
		}

        [Test]
        public void Should_be_null()
        {
            TestMethod().ShouldBeNull();
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void Should_not_be_null()
        {
            "one".ShouldBeNull();
        }

        private string TestMethod()
        {
            return null;
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void Should_make_sure_thing_is_not_null()
        {
            string bang = null;
            bang.ShouldNotBeNull();
        }

        [Test]
        public void SHould_not_say_thing_is_null_and_should_allow_chaining()
        {
            "Hi".ShouldNotBeNull().ShouldEqual("Hi");
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void Should_be_wrong_type()
        {
            "asdf".ShouldBeOfType<Array>();
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void Should_not_be_equal_on_numbers()
        {
            1.ShouldEqual(0);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void Should_not_be_equal_on_strings()
        {
            "asDF".ShouldEqual("asdf");
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void Should_not_be_equal_on_string_arrays()
        {
            new[] { "one", "two" }.ShouldEqual(new[] { "two", "one" });
        }

        [Test]
        public void Shoud_be_same()
        {
            object o = new object();
            o.ShouldBeTheSameAs(o);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void Shoud_be_same_should_fail_when_not_same()
        {
            object o = new object();
            object o2 = new object();
            o.ShouldBeTheSameAs(o2);
        }

        [Test]
        public void Should_not_be_same()
        {
            object o = new object();
            object o2 = new object();
            o.ShouldNotBeTheSameAs(o2);
        }

        [Test]
        [ExpectedException(typeof(AssertionException))]
        public void Should_not_be_same_should_fail_when_same()
        {
            object o = new object();
            o.ShouldNotBeTheSameAs(o);
        }
    }
}