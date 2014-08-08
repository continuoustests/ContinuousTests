using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE80;

namespace AutoTest.VS.Util.Navigation
{
    public class TypeNavigation
    {
        public void GoToType(DTE2 application, string assembly, string typename)
        {
            using (var converter = new AutoTest.UI.CodeReflection.TypeConverter(assembly))
            {
                try
                {
                    var signature = converter.ToSignature(typename);
                    if (signature != null)
                    {
                        if (signature.Type == UI.CodeReflection.SignatureTypes.Class)
                            AutoTest.VS.Util.MethodFinder_Slow.GotoTypeByFullName(application, signature.Name);
                        if (signature.Type == UI.CodeReflection.SignatureTypes.Method || signature.Type == UI.CodeReflection.SignatureTypes.Field)
                            AutoTest.VS.Util.MethodFinder_Slow.GotoMethodByFullname(signature.Name, application);
                    }
                }
                catch
                {
                }
            }
        }
    }
}
