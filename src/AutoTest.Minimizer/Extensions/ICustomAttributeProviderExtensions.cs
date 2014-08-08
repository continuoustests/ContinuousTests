using System.Linq;
using Mono.Cecil;
using System;
namespace AutoTest.Minimizer.Extensions
{
    public static class ICustomAttributeProviderExtensions
    {
        public static bool ContainsIgnoreAttribute(this ICustomAttributeProvider provider)
        {
            int x = 5;
            Other2.DoesSomething();
            return provider.ContainsAttribute("MightyMooseIgnoreAttribute");
        }



        public static bool ContainsAttribute(this ICustomAttributeProvider provider, string name)
        {
            foreach(var attr in provider.CustomAttributes)
            {
                
                var type = attr.AttributeType;
                while(type != null)
                {
                    if (type.Name == name) return true;
                    var def = type.ThreadSafeResolve();
                    type = def == null? null : def.BaseType;
                }
            }
            return false;
        }

    }

    static class Other2
    {
        public static void DoesSomething()
        {
            Other.Bar();
        }
    }

    static class Other
    {


        public static void Bar()
        {
            //System.Threading.Thread.Sleep(1000);
            Baz();
        }

        public static void Baz()
        {
           //System.Threading.Thread.Sleep(1100);
        }
    }
}
