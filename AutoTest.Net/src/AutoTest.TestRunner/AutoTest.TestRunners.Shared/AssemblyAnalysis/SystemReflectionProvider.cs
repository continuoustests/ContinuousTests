using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoTest.TestRunners.Shared.Plugins;
using System.IO;
using AutoTest.TestRunners.Shared.Targeting;

namespace AutoTest.TestRunners.Shared.AssemblyAnalysis
{
    public class SystemReflectionProvider : IReflectionProvider
    {
        private readonly AppDomain _childDomain;
        private ISystemReflectionProvider_Internal _locator;

        public SystemReflectionProvider(string assembly)
        {
            try
            {
                var domainSetup = new AppDomainSetup
                                      {
                    ApplicationBase = Path.GetDirectoryName(assembly),
                    ApplicationName = AppDomain.CurrentDomain.SetupInformation.ApplicationName,
                    LoaderOptimization = LoaderOptimization.MultiDomainHost
                };
                _childDomain = AppDomain.CreateDomain("Type locator app domain", null, domainSetup);

                // Create an instance of the runtime in the second AppDomain. 
                // A proxy to the object is returned.
                _locator = (ISystemReflectionProvider_Internal)_childDomain.CreateInstanceFromAndUnwrap(Assembly.GetExecutingAssembly().Location, typeof(SystemReflectionProvider_Internal).FullName);
                _locator.Load(assembly);
            }
            catch (Exception ex)
            {
                Logging.Logger.Write(ex);
            }
        }

        public string GetName()
        {
            return _locator.GetName();
        }

        public Version GetTargetFramework()
        {
            return new Version(_locator.GetTargetFramework());
        }

        public Platform GetPlatform()
        {
            return _locator.GetPlatform();
        }

        public IEnumerable<TypeName> GetReferences()
        {
            return _locator.GetReferences();
        }

        public string GetParentType(string type)
        {
            return _locator.GetParentType(type);
        }

        public SimpleClass LocateClass(string type)
        {
            return _locator.LocateClass(type);
        }

        public SimpleMethod LocateMethod(string type)
        {
            return _locator.LocateMethod(type);
        }

        public void Dispose()
        {
            _locator = null;
            if (_childDomain != null)
                AppDomain.Unload(_childDomain);
        }
    }

    public interface ISystemReflectionProvider_Internal
    {
        void Load(string assembly);
        string GetName();
        string GetTargetFramework();
        Platform GetPlatform();
        List<TypeName> GetReferences();
        string GetParentType(string type);
        SimpleClass LocateClass(string type);
        SimpleMethod LocateMethod(string type);
    }

    public class SystemReflectionProvider_Internal : MarshalByRefObject, ISystemReflectionProvider_Internal
    {
        private Assembly _assembly;

        public void Load(string assembly)
        {
            if (!File.Exists(assembly))
                return;

            var hitPaths = new[]
                                {
                                    Path.GetDirectoryName(assembly),
                                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                                };
            //TODO GFY WTF why do we not use the resolver?
            // The only reason we pull a resolver out is so that we
            // can have a using statement
            // The assembly resovler binds up the AppDomain resolve
            // event. We need to manually handle dependency resolves
            // as the dll are in various different directories.
            // Because of the using it will unbind the event.
            using (var resolver = new AssemblyResolver(hitPaths))
            {
                try
                {
                    _assembly = Assembly.LoadFrom(assembly);
                }
                catch (Exception ex)
                {
                    Logging.Logger.Write(ex);
                }
            }
        }

        public string GetName()
        {
            return _assembly.FullName;
        }

        public string GetTargetFramework()
        {
            try
            {
                var runtime = _assembly.ImageRuntimeVersion.Replace("v", "");
                var ver = new Version(runtime);
                return new Version(ver.Major, ver.Minor).ToString();
            }
            catch
            {
                return new Version().ToString();
            }
        }

        public Platform GetPlatform()
        {
            try
            {
                var architecture = _assembly.GetName().ProcessorArchitecture;
                if (architecture == ProcessorArchitecture.X86)
                    return Platform.x86;
                if (architecture == ProcessorArchitecture.Amd64)
                    return Platform.AnyCPU;
                if (architecture == ProcessorArchitecture.IA64)
                    return Platform.AnyCPU;
                if (architecture == ProcessorArchitecture.MSIL)
                    return Platform.x86;
                return Platform.Unknown;
            }
            catch
            {
                return Platform.Unknown;
            }
        }

        public List<TypeName> GetReferences()
        {
            try
            {
                var references = _assembly.GetReferencedAssemblies();
                var names = new List<TypeName>();
                foreach (var reference in references)
                    names.Add(new TypeName(reference.FullName, reference.Name));
                return names;
            }
            catch
            {
            }
            return new TypeName[] { }.ToList();
        }

        public string GetParentType(string type)
        {
            var end = type.LastIndexOf('.');
            if (end == -1)
                return null;
            return type.Substring(0, end);
        }

        public SimpleClass LocateClass(string type)
        {
            var cls = locate(type);
            if (cls == null)
                return null;
            if (cls.GetType() == typeof(SimpleClass))
                return (SimpleClass)cls;
            return null;
        }

        public SimpleMethod LocateMethod(string type)
        {
            var method = locate(type);
            if (method == null)
                return null;
            if (method.GetType() == typeof(SimpleMethod))
                return (SimpleMethod)method;
            return null;
        }

        private SimpleType locate(string type)
        {
            foreach (var module in _assembly.GetModules())
            {
                var result = locateSimpleType(module.GetTypes(), type);
                if (result != null)
                    return result;
            }
            return null;
        }

        private SimpleType locateSimpleType(Type[] types, string typeName)
        {
            foreach (var type in types)
            {
                if (type.FullName.Equals(typeName))
                    return getType(type);
                var result = locateSimpleType(type.GetNestedTypes(), typeName);
                if (result != null)
                    return result;
                result = locateSimpleMethod(type.GetMethods(), typeName, type.FullName);
                if (result != null)
                    return result;
            }
            return null;
        }

        private SimpleType getType(Type type)
        {
            return new SimpleClass(
                type.FullName,
                getTypeAttributes(type),
                type.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).Select(x => new SimpleField(
                    type.FullName + "." + x.Name,
                    getAttributes(x.GetCustomAttributes(true)).ToList(),
                    x.FieldType.FullName)).ToList(),
                type.GetMethods().Select(x => new SimpleMethod(
                    type.FullName + "." + x.Name,
                    getAttributes(x.GetCustomAttributes(true)).ToList(),
                    x.IsAbstract, x.ReturnType.ToString())).ToList(),
                    type.IsAbstract);
        }

        private List<string> getTypeAttributes(Type type)
        {
            var attributes = new List<string>();
            attributes.AddRange(getAttributes(type.GetCustomAttributes(true)));
            var baseType = type.BaseType;
            if (baseType != null)
                attributes.AddRange(getTypeAttributes(baseType));
            return attributes;
        }

        private IEnumerable<string> getAttributes(object[] customAttributes)
        {
            var attributes = new List<string>();
            foreach (var attribute in customAttributes)
            {
                var type = attribute.GetType();
                attributes.Add(type.FullName);
                addBaseAttributes(attributes, type);
            }
            return attributes;
        }

        private void addBaseAttributes(List<string> attributes, Type type)
        {
            if (type == null)
                return;
            if (type.BaseType == null)
                return;
            attributes.Add(type.BaseType.FullName);
            addBaseAttributes(attributes, type.BaseType);
        }

        private SimpleType locateSimpleMethod(MethodInfo[] methods, string typeName, string typeFullname)
        {
            foreach (var method in methods)
            {
                var fullName = typeFullname + "." + method.Name;
                if (fullName.Equals(typeName))
                    return new SimpleMethod(fullName, getAttributes(method.GetCustomAttributes(true)).ToList(), method.IsAbstract, method.ReturnType.ToString());
            }
            return null;
        }
    }
}
