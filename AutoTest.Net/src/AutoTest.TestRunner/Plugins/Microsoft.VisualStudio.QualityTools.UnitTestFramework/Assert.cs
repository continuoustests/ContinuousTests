using System;
using System.Globalization;
using NUnitAssert = NAssert.Assert;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    public static class Assert
    {
        public static void AreEqual(object expected, object actual)
        {
            toMSException(() => NUnitAssert.AreEqual(expected, actual));
        }

        public static void AreEqual<T>(T expected, T actual)
        {
            toMSException(() => NUnitAssert.AreEqual(expected, actual));
        }
        
        public static void AreEqual(double expected, double actual, double delta)
        {
            toMSException(() => NUnitAssert.AreEqual(expected, actual, delta));
        }
        
        public static void AreEqual(float expected, float actual, float delta)
        {
            toMSException(() => NUnitAssert.AreEqual(expected, actual, delta));
        }
        
        public static void AreEqual(object expected, object actual, string message)
        {
            toMSException(() => NUnitAssert.AreEqual(expected, actual, message));
        }
        
        public static void AreEqual(string expected, string actual, bool ignoreCase)
        {
            AreEqual(expected, actual, ignoreCase, CultureInfo.InvariantCulture);
        }
        
        public static void AreEqual<T>(T expected, T actual, string message)
        {
            toMSException(() => NUnitAssert.AreEqual(expected, actual, message));
        }
        
        public static void AreEqual(double expected, double actual, double delta, string message)
        {
            toMSException(() => NUnitAssert.AreEqual(expected, actual, delta, message));
        }
        
        public static void AreEqual(float expected, float actual, float delta, string message)
        {
            toMSException(() => NUnitAssert.AreEqual(expected, actual, delta, message));
        }
        
        public static void AreEqual(object expected, object actual, string message, params object[] parameters)
        {
            toMSException(() => NUnitAssert.AreEqual(expected, actual, message, parameters));
        }
        
        public static void AreEqual(string expected, string actual, bool ignoreCase, CultureInfo culture)
        {
            if (ignoreCase)
                toMSException(() => NUnitAssert.AreEqual(expected.ToLower(), actual.ToLower()));
            else
                toMSException(() => NUnitAssert.AreEqual(expected, actual));
        }
        
        public static void AreEqual(string expected, string actual, bool ignoreCase, string message)
        {
            if (ignoreCase)
                toMSException(() => NUnitAssert.AreEqual(expected.ToLower(), actual.ToLower(), message));
            else
                toMSException(() => NUnitAssert.AreEqual(expected, actual, message));
        }
        
        public static void AreEqual<T>(T expected, T actual, string message, params object[] parameters)
        {
            toMSException(() => NUnitAssert.AreEqual(expected, actual, message, parameters));
        }
        
        public static void AreEqual(double expected, double actual, double delta, string message, params object[] parameters)
        {
            toMSException(() => NUnitAssert.AreEqual(expected, actual, delta, message, parameters));
        }
        
        public static void AreEqual(float expected, float actual, float delta, string message, params object[] parameters)
        {
            toMSException(() => NUnitAssert.AreEqual(expected, actual, delta, message, parameters));
        }
        
        public static void AreEqual(string expected, string actual, bool ignoreCase, CultureInfo culture, string message)
        {
            AreEqual(expected, actual, ignoreCase, message);
        }
        
        public static void AreEqual(string expected, string actual, bool ignoreCase, string message, params object[] parameters)
        {
            if (ignoreCase)
                toMSException(() => NUnitAssert.AreEqual(expected.ToLower(), actual.ToLower(), message, parameters));
            else
                toMSException(() => NUnitAssert.AreEqual(expected, actual, message, parameters));
        }
        
        public static void AreEqual(string expected, string actual, bool ignoreCase, CultureInfo culture, string message, params object[] parameters)
        {
            AreEqual(expected, actual, ignoreCase, message, parameters);
        }
        
        public static void AreNotEqual(object notExpected, object actual)
        {
            toMSException(() => NUnitAssert.AreNotEqual(notExpected, actual));
        }
        
        public static void AreNotEqual<T>(T notExpected, T actual)
        {
            toMSException(() => NUnitAssert.AreNotEqual(notExpected, actual));
        }
        
        public static void AreNotEqual(double notExpected, double actual, double delta)
        {
            if (notExpected > actual && (notExpected - actual) <= delta)
                throw new AssertFailedException("Deviation is less than delta");
            if (notExpected < actual && (actual - notExpected) <= delta)
                throw new AssertFailedException("Deviation is less than delta");
        }
        
        public static void AreNotEqual(float notExpected, float actual, float delta)
        {
            AreNotEqual(notExpected, actual, delta, "", null);
        }
        
        public static void AreNotEqual(object notExpected, object actual, string message)
        {
            toMSException(() => NUnitAssert.AreNotEqual(notExpected, actual, message));
        }
        
        public static void AreNotEqual(string notExpected, string actual, bool ignoreCase)
        {
            if (ignoreCase)
                toMSException(() => NUnitAssert.AreNotEqual(notExpected.ToLower(), actual.ToLower()));
            else
                toMSException(() => NUnitAssert.AreNotEqual(notExpected, actual));
        }
        
        public static void AreNotEqual<T>(T notExpected, T actual, string message)
        {
            toMSException(() => NUnitAssert.AreNotEqual(notExpected, actual, message));
        }
        
        public static void AreNotEqual(double notExpected, double actual, double delta, string message)
        {
            AreNotEqual(notExpected, actual, delta, message, null);
        }
        
        public static void AreNotEqual(float notExpected, float actual, float delta, string message)
        {
            AreNotEqual(notExpected, actual, delta, message, null);
        }
        
        public static void AreNotEqual(object notExpected, object actual, string message, params object[] parameters)
        {
            toMSException(() => NUnitAssert.AreNotEqual(notExpected, actual, message, parameters));
        }
        
        public static void AreNotEqual(string notExpected, string actual, bool ignoreCase, CultureInfo culture)
        {
            AreNotEqual(notExpected, actual, ignoreCase);
        }
        
        public static void AreNotEqual(string notExpected, string actual, bool ignoreCase, string message)
        {
            if (ignoreCase)
                toMSException(() => NUnitAssert.AreNotEqual(notExpected.ToLower(), actual.ToLower(), message));
            else
                toMSException(() => NUnitAssert.AreNotEqual(notExpected, actual, message));
        }
        
        public static void AreNotEqual<T>(T notExpected, T actual, string message, params object[] parameters)
        {
            toMSException(() => NUnitAssert.AreNotEqual(notExpected, actual, message, parameters));
        }
        
        public static void AreNotEqual(double notExpected, double actual, double delta, string message, params object[] parameters)
        {
            var msg = "(" + message + ")";
            if (parameters != null)
                msg = string.Format(msg, parameters);
            if (notExpected > actual && (notExpected - actual) <= delta)
                throw new AssertFailedException("Deviation is less than delta" + msg);
            if (notExpected < actual && (actual - notExpected) <= delta)
                throw new AssertFailedException("Deviation is less than delta" + msg);
        }
        
        public static void AreNotEqual(float notExpected, float actual, float delta, string message, params object[] parameters)
        {
            var msg = "(" + message + ")";
            if (parameters != null)
                msg = string.Format(msg, parameters);
            if (notExpected > actual && (notExpected - actual) <= delta)
                throw new AssertFailedException("Deviation is less than delta" + msg);
            if (notExpected < actual && (actual - notExpected) <= delta)
                throw new AssertFailedException("Deviation is less than delta" + msg);
        }
        
        public static void AreNotEqual(string notExpected, string actual, bool ignoreCase, CultureInfo culture, string message)
        {
            AreNotEqual(notExpected, actual, ignoreCase, message);
        }
        
        public static void AreNotEqual(string notExpected, string actual, bool ignoreCase, string message, params object[] parameters)
        {
            if (ignoreCase)
                toMSException(() => NUnitAssert.AreNotEqual(notExpected.ToLower(), actual.ToLower(), message, parameters));
            else
                toMSException(() => NUnitAssert.AreNotEqual(notExpected, actual, message, parameters));
        }
        
        public static void AreNotEqual(string notExpected, string actual, bool ignoreCase, CultureInfo culture, string message, params object[] parameters)
        {
            AreNotEqual(notExpected, actual, ignoreCase, message, parameters);
        }
        
        public static void AreNotSame(object notExpected, object actual)
        {
            toMSException(() => NUnitAssert.AreNotSame(notExpected, actual));
        }
        
        public static void AreNotSame(object notExpected, object actual, string message)
        {
            toMSException(() => NUnitAssert.AreNotSame(notExpected, actual, message));
        }
        
        public static void AreNotSame(object notExpected, object actual, string message, params object[] parameters)
        {
            toMSException(() => NUnitAssert.AreNotSame(notExpected, actual, message, parameters));
        }
        
        public static void AreSame(object expected, object actual)
        {
            toMSException(() => NUnitAssert.AreSame(expected, actual));
        }
        
        public static void AreSame(object expected, object actual, string message)
        {
            toMSException(() => NUnitAssert.AreSame(expected, actual, message));
        }
        
        public static void AreSame(object expected, object actual, string message, params object[] parameters)
        {
            toMSException(() => NUnitAssert.AreSame(expected, actual, message, parameters));
        }
        
        public static void Fail()
        {
            toMSException(() => NUnitAssert.Fail());
        }
        
        public static void Fail(string message)
        {
            toMSException(() => NUnitAssert.Fail(message));
        }
        
        public static void Fail(string message, params object[] parameters)
        {
            toMSException(() => NUnitAssert.Fail(message, parameters));
        }
        
        public static void Inconclusive()
        {
            toMSInconclusive(() => NUnitAssert.Inconclusive());
        }
        
        public static void Inconclusive(string message)
        {
            toMSInconclusive(() => NUnitAssert.Inconclusive(message));
        }
        
        public static void Inconclusive(string message, params object[] parameters)
        {
            toMSInconclusive(() => NUnitAssert.Inconclusive(message, parameters));
        }
        
        public static void IsFalse(bool condition)
        {
            toMSException(() => NUnitAssert.IsFalse(condition));
        }
        
        public static void IsFalse(bool condition, string message)
        {
            toMSException(() => NUnitAssert.IsFalse(condition, message));
        }
        
        public static void IsFalse(bool condition, string message, params object[] parameters)
        {
            toMSException(() => NUnitAssert.IsFalse(condition, message, parameters));
        }
        
        public static void IsInstanceOfType(object value, Type expectedType)
        {
            toMSException(() => NUnitAssert.IsInstanceOf(expectedType, value));
        }
        
        public static void IsInstanceOfType(object value, Type expectedType, string message)
        {
            toMSException(() => NUnitAssert.IsInstanceOf(expectedType, value, message));
        }
        
        public static void IsInstanceOfType(object value, Type expectedType, string message, params object[] parameters)
        {
            toMSException(() => NUnitAssert.IsInstanceOf(expectedType, value, message, parameters));
        }
        
        public static void IsNotInstanceOfType(object value, Type wrongType)
        {
            toMSException(() => NUnitAssert.IsNotInstanceOf(wrongType, value));
        }
        
        public static void IsNotInstanceOfType(object value, Type wrongType, string message)
        {
            toMSException(() => NUnitAssert.IsNotInstanceOf(wrongType, value, message));
        }
        
        public static void IsNotInstanceOfType(object value, Type wrongType, string message, params object[] parameters)
        {
            toMSException(() => NUnitAssert.IsNotInstanceOf(wrongType, value, message, parameters));
        }
        
        public static void IsNotNull(object value)
        {
            toMSException(() => NUnitAssert.IsNotNull(value));
        }
        
        public static void IsNotNull(object value, string message)
        {
            toMSException(() => NUnitAssert.IsNotNull(value, message));
        }
        
        public static void IsNotNull(object value, string message, params object[] parameters)
        {
            toMSException(() => NUnitAssert.IsNotNull(value, message, parameters));
        }
        
        public static void IsNull(object value)
        {
            toMSException(() => NUnitAssert.IsNull(value));
        }
        
        public static void IsNull(object value, string message)
        {
            toMSException(() => NUnitAssert.IsNull(value, message));
        }
        
        public static void IsNull(object value, string message, params object[] parameters)
        {
            toMSException(() => NUnitAssert.IsNull(value, message, parameters));
        }
        
        public static void IsTrue(bool condition)
        {
            toMSException(() => NUnitAssert.IsTrue(condition));
        }
        
        public static void IsTrue(bool condition, string message)
        {
            toMSException(() => NUnitAssert.IsTrue(condition, message));
        }
        
        public static void IsTrue(bool condition, string message, params object[] parameters)
        {
            toMSException(() => NUnitAssert.IsTrue(condition, message, parameters));
        }
        
        public static string ReplaceNullChars(string input)
        {
            return input.Replace("\0", "\\0");
        }

        private static void toMSException(Action assert)
        {
            try
            {
                assert.Invoke();
            }
            catch (Exception ex)
            {
                throw new AssertFailedException(ex.Message);
            }
        }

        private static void toMSInconclusive(Action assert)
        {
            try
            {
                assert.Invoke();
            }
            catch (Exception ex)
            {
                throw new AssertInconclusiveException(ex.Message);
            }
        }
    }
}