using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Simple.Testing.Framework
{
    public delegate void WhenAction<in T>(T item);
    public delegate bool Expectation<in T>(T obj);

    public interface TypedSpecification<T> : Specification
    {
        Action GetBefore();
        Delegate GetOn();
        Delegate GetWhen();
        IEnumerable<Expression<Func<T, bool>>> GetAssertions();
        Action GetFinally();
    }

    public interface Specification
    {
        string GetName();
    }

    public class TransformedSpecification<TGiven, TWhen, TResult> : TypedSpecification<TResult>
    {
        public Action Before;
        public Func<TGiven> On;
        public Func<TGiven, TWhen> When;
        public Func<TWhen, TResult> AndTransformedBy;
        public List<Expression<Func<TResult, bool>>> Expect = new List<Expression<Func<TResult, bool>>>();
        public Action Finally;
        public string Name;

        public Action GetBefore() { return Before; }
        public Delegate GetOn() { return On; }
        public Delegate GetWhen() { return When; }
        public IEnumerable<Expression<Func<TResult, bool>>> GetAssertions() { return Expect; }
        public Action GetFinally() { return Finally; }
        public string GetName() { return Name; }
    }

    public class QuerySpecification<TGiven, TResult> : TypedSpecification<TResult>
    {
        public Action Before;
        public Func<TGiven> On;
        public Func<TGiven, TResult> When;
        public List<Expression<Func<TResult, bool>>> Expect = new List<Expression<Func<TResult, bool>>>();
        public Action Finally;
        public string Name;

        public Action GetBefore() { return Before; }
        public Delegate GetOn() { return On; }
        public Delegate GetWhen() { return When; }
        public IEnumerable<Expression<Func<TResult, bool>>> GetAssertions() { return Expect; }
        public Action GetFinally() { return Finally; }
        public string GetName() { return Name; }
    }

    public class ConstructorSpecification<TObject> : TypedSpecification<TObject>
    {
        public Action Before;
        public Func<TObject> When;
        public List<Expression<Func<TObject, bool>>> Expect = new List<Expression<Func<TObject, bool>>>();
        public Action Finally;
        public string Name;

        public Action GetBefore() { return Before; }
        public Delegate GetOn() {return (Action) (() => {});}
        public Delegate GetWhen() { return When; }
        public IEnumerable<Expression<Func<TObject, bool>>> GetAssertions() { return Expect; }
        public Action GetFinally() { return Finally; }
        public string GetName() { return Name; }
    }

    public class ActionSpecification<TSut> : TypedSpecification<TSut>
    {
        public Action Before;
        public Func<TSut> On;
        public Action<TSut> When;
        public List<Expression<Func<TSut, bool>>> Expect = new List<Expression<Func<TSut, bool>>>();
        public Action Finally;
        public string Name;

        public Action GetBefore() { return Before; }
        public Delegate GetOn() { return On; }
        public Delegate GetWhen() { return When; }
        public IEnumerable<Expression<Func<TSut, bool>>> GetAssertions() { return Expect; }
        public Action GetFinally() { return Finally; }
        public string GetName() { return Name; }
    }

    public class FailingSpecification<TSut, TException> : TypedSpecification<TException> where TException : Exception
    {
        public Action Before;
        public Func<TSut> On;
        public WhenAction<TSut> When;
        public List<Expression<Func<TException, bool>>> Expect = new List<Expression<Func<TException, bool>>>();
        public Action Finally;
        public string Name;

        public Action GetBefore() { return Before; }
        public Delegate GetOn() { return On; }
        public Delegate GetWhen()
        {
            return (Func<TSut, TException>) (x =>
                                                    {
                                                        try
                                                        {
                                                            When(x);
                                                        }
                                                        catch (TException ex)
                                                        {
                                                            return ex;
                                                        }
                                                        return null;
                                                    });
        }


        public IEnumerable<Expression<Func<TException, bool>>> GetAssertions() { return Expect; }
        public Action GetFinally() { return Finally; }
        public string GetName() { return Name; }
    }
}
