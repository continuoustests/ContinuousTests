using System;

namespace ContinuousTests.Handlers
{
    abstract class Handler
    {
        protected Action<string, object> _dispatch;

        public void DispatchThrough(Action<string, object> dispatcher) {
            _dispatch = dispatcher;
        }
    
        public abstract void OnMessage(object message);

        protected bool isType<T>(object o)
        {
            return o.GetType() == typeof(T);
        }
    }
}
