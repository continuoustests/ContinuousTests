using System;
using Simple.Testing.Framework;

namespace Simple.Testing.Example
{
    /*
     * Sometimes the point of a specification is to show that a given scenario fails.
     * 
     * A failing specification is similar to a SUT specification. It will return the SUT
     * from its On() method. The SUT will then be passed to the when() method where some 
     * action will happen. It is expected that this action will throw an exception.
     * 
     * The exception is then passed to the expectations.
     * 
     * note: if you are doing more complex operations other templates can be used with 
     * the TestContext pattern.
     */
    public class FailingSpecification
    {
        public Specification it_will_fail = new FailingSpecification<FailingExample, SomethingFailedException>
                                                {
                On = () => new FailingExample(),
                When = (obj) => obj.CauseFailure(),
                Expect =
                {
                    exception => exception.Message == "Something failed!",
                    exception => exception.ErrorCode == 17
                },
        };
    }

    public class FailingExample
    {
        public void CauseFailure()
        {
            throw new SomethingFailedException("Something failed!", 17);
        }
    }

    public class SomethingFailedException : Exception
    {
        public readonly int ErrorCode;
        public SomethingFailedException(string msg, int errorCode) : base(msg)
        {
            ErrorCode = errorCode;
        }
    }
}
