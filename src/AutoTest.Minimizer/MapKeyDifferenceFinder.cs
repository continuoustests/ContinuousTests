using System.Collections.Generic;
using System.Linq;

namespace AutoTest.Minimizer
{

    public class MapKeyDifferenceFinder
    {
         public static List<Change<V>> GetChangesBetween<T,V>(Dictionary<T,V> oldMap, Dictionary<T,V> newMap)
         {
             var paintings = new Dictionary<T, bool>();
             var changes = new List<Change<V>>();
             foreach(var current in newMap.Keys)
             {
                 if(oldMap.ContainsKey(current))
                 {
                     paintings.Add(current, true);
                 } else
                 {
                     changes.Add(new Change<V>(ChangeType.Add, newMap[current]));
                 }
             }
             changes.AddRange(from current in oldMap.Keys
                              where !paintings.ContainsKey(current)
                              select new Change<V>(ChangeType.Remove, oldMap[current]));
             return changes;
         }
    }

    public interface ICanBePainted
    {
        void ClearPainting();
        void Paint();
        bool IsPainted { get;  }
    }
}
