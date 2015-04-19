using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.TestRunners.Shared.AssemblyAnalysis
{
    [Serializable]
    public class SimpleClass : SimpleType
    {
        public bool IsAbstract { get; private set; }
        public IEnumerable<SimpleField> Fields { get; private set; }
        public IEnumerable<SimpleMethod> Methods { get; private set; }

        public SimpleClass(string fullname, IEnumerable<string> attributes, IEnumerable<SimpleField> fields, IEnumerable<SimpleMethod> methods, bool isAbstract)
            : base(fullname, attributes)
        {
            IsAbstract = isAbstract;
            Fields = fields;
            Methods = methods;
        }
    }

    [Serializable]
    public class SimpleMethod : SimpleType
    {
        public bool IsAbstract { get; private set; }
        public string ReturnType { get; private set; }
        public SimpleMethod(string fullname, IEnumerable<string> attributes, bool isAbstract, string retType)
            : base(fullname, attributes)
        {
            ReturnType = retType;
            IsAbstract = isAbstract;
        }
    }

    [Serializable]
    public class SimpleField : SimpleType
    {
        public string FieldType { get; private set; }

        public SimpleField(string fullname, IEnumerable<string> attributes, string fieldType)
            : base(fullname, attributes)
        {
            FieldType = fieldType;
        }
    }

    [Serializable]
    public class SimpleType
    {
        public string Fullname { get; private set; }
        public IEnumerable<string> Attributes { get; private set; }

        public SimpleType(string fullname, IEnumerable<string> attributes)
        {
            Fullname = fullname;
            Attributes = attributes;
        }
    }

    [Serializable]
    public class TypeName
    {
        public string FullName { get; private set; }
        public string Name { get; private set; }

        public TypeName(string fullname, string name)
        {
            FullName = fullname;
            Name = name;
        }
    }
}
