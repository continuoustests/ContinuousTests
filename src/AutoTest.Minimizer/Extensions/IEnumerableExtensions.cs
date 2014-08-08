using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AutoTest.Minimizer.Extensions
{
    internal static class EnumerableExtensions
    {
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
                                                         var items = (IEnumerable<T>) x;
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
            foreach (var e in exceptions.Where(e => e != null)) throw new BackGroundProcessException(e);
            return results.SelectMany(cur => cur).ToList();
        }

        public static IEnumerable<T>[] SplitTo<T>(this IEnumerable<T> input, int count)
        {
            var ret = new IEnumerable<T>[count];
            int current = 0;
            for (var i = 0; i < count;i++)
            {
                ret[i] = new List<T>();
            }
            foreach (var i in input)
            {
                ((List<T>) ret[current % count]).Add(i);
                current++;
            }
            return ret;
        }

        public static IEnumerable<T> CombineUniques<T>(this IEnumerable<IEnumerable<T>> items)
        {
            var ret = new List<T>();
            foreach(var c in items)
            {
                ret.AddNotExistRange(c);
            }
            return ret;
        }

        public static Dictionary<V, T> ToSafeDictionary<T, V>(this IEnumerable<T> list, Func<T,V> key)
        {
            var ret = new Dictionary<V,T>();
            foreach(var item in list)
            {
                var k = key(item);
                if(!ret.ContainsKey(k))
                    ret.Add(k, item);
            }
            return ret;
        }

        /// <summary>
        /// Enumerates through each item in a list in parallel
        /// </summary>
        public static void EachParallel<T>(this IEnumerable<T> list, Action<T> action)
        {
            // enumerate the list so it can't change during execution
            list = list.ToArray();
            var count = list.Count();

            if (count == 0)
            {
                return;
            }
            else if (count == 1)
            {
                // if there's only one element, just execute it
                action(list.First());
            }
            else
            {
                // Launch each method in it's own thread
                const int MaxHandles = 64;
                for (var offset = 0; offset <= list.Count() / MaxHandles; offset++)
                {
                    // break up the list into 64-item chunks because of a limitiation 
                    // in WaitHandle
                    var chunk = list.Skip(offset * MaxHandles).Take(MaxHandles);

                    // Initialize the reset events to keep track of completed threads
                    var resetEvents = new ManualResetEvent[chunk.Count()];

                    // spawn a thread for each item in the chunk
                    int i = 0;
                    foreach (var item in chunk)
                    {
                        resetEvents[i] = new ManualResetEvent(false);
                        ThreadPool.QueueUserWorkItem(new WaitCallback((object data) =>
                                                                          {
                                                                              int methodIndex = (int)((object[])data)[0];

                                                                              // Execute the method and pass in the enumerated item
                                                                              action((T)((object[])data)[1]);

                                                                              // Tell the calling thread that we're done
                                                                              resetEvents[methodIndex].Set();
                                                                          }), new object[] { i, item });
                        i++;
                    }

                    // Wait for all threads to execute
                    WaitHandle.WaitAll(resetEvents);
                }
            }
        }
    }
}