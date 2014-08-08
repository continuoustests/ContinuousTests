using NUnit.Framework;

namespace AutoTest
{
    public static class TestingExtensionMethods
    {
        public static void ShouldBeNull<T>(this T actual)
        {
            Assert.IsNull(actual);
        }

        public static T ShouldEqual<T>(this T actual, T expected)
        {
            Assert.AreEqual(expected, actual);
            return actual;
        }

        public static void ShouldBeFalse(this bool actual)
        {
            Assert.IsFalse(actual);
        }

        public static void ShouldBeTrue(this bool actual)
        {
            Assert.IsTrue(actual);
        }

        public static T ShouldBeOfType<T>(this object actual) where T : class
        {
            Assert.IsInstanceOf<T>(actual);
            return actual as T;
        }
		
		public static void ShouldNotBeOfType<T>(this object actual) where T : class
		{
			Assert.IsNotInstanceOf<T>(actual);
		}
        
        public static T ShouldNotBeNull<T>(this T actual)
        {
            Assert.IsNotNull(actual);
            return actual;
        }

        public static void ShouldBeTheSameAs<T>(this T actual, T expected)
        {
            Assert.AreSame(expected, actual);
        }

        public static void ShouldNotBeTheSameAs<T>(this T actual, T expected)
        {
            Assert.AreNotSame(expected, actual);
        }
    }
}