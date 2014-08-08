using System;
using System.Collections;
using System.Linq;

namespace PowerAssert.Infrastructure.Nodes
{
    internal abstract class Node
    {
        internal abstract void Walk(NodeWalker walker, int depth);
        internal delegate void NodeWalker(string text, string value = null, int depth = 0);

        public override bool Equals(object obj)
        {
            if(obj.GetType() != GetType())
            {
                return false;
            }

            var allPropertiesMatch = from info in GetType().GetProperties()
                                     let mine = info.GetValue(this, null)
                                     let theirs = info.GetValue(obj, null)
                                     select ObjectsOrEnumerablesEqual(mine, theirs);

            return allPropertiesMatch.All(b => b);
        }

        static bool ObjectsOrEnumerablesEqual(object mine, object theirs)
        {
            if(mine == theirs)
            {
                return true;
            }
            if(mine == null || theirs == null)
            {
                return false;
            }
            return mine is IEnumerable ? ((IEnumerable) mine).Cast<object>().SequenceEqual(((IEnumerable) theirs).Cast<object>()) : mine.Equals(theirs);
        }

        public override int GetHashCode()
        {
            var v = from info in GetType().GetProperties()
                    let value = info.GetValue(this, null)
                    select value == null ? 0 : value.GetHashCode();

            return v.Aggregate((x, y) => x ^ y * 397);
        }

        public override string ToString()
        {
            var strings = NodeFormatter.Format(this);
            return string.Join(Environment.NewLine, strings);
        }
    }
}