using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Collections.Generic;

namespace AutoTest.UI.CodeReflection
{
    public class TypeConverter : IDisposable
    {
        private readonly AssemblyDefinition _assembly = null;

        public TypeConverter(string assembly)
        {
            _assembly = AssemblyDefinition.ReadAssembly(assembly);
        }

        public Signature ToSignature(string type)
        {
            Signature signature = null;
            foreach (var m in _assembly.Modules)
            {
                signature = getSignature(m.GetTypes(), type);
                if (signature != null) break;
            }
            return signature;
        }

        private Signature getSignature(IEnumerable<TypeDefinition> types, string type)
        {
            Signature signature = null;
            foreach (var t in types)
            {
                if (t.FullName.Equals(type))
                    signature = new Signature(SignatureTypes.Class, t.FullName);
                else if (t.HasNestedTypes)
                    signature = getSignature(t.NestedTypes, type);
                else if (t.HasMethods)
                    signature = getMethods(t.FullName, t.Methods, type);
                else if (t.HasFields)
                    signature = getFields(t.FullName, t.Fields, type);

                if (signature != null) break;
            }
            return signature;
        }

        private Signature getMethods(string parent, IEnumerable<MethodDefinition> methods, string type)
        {
            var method = methods.FirstOrDefault(x => type.Equals(string.Format("{0}.{1}", parent, x.Name)));
            if (method == null)
                return null;
            return new Signature(SignatureTypes.Method, method.FullName);
        }

        private Signature getFields(string parent, IEnumerable<FieldDefinition> fields, string type)
        {
            var field = fields.FirstOrDefault(x => type.Equals(string.Format("{0}.{1}", parent, x.Name)));
            if (field == null)
                return null;
            return new Signature(SignatureTypes.Field, field.FullName);
        }

        public void Dispose()
        {
            if (_assembly != null)
                _assembly.Dispose();
        }
    }
}
