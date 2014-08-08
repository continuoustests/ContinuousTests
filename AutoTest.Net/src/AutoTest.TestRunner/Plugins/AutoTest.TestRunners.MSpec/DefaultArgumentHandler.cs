using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace LinFu.DynamicProxy
{
    internal class DefaultArgumentHandler : IArgumentHandler
    {
        #region IArgumentHandler Members

        public void PushArguments(ParameterInfo[] parameters, ILGenerator IL, bool isStatic)
        {
            int parameterCount = parameters == null ? 0 : parameters.Length;

            // object[] args = new object[size];
            IL.Emit(OpCodes.Ldc_I4, parameterCount);
            IL.Emit(OpCodes.Newarr, typeof (object));
            IL.Emit(OpCodes.Stloc_S, 0);

            if (parameterCount == 0)
            {
                IL.Emit(OpCodes.Ldloc_S, 0);
                return;
            }

            // Populate the object array with the list of arguments
            int index = 0;
            int argumentPosition = 1;
            foreach (ParameterInfo param in parameters)
            {
                Type parameterType = param.ParameterType.IsByRef ? param.ParameterType.GetElementType() : param.ParameterType; 
                // args[N] = argumentN (pseudocode)
                IL.Emit(OpCodes.Ldloc_S, 0);
                IL.Emit(OpCodes.Ldc_I4, index);

                // Zero out the [out] parameters
                if (param.IsOut)
                {
                    IL.Emit(OpCodes.Ldnull);
                    IL.Emit(OpCodes.Stelem_Ref);
                    argumentPosition++;
                    index++;
                    continue;
                }

                IL.Emit(OpCodes.Ldarg, argumentPosition);

                bool isGeneric = parameterType.IsGenericParameter;

                if (param.ParameterType.IsByRef)
                {
                    var typeName = param.ParameterType.Name;
                    var ldindInstruction = OpCodes.Ldind_Ref;
                    var ldindMap = new Dictionary<string, OpCode>();
                    ldindMap["Bool&"] = OpCodes.Ldind_I1;
                    ldindMap["Int8&"] = OpCodes.Ldind_I1;
                    ldindMap["Uint8&"] = OpCodes.Ldind_I1;

                    ldindMap["Int16&"] = OpCodes.Ldind_I2;
                    ldindMap["Uint16&"] = OpCodes.Ldind_I2;

                    ldindMap["Uint32&"] = OpCodes.Ldind_I4;
                    ldindMap["Int32&"] = OpCodes.Ldind_I4;

                    ldindMap["IntPtr"] = OpCodes.Ldind_I4;
                    ldindMap["Uint64&"] = OpCodes.Ldind_I8;
                    ldindMap["Int64&"] = OpCodes.Ldind_I8;
                    ldindMap["Float32&"] = OpCodes.Ldind_R4;
                    ldindMap["Float64&"] = OpCodes.Ldind_R8;

                    if (ldindMap.ContainsKey(typeName))
                        ldindInstruction = ldindMap[typeName];

                    IL.Emit(ldindInstruction);
                }

                if (parameterType.IsValueType || parameterType.IsByRef || isGeneric)
                    IL.Emit(OpCodes.Box, parameterType);

                IL.Emit(OpCodes.Stelem_Ref);

                index++;
                argumentPosition++;
            }
            IL.Emit(OpCodes.Ldloc_S, 0);
        }

        #endregion
    }
}