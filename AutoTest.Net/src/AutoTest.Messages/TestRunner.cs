using System;

namespace AutoTest.Messages
{
    public enum TestRunner
    {
        Any = 0,
        NUnit = 1,
        MSTest = 2,
        XUnit = 3,
        MSpec = 4,
        MbUnit = 5,
        SimpleTesting = 6,
        PhpUnit = 7,
        PhpParseError = 8
    }

    public static class TestRunnerConverter
    {
        public static string ToString(TestRunner runner)
        {
            return runner.ToString();
        }

        public static TestRunner FromString(string runner)
        {
            try
            {
                if (runner.ToLower().StartsWith("mspec"))
                    return TestRunner.MSpec;
                return (TestRunner)Enum.Parse(typeof(TestRunner), runner, true);
            }
            catch
            {
                return TestRunner.Any;
            }
        }
    }
}
