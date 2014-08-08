using System;
using System.Collections.Generic;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;

namespace AutoTest.VS.Util
{
    public class MethodNameReader
    {
        public static string GetMethodStringFromElement(CodeElement elem)
        {
            try
            {
                if (elem.Kind == vsCMElement.vsCMElementFunction)
                {
                    return GetMethodStringFromElement(elem as CodeFunction);
                }
                if (elem.Kind == vsCMElement.vsCMElementProperty)
                {
                    var getter = ((CodeProperty) elem).Getter;
                    return GetMethodStringFromElement(getter);
                }
                if (elem.Kind == vsCMElement.vsCMElementVariable)
                {
                    return GetFieldStringFromElement(elem);
                }
            }
            catch(Exception ex)
            {
                Core.DebugLog.Debug.WriteDebug("Exception getting Method String : " + ex);
                return null;
            }
            return null;
        }

        private static string GetMethodStringFromElement(CodeFunction justfunction)
        {
            string all = null;
            var first = true;
            if (justfunction != null)
            {
                if (justfunction.FunctionKind == vsCMFunction.vsCMFunctionConstant) return null;
                if (justfunction.FunctionKind == vsCMFunction.vsCMFunctionPropertySet)
                    return GetSetterNameFrom(justfunction);
                all = GetReturnType(justfunction) + " " + GetMethodName(justfunction) + "(";
                foreach (CodeParameter2 param in justfunction.Parameters)
                {
                    if (!first) all += ",";
                    
                    all += GetParameterName(param);
                    first = false;
                }
                all += ")";
            }
            return all;
        }

        private static string GetParameterName(CodeParameter2 param)
        {
            var typeString = param.Type.AsFullName;
            GetTypeName(param.Type);
            if (param.Type.TypeKind == vsCMTypeRef.vsCMTypeRefArray)
                typeString = getArray(param.Type);
            typeString = GenericNameMangler.MangleParameterName(typeString);
            if (param.ParameterKind == vsCMParameterKind.vsCMParameterKindOut || param.ParameterKind == vsCMParameterKind.vsCMParameterKindRef)
            {
                typeString += "&";
            }
            return typeString;
        }

        private static string GetSetterNameFrom(CodeFunction justfunction)
        {
            var ret = "System.Void ";
            ret += GetMethodName(justfunction) + "(";

            var type = justfunction.Type.AsFullName;
            
            if (justfunction.Type.TypeKind == vsCMTypeRef.vsCMTypeRefArray)
                type = getArray(justfunction.Type);
            type = GenericNameMangler.MangleParameterName(type);
            ret += type + ")";
            return ret;
        }


        private static string GetFieldStringFromElement(CodeElement elem)
        {
            var v = (CodeVariable) elem;
            if (v.IsConstant) return null;
            var typeName = GenericNameMangler.MangleTypeName(GetTypeName(v.Parent));
            var oftype = GetVariableType(v);
            return oftype + " " + typeName + "::" + v.Name;
        }

        private static string GetMethodName(CodeFunction function)
        {
            var typeName = GetTypeName(function.Parent);
            var typename = GenericNameMangler.MangleTypeName(typeName);
            string method;
            switch (function.FunctionKind)
            {
                case vsCMFunction.vsCMFunctionConstructor:
                    method = function.IsShared ? ".cctor" : ".ctor";
                    break;
                case vsCMFunction.vsCMFunctionDestructor:
                    method = "Finalize";
                    break;
                case vsCMFunction.vsCMFunctionPropertyGet:
                    method = "get_" + function.Name;
                    break;
                case vsCMFunction.vsCMFunctionPropertySet:
                    method = "set_" + function.Name;
                    break;
                case vsCMFunction.vsCMFunctionOperator:
                    string translated;
                    ops.TryGetValue(function.Name.Replace("operator ", ""), out translated);
                    method = translated;
                    break;
                default:
                    method = GenericNameMangler.MangleMethodName(function.Name);
                    break;
            }
            return typename + "::" + method;
        }

        
        private static string GetTypeName(object item)
        {
            var type = item as CodeElement;
            if (type == null)
            {
                //MessageBox.Show(item.GetType()+ " " + item.GetType());
                return null;
            }
            if (type.Kind == vsCMElement.vsCMElementInterface)
                return getInterfaceName((CodeInterface) type);
            if (type.Kind == vsCMElement.vsCMElementStruct)
                return getStructName((CodeStruct2) type);
            if (type.Kind == vsCMElement.vsCMElementClass)
                return getClassName((CodeClass)type);
            if (type.Kind == vsCMElement.vsCMElementProperty)
                return GetTypeName(((CodeProperty) type).Parent);
            return null;
        }

        private static string getInterfaceName(CodeInterface @interface)
        {
            var parents = GetParents(@interface);
            var nspace = GetNameSpaceName(@interface.Namespace);
            return nspace + "." + parents;
        }

        private static string getClassName(CodeClass clazz)
        {
            string parents = GetParents(clazz);
            var nspace = GetNameSpaceName(clazz.Namespace);
            return nspace + "." + parents;
        }

        private static string GetNameSpaceName(CodeNamespace codeNamespace)
        {
            return codeNamespace.FullName;
        }

        private static string getStructName(CodeStruct structure)
        {
            string parents = GetParents(structure);
            var nspace = GetNameSpaceName(structure.Namespace);
            return nspace + "." + parents;
        }

        public static string GetParents(CodeInterface c)
        {
            var parent = c.Parent as CodeClass2;
            return GetParents((CodeElement2)c, parent);
        }

        public static string GetParents(CodeClass c)
        {

            var parent = c.Parent as CodeClass;
            return GetParents((CodeElement)c, parent);
        }

        public static string GetParents(CodeStruct c)
        {
            var parent = c.Parent as CodeClass;
            return GetParents((CodeElement)c, parent);
        }

        private static string GetParents(CodeElement c, CodeClass parent)
        {
            if (parent != null)
            {
                var name = GetParents(parent) + "/" + GetTypeNameWithoutNamespace(c.FullName);
                return name;
            }
            return GetTypeNameWithoutNamespace(c.FullName);
        }

        private static string GetTypeNameWithoutNamespace(string fullName)
        {
            if (fullName == null) return "";
            var lastdot = fullName.LastIndexOf(".");
            if (lastdot == -1) return fullName;
            return fullName.Substring(lastdot + 1, fullName.Length - lastdot - 1);
        }

        private static string GetReturnType(CodeFunction n)
        {
            if (n.Type.TypeKind == vsCMTypeRef.vsCMTypeRefArray)
                return GenericNameMangler.MangleParameterName(getArray(n.Type));
            if (n.Type.AsFullName == "") return "System.Void";
            return GenericNameMangler.MangleParameterName(n.Type.AsFullName);
        }

        private static string GetVariableType(CodeVariable n)
        {
            if (n.Type.TypeKind == vsCMTypeRef.vsCMTypeRefArray)
                return GenericNameMangler.MangleParameterName(getArray(n.Type));
            return GenericNameMangler.MangleParameterName(n.Type.AsFullName);
        }


        private static string getArray(CodeTypeRef type)
        {
            try
            {
                return type.CodeType.FullName;
            }
            catch
            {
                return type.AsString;
            }
        }


        private static Dictionary<string, string> ops;

        static MethodNameReader()
        {
            ops = new Dictionary<string, string>();
            ops.Add("implicit", "op_Implicit");
            ops.Add("explicit", "op_explicit");
            ops.Add("+", "op_Addition");
            ops.Add("-", "op_Subtraction");
            ops.Add("*", "op_Multiply");
            ops.Add("/", "op_Division");
            ops.Add("%", "op_Modulus");
            ops.Add("^", "op_ExclusiveOr");
            ops.Add("&", "op_BitwiseAnd");
            ops.Add("|", "op_BitwiseOr");
            ops.Add("&&", "op_LogicalAnd");
            ops.Add("||", "op_LogicalOr");
            ops.Add("=", "op_Assign");
            ops.Add("<<", "op_LeftShift");
            ops.Add(">>", "op_RightShift");
            //ops.Add("dunno", "op_SignedRightShift");
            //ops.Add("dunno2", "op_UnsignedRightShift");
            ops.Add("==", "op_Equality");
            ops.Add(">", "op_GreaterThan");
            ops.Add("<", "op_LessThan");
            ops.Add("!=", "op_Inequality");
            ops.Add(">=", "op_GreaterThanOrEqual");
            ops.Add("<=", "op_LessThanOrEqual");
            ops.Add("*=", "op_MultiplicationAssignment");
            ops.Add("-=", "op_SubtractionAssignment");
            ops.Add("^=", "op_ExclusiveOrAssignment");
            ops.Add("<<=", "op_LeftShiftAssignment");
            ops.Add("%=", "op_ModulusAssignment");
            ops.Add("+=", "op_AdditionAssignment");
            ops.Add("&=", "op_BitwiseAndAssignment");
            ops.Add("|=", "op_BitwiseOrAssignment");
            ops.Add("dunno", "op_Comma");
            ops.Add("/=", "op_DivisionAssignment");
            ops.Add("--", "op_Decrement");
            ops.Add("++", "op_Increment");
            //ops.Add("dunno", "op_UnaryNegation");
            //ops.Add("dunno", "op_UnaryPlus");
            //ops.Add("dunno", "op_OnesComplement");
        }
    }
}
