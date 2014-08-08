using System;
using System.Linq;
using System.Reflection;
using PowerAssert;

namespace Simple.Testing.Framework
{
    public class SpecificationRunner
    {
        public RunResult RunSpecifciation(SpecificationToRun spec)
        {
            var method = typeof(SpecificationRunner).GetMethod("Run", BindingFlags.NonPublic | BindingFlags.Instance);
            var tomake = spec.Specification.GetType().GetInterfaces().Single(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(TypedSpecification<>));
            var generic = method.MakeGenericMethod(tomake.GetGenericArguments()[0]);
            var result = (RunResult) generic.Invoke(this, new [] {spec.Specification});
            result.FoundOnMemberInfo = spec.FoundOn;
            return result;
        }

        private RunResult Run<T>(TypedSpecification<T> spec)
        {
            var result = new RunResult { SpecificationName = spec.GetName()};
            try
            {
                var before = spec.GetBefore();
                before.InvokeIfNotNull();
            }
            catch(Exception ex)
            {
                result.MarkFailure("Before Failed", ex.InnerException);
                return result;
            }
            object sut = null;
            try
            {
                var given = spec.GetOn();
                sut = given.DynamicInvoke();
                result.On = given;
            }
            catch(Exception ex)
            {
                result.MarkFailure("On Failed", ex.InnerException);
            }
            object whenResult = null;
            Delegate when;
            try
            {
                when = spec.GetWhen();
                if (when == null) return new RunResult { SpecificationName = spec.GetName(), Passed = false, Message = "No when on specification" };
                if(when.Method.GetParameters().Length == 1)
                    whenResult = when.DynamicInvoke(new[] {sut});
                else
                    whenResult = when.DynamicInvoke();
                if (when.Method.ReturnType != typeof(void))
                    result.Result = whenResult;
                else
                    result.Result = sut;
            }
            catch(Exception ex)
            {
                result.MarkFailure("When Failed", ex.InnerException);
                return result;
            }
            var fromWhen = when.Method.ReturnType == typeof(void) ? sut : whenResult;
            bool allOk = true;
            foreach(var exp in spec.GetAssertions())
            {
                var partiallyApplied = PartialApplicationVisitor.Apply(exp, fromWhen);
                try
                {
                    PAssert.IsTrue(partiallyApplied);
                    result.Expectations.Add(new ExpectationResult { Passed = true, Text = PAssert.CreateSimpleFormatFor(partiallyApplied), OriginalExpression = exp});
                }
                catch(Exception ex)
                {
                    allOk = false;
                    result.Expectations.Add(new ExpectationResult { Passed = false, Text = PAssert.CreateSimpleFormatFor(partiallyApplied), OriginalExpression = exp, Exception = ex });
                }
            }
            try
            {
                var Finally = spec.GetFinally();
                Finally.InvokeIfNotNull();
            }
            catch (Exception ex)
            {
                allOk = false;
                result.Message = "Finally failed";
                result.Thrown = ex.InnerException;
            }
            result.Passed = allOk;
            return result;
        }
    }

}
