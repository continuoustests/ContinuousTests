using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace celer.Core.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> Combined<T>(this IEnumerable<IEnumerable<T>> list)
        {
            return list.SelectMany(enumerable => enumerable);
        }

        public static IEnumerable<TVReturn> ForkAndJoinTo<T, TVReturn>(this IEnumerable<T> input, int count, Func<T, TVReturn> func)
        {
            var results = new List<TVReturn>[count];
            var resetEvents = new ManualResetEvent[count];
            var worksets = input.SplitTo(count);
            var exceptions = new Exception[count];
            exceptions.Initialize();
            for (var i = 0; i < count; i++)
            {
                resetEvents[i] = new ManualResetEvent(false);
                var current = i;
                results[current] = new List<TVReturn>();
                ThreadPool.QueueUserWorkItem(x =>
                {
                    try
                    {
                        var items = (IEnumerable<T>)x;
                        foreach (var c in items) results[current].Add(func(c));

                    }
                    catch (Exception ex)
                    {
                        exceptions[current] = ex;
                    }
                    finally
                    {
                        resetEvents[current].Set();
                    }
                }, worksets[current]);
            }
            WaitHandle.WaitAll(resetEvents);
            foreach (var e in exceptions.Where(e => e != null)) throw e;
            return results.SelectMany(cur => cur).ToList();
        }

        public static IEnumerable<T>[] AsMultiple<T>(this IEnumerable<T> input, int count)
        {
            //TODO make this work so everything can be multi-threaded
            var ret = new IEnumerable<T>[count];
            var lockedReader = new LockedReadFrom<T>(input);
            for(var i=0;i<count;i++)
            {
                ret[i] = lockedReader.GetReader();
            }
            return ret;
        }

        public static IEnumerable<T>[] SplitTo<T>(this IEnumerable<T> input, int count)
        {
            var ret = new IEnumerable<T>[count];
            int current = 0;
            for (var i = 0; i < count; i++)
            {
                ret[i] = new List<T>();
            }
            foreach (var i in input)
            {
                ((List<T>)ret[current % count]).Add(i);
                current++;
            }
            return ret;
        }
    }

    public class LockedReadFrom<T>
    {
        private readonly IEnumerator<T> _enumerator;

        public IEnumerable<T> GetReader()
        {
            lock(_enumerator)
            {
                if(!_enumerator.MoveNext())
                {
                    yield break;
                }
                yield return _enumerator.Current;
            }
        }

        public LockedReadFrom(IEnumerable<T> input)
        {
            _enumerator = input.GetEnumerator();
        }
    }
}