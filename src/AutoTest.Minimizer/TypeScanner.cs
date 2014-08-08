using System;
using System.Collections.Generic;
using AutoTest.Minimizer.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace AutoTest.Minimizer
{
    public class TypeScanner
    {
        private static readonly bool[] branches;

        static TypeScanner()
        {
            branches = new bool[256];
            branches[(int) OpCodes.Beq.Code] = true;
            branches[(int) OpCodes.Beq_S.Code] = true;
            branches[(int) OpCodes.Bge.Code] = true;
            branches[(int) OpCodes.Bge_S.Code] = true;
            branches[(int) OpCodes.Bge_Un.Code] = true;
            branches[(int) OpCodes.Bge_Un_S.Code] = true;
            branches[(int) OpCodes.Bgt.Code] = true;
            branches[(int) OpCodes.Bgt_S.Code] = true;
            branches[(int) OpCodes.Bgt_Un.Code] = true;
            branches[(int) OpCodes.Bgt_Un_S.Code] = true;
            branches[(int) OpCodes.Ble.Code] = true;
            branches[(int) OpCodes.Ble_S.Code] = true;
            branches[(int) OpCodes.Blt.Code] = true;
            branches[(int) OpCodes.Blt_S.Code] = true;
            branches[(int) OpCodes.Blt_Un.Code] = true;
            branches[(int) OpCodes.Blt_Un_S.Code] = true;
            branches[(int) OpCodes.Bne_Un.Code] = true;
            branches[(int) OpCodes.Bne_Un_S.Code] = true;
            branches[(int)OpCodes.Br.Code] = true;
            branches[(int)OpCodes.Br_S.Code] = true;
            branches[(int)OpCodes.Brfalse.Code] = true;
            branches[(int)OpCodes.Brfalse_S.Code] = true;
            branches[(int)OpCodes.Brtrue.Code] = true;
            branches[(int)OpCodes.Brtrue_S.Code] = true;            
        }
        public static IList<TypeReference> GetDirectDependencies(TypeDefinition type)
        {
            var dep = new List<TypeReference>();
            
            dep.AddNotExist(type.BaseType);
            foreach (var field in type.Fields)
            {
                dep.AddNotExist(field.FieldType);
            }
            return dep;
        }

        public static ScanResult ScanMethod(MethodDefinition method)
        {
            try
            {
                var dep = new List<MemberAccess>();
                if (!method.HasBody) return new ScanResult(dep, method, 0);
                int complexity = 0;
                for (int index = 0; index < method.Body.Instructions.Count; index++)
                {
                    var instruction = method.Body.Instructions[index];
                    var memberRef = instruction.Operand as MemberReference;
                    if (branches[(int) instruction.OpCode.Code])
                    {
                        complexity++;
                    }
                    if (memberRef != null)
                    {
                        var methodReference = memberRef as MethodReference;
                        var info = new MethodCallInfo(false, null, false);
                        if (methodReference != null && methodReference.HasThis)
                        {
                            info = GetEnhancedCallInformation(instruction, method.HasThis);
                        }
                        var methodDef = memberRef as MethodDefinition;
                        if (instruction.OpCode == OpCodes.Ldfld || instruction.OpCode == OpCodes.Ldsfld ||
                            (methodDef != null && methodDef.IsConstructor))
                            dep.AddNotExist(new MemberAccess(memberRef, true, info.IsSelfCall, info.FieldReference,
                                                             methodReference, info.IsLocal));
                        else
                            dep.AddNotExist(new MemberAccess(memberRef, false, info.IsSelfCall, info.FieldReference,
                                                             methodReference, info.IsLocal));
                    }
                }
                return new ScanResult(dep, method, complexity);
            }
            catch (Exception ex)
            {
                throw new MethodDependencyDetectionFailedException(method.GetCacheName(), ex);
            }
        }

        private static MethodCallInfo GetEnhancedCallInformation(Instruction instruction, bool expectThis)
        {
            var methodRef = (MethodReference) instruction.Operand;
            var isSelfCall = false;
            var isLocal = false;
            var current = instruction;
            for (int i = 0; i < methodRef.Parameters.Count; i++)
            {
                current = current == null ? null : current.Previous;
            }
            current = current == null ? null : current.Previous; //"this"
            FieldReference reference = null;
            if (current != null)
            {
                isSelfCall = expectThis && current.OpCode == OpCodes.Ldarg_0;
                reference = current.Operand as FieldReference;
            }
            return new MethodCallInfo(isSelfCall, reference, false);
        }
    }
}