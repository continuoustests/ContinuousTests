using System;
using System.Collections.Generic;
using AutoTest.Minimizer.Extensions;
using Mono.Cecil;

namespace AutoTest.Minimizer
{
    public class GenericContext
    {
        public void SetIndirectConstraintsOn(MemberReference reference, MemberReference parent, bool inInheritance)
        {
            if (reference == null) return;
            if (parent == null) return;
            //TODO handle inInheritance call
            var def = reference.DeclaringType.ThreadSafeResolve();
            if (def != null && def.IsInterface)
            {
                //coming back to an interface
                var mdp = ((MethodReference)parent).ThreadSafeResolve();
                var mdr = ((MethodReference)reference).ThreadSafeResolve();
                if (mdp == null || mdr == null) return;
                if (mdp.Parameters.Count != mdr.Parameters.Count) Console.WriteLine("not sure what to do?!");
                for (var i = 0; i < mdp.Parameters.Count; i++)
                {
                    var current = mdr.Parameters[i].GetTypeWithGenericResolve();
                    if (current == null) continue; ;
                    if (current.IsGenericParameter)
                    {
                        var par = mdp.Parameters[i].GetTypeWithGenericResolve();
                        if (par == null) continue;
                        if(!_limitations.ContainsKey(current.Name))
                        {
                            _limitations.Add(current.Name,
                                             par.IsGenericParameter
                                                 ? new GenericEntry(null, true, par.Name)
                                                 : new GenericEntry(par.ThreadSafeResolve(), false, null));
                        }
                    }
                }
            }
        }

        public int LimitationCount
        {
            get { return _limitations.Count; }
        }

        private readonly Dictionary<string, GenericEntry> _limitations;
        public GenericContext(Dictionary<string,GenericEntry> initialContext)
        {
            _limitations = initialContext;
        }

        public GenericContext() : this(new Dictionary<string, GenericEntry>())
        {
        }

        public Dictionary<string, GenericEntry> GetGenericContextOf(MemberReference memberReference)
        {
            var ret = new Dictionary<string, GenericEntry>();
            GetMethodGenericArgumentsOn(memberReference, ret);
            GetClassGenericArgumentsOn(memberReference, ret);
            return ret;
        }

        private static void GetClassGenericArgumentsOn(MemberReference memberReference, Dictionary<string, GenericEntry> ret)
        {
                if (memberReference == null) return;
                var instance = memberReference.DeclaringType as GenericInstanceType;
                if (instance != null)
                {
                    var resolved = instance.ThreadSafeResolve();
                    if (resolved == null) return;
                    for (int i = 0; i < resolved.GenericParameters.Count; i++)
                    {
                        var current = resolved.GenericParameters[i];
                        var instanceParam = instance.GenericArguments[i];
                        if(instanceParam.IsGenericParameter)
                        {
                            ret.Add(current.Name, new GenericEntry(null, true, instanceParam.Name));
                        } else
                        {
                            ret.Add(current.Name, new GenericEntry(instanceParam.ThreadSafeResolve(), false, ""));
                        }
                    }
                }

        }

        public void GetMethodGenericArgumentsOn(MemberReference memberReference, Dictionary<string, GenericEntry> ret)
        {
            var genericInstanceMethod = memberReference as GenericInstanceMethod;
            
            if (genericInstanceMethod != null)
            {
                var elementMethod = genericInstanceMethod.ElementMethod;
                for (int i = 0; i < elementMethod.GenericParameters.Count; i++)
                {
                    var param = elementMethod.GenericParameters[i];
                    if (!genericInstanceMethod.GenericArguments[i].IsGenericParameter)
                    {
                        ret.Add(param.Name, new GenericEntry(genericInstanceMethod.GenericArguments[i].ThreadSafeResolve(), false, ""));
                    } else
                    {
                        ret.Add(param.Name, new GenericEntry(null, true, genericInstanceMethod.GenericArguments[i].Name));
                    }
                }
            }
        }

        public GenericContext TransitionTo(Dictionary<string,GenericEntry> context2)
        {
            var newLimitations = new Dictionary<string, GenericEntry>();
            foreach(var item in context2)
            {
                if(item.Value.IsGeneric)
                {
                    GenericEntry entry;
                    string key;
                    if (_limitations.ContainsKey(item.Key))
                    {
                        entry = _limitations[item.Key];
                    }
                    else
                    {
                        entry = item.Value;
                    }
                    newLimitations.Add(item.Value.Name, entry);
                }
                else
                {
                    newLimitations.Add(item.Key, item.Value);                    
                }
            }
            return new GenericContext(newLimitations);
        }

        public GenericEntry this[string name]
        {
            get { return _limitations[name]; }
        }

        public bool CanTransitionTo(Dictionary<string,GenericEntry> context2)
        {
            foreach(var param in context2)
            {
                if (param.Value.IsGeneric) continue;
                if(_limitations.ContainsKey(param.Key))
                {
                    var entry = _limitations[param.Key];
                    if (entry.IsGeneric) continue;
                    if(!param.Value.Type.IsBaseTypeOf(entry.Type))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static GenericContext Empty()
        {
            return new GenericContext();
        }

        public bool IsClear()
        {
            return _limitations.Count == 0;
        }
    }

    public class GenericEntry
    {
        public readonly TypeDefinition Type;
        public readonly bool IsGeneric;
        public readonly string Name;

        public GenericEntry(TypeDefinition type, bool isGeneric, string name)
        {
            Type = type;
            Name = name;
            IsGeneric = isGeneric;
        }
    }
}