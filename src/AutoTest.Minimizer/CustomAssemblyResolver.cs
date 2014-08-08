using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace AutoTest.Minimizer
{
    public class CustomAssemblyResolver : BaseAssemblyResolver, IDisposable
    {
        readonly IDictionary<string, AssemblyDefinition> cache;
        private readonly List<AssemblyDefinition> resolved = new List<AssemblyDefinition>();

        public CustomAssemblyResolver()
        {
            cache = new Dictionary<string, AssemblyDefinition>();
        }

        public override AssemblyDefinition Resolve(AssemblyNameReference name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            AssemblyDefinition assembly;
            if (cache.TryGetValue(GetNameFrom(name), out assembly))
                return assembly;

            assembly = base.Resolve(name);
            cache[GetNameFrom(name)] = assembly;
            resolved.Add(assembly);
            return assembly;
        }

        public override AssemblyDefinition Resolve(string name)
        {
            AssemblyDefinition ass;
            if (cache.TryGetValue(name, out ass))
                return ass;

            var assembly = base.Resolve(name);
            if (!name.Contains(".mm") && !cache.ContainsKey(GetNameFrom(assembly.Name)))
            {
                cache[GetNameFrom(assembly.Name)] = assembly;
                resolved.Add(assembly);
                
            }
            return assembly;
        }

        public override AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
        {
            AssemblyDefinition ass;
            if (cache.TryGetValue(GetNameFrom(name), out ass))
                return ass;

            var assembly = base.Resolve(name, parameters);
            resolved.Add(assembly);
            cache[GetNameFrom(name)] = assembly;
            return assembly;
        }

        public override AssemblyDefinition Resolve(string name, ReaderParameters parameters)
        {
            AssemblyDefinition ass;
            if (cache.TryGetValue(name, out ass))
                return ass;

            var assembly = base.Resolve(name, parameters);
            if (!name.Contains(".mm") && !cache.ContainsKey(GetNameFrom(assembly.Name)))
            {
                cache[GetNameFrom(assembly.Name)] = assembly;
                resolved.Add(assembly);
                
            }
            return assembly;
        }

        protected void RegisterAssembly(AssemblyDefinition assembly)
        {
            resolved.Add(assembly);
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            var name = GetNameFrom(assembly.Name);
            if (cache.ContainsKey(name))
                return;

            cache[name] = assembly;
        }

        private string GetNameFrom(AssemblyNameReference name)
        {
            return name.Name + " " + name.Version;
        }

        public void Dispose()
        {
            resolved.ForEach(q => q.Dispose());
        }

        public string GetName()
        {
            return this.GetSearchDirectories()[0];
        }

        public IEnumerable<string> GetCachedAssemblies()
        {
            return cache.Select(item => item.Key + " " + item.Value.FullName);
        }

        public void SetNewSearchDirectoryTo(string path)
        {
            var dirs = GetSearchDirectories();
            foreach (var d in dirs)
            {
                RemoveSearchDirectory(d);
            }
            AddSearchDirectory(path);
        }
    }
}