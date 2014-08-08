using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AutoTest.Profiler
{
    public class BinaryFileProfilerDataParser : IProfilerDataParser
    {
        public IEnumerable<ProfilerEntry> Parse(Stream stream)
        {
            var hash = new Dictionary<int, Stack<ProfilerEntry>>();
            using(var reader = new BinaryReader(stream))
            {
                while (reader.PeekChar() != -1)
                {
                    var entry = new ProfilerEntry();
                    entry.Type = (ProfileType) reader.ReadByte();
                    entry.Sequence = reader.ReadInt32();
                    entry.Time = reader.ReadDouble(); 
                    entry.Thread = reader.ReadInt32();
                    entry.Functionid = reader.ReadInt64();

                    Stack<ProfilerEntry> stack;
                    if (entry.Type == ProfileType.Enter)
                    {
                        var length = reader.ReadInt32();
                        entry.Method = string.Intern(Encoding.Unicode.GetString(reader.ReadBytes(length * 2)).Replace(", ", ","));
                        length = reader.ReadInt32();
                        entry.Runtime = string.Intern(Hack.Generics(Encoding.Unicode.GetString(reader.ReadBytes(length * 2)).Replace(", ", ",")));
                        if(!hash.TryGetValue(entry.Thread, out stack))
                        {
                            stack = new Stack<ProfilerEntry>();
                            hash.Add(entry.Thread, stack);
                        }
                        stack.Push(entry);
                    } else if(entry.Type == ProfileType.Leave)
                    {
                        if (!hash.TryGetValue(entry.Thread, out stack)) throw new ProfilerStackUnderFlowException();
                        var previous = GetCurrentStackPosition(stack, entry);
                        entry.Method = previous.Method;
                        entry.Runtime = previous.Runtime;
                    } else
                    {
                        //TODO GREG HANDLE TAIL CALLS
                        //if (!hash.TryGetValue(entry.Thread, out stack)) throw new ProfilerStackUnderFlowException();
                        //var current = stack.Peek();
                        //entry.Method = current.Method;
                        //entry.Runtime = current.Runtime;
                    }
                    yield return entry;
                }
            }
            //this maybe should be here? TODO:GREG
            //if (hash.Values.Any(item => item.Count != 0))
            //{
            //    throw new ProfilerMissingLeavesException();
            //}
        }


        private ProfilerEntry GetCurrentStackPosition(Stack<ProfilerEntry> stack, ProfilerEntry entry)
        {
            while (stack.Count != 0)
            {
                var previous = stack.Pop();
                if (previous.Functionid == entry.Functionid)
                    return previous;
            }
            throw new ProfilerStackUnderFlowException();
        }
    }

    public static class Hack
    {

        public static string HackName(string methodName)
        {
            try
            {
                int paren = methodName.IndexOf('(');
                int firstspace = methodName.IndexOf(' ');
                string hacked = methodName.Replace(".ctor", "");
                int lastdot = hacked.LastIndexOf('.');
                string beforelastdot = methodName.Substring(0, lastdot);
                string afterlastdotbeforeparen = "";
                string afterlastparen = "";
                if (paren > 0)
                {
                    afterlastdotbeforeparen = methodName.Substring(lastdot + 1, paren - lastdot - 1);
                    afterlastparen = methodName.Substring(paren, methodName.Length - paren);
                }
                else
                {
                    afterlastdotbeforeparen = methodName.Substring(lastdot + 1, methodName.Length - lastdot - 1);
                }
                var returntype = "";
                string args;
                if (firstspace >= paren || firstspace == -1)
                {
                    returntype = "System.Void ";
                }
                beforelastdot = beforelastdot.Replace(returntype, "");
                args = paren == -1 ? "()" : afterlastparen;
                return returntype + beforelastdot + "::" + afterlastdotbeforeparen + args;
            }
            catch(Exception)
            {
                return methodName;
            }
        }

        public static string Generics(string s)
        {
            try
            {
                var ret = s;
                var firstspace = s.IndexOf(' ');
                if (firstspace < 0) return s;
                var idx = s.IndexOf("::");
                if (idx < 0) return s;
                if (idx - firstspace < 0) return s; //dunno why this would be the case
                var open = s.IndexOf('<', firstspace, idx - firstspace);
                var close = s.IndexOf('>', firstspace, idx - firstspace);
                if (open > 0 && close > 0 && close > open)
                {
                    ret = s.Substring(0, open) + s.Substring(close + 1, s.Length - close - 1);
                }
                idx = ret.IndexOf("::");
                var firstparen = ret.IndexOf('(');
                if (firstparen > 0 || firstparen > idx)
                {
                    var methodargopen = ret.IndexOf('<', idx, firstparen - idx);
                    var methodargclose = ret.IndexOf('>', idx, firstparen - idx);
                    if (methodargopen > 0 && methodargclose > 0 && methodargclose > methodargopen && methodargopen != idx+2)
                    {
                        ret = ret.Substring(0, methodargopen) +
                              ret.Substring(methodargclose + 1, ret.Length - methodargclose - 1);
                    }
                }
                return ret;
            }catch(Exception ex)
            {
                throw new ArgumentException("name", "'" + s + "' had exception", ex);
            }
        }
    }
}
