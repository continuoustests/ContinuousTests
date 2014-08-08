using System;
using System.Collections.Generic;
using System.Linq;
using AutoTest.Minimizer.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace AutoTest.Minimizer.TestIdentifiers
{
    public static class MSpecTranslator
    {
        // This code has a lot of external dependencies.
        // It depends on how the C# compiler lays out IL of anonymous methods in IL
        //     If compiler stops caching, this stops working.
        //     If mono does it even slightly differently it stops working
        //     If mspec changes the definition of their "It" this should still work
        public static FieldDefinition TranslateGeneratedMethod(MethodDefinition definition)
        {
            try
            {
                if (definition == null) return null;
                if (!definition.HasBody) return null;
                var constructor = definition.DeclaringType.Methods.FirstOrDefault(x => x.Name == ".ctor");
                if (constructor == null) return null;
                var body = constructor.Body;
                for (var i = 0; i < body.Instructions.Count; i++)
                {
                    var instruction = body.Instructions[i];
                    if (instruction.OpCode.Code == Code.Ldftn)
                    {
                        var reference = instruction.Operand as MethodReference;
                        if (reference == null) continue;
                        var resolved = reference.ThreadSafeResolve();
                        if (resolved == null) continue;
                        if (resolved.GetCacheName() == definition.GetCacheName())
                        {
                            var nextstsfld = GetNextInstructionAfter(i, body.Instructions, Code.Stsfld);
                            if (nextstsfld == -1) continue;
                            var cachedfield = body.Instructions[nextstsfld].Operand as FieldReference;
                            if (cachedfield == null) continue;
                            var nextldsfld = GetNextInstructionAfter(nextstsfld, body.Instructions, Code.Ldsfld);
                            if (nextldsfld == -1 ||
                                cachedfield.FullName !=
                                ((FieldReference) body.Instructions[nextldsfld].Operand).FullName) continue;
                            if (body.Instructions[nextldsfld + 1].OpCode.Code != Code.Stfld) continue;
                            return ((FieldReference) body.Instructions[nextldsfld + 1].Operand).Resolve();
                        }
                    }
                }
            } catch
            {
                
            }
            return null;
        }

        private static int GetNextInstructionAfter(int start, IList<Instruction> instructions, Code code)
        {
            
            for(int j=start;j<instructions.Count;j++)
            {
                if(instructions[j].OpCode.Code == code)
                {
                    return j;
                }
            }
            return -1;
        }
    }
}
