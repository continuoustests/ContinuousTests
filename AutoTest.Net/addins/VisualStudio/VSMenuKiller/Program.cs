using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;

namespace VSMenuKiller
{
    class Program
    {
        static void Main(string[] args)
        {
            args = new string[] { "VisualStudio.DTE.10.0", "MenuBar", "ContinuousTests" };
            try
            {
                if (args.Length < 3)
                    throw new Exception("Usage 'VisualStudio.DTE.10.0' 'MenuBar' 'AutoTest.Net' ['SomeMenu'] [--non-recursive]");

                object control = null;
                var dte = getDte(args[0]);
                var nonRecursive = args.Count(x => x.Equals("--non-recursive")) > 0;
                IEnumerator controls = getCommandBars(dte, args[1]);
                for (int i = 2; i < args.Length; i++)
                {
                    if (args[i].StartsWith("--"))
                        continue;
                    control = getControl(controls, args[i]);
                    controls = getControls(control);
                }

                if (control == null)
                {
                    Console.WriteLine("Specified menu was not found");
                    return;
                }

                var toDelete = new List<object>();
                if (!nonRecursive)
                {
                    while (controls.MoveNext())
                    {
                        toDelete.Add(controls.Current);
                    }
                }
                toDelete.Add(control);

                foreach (var ctl in toDelete)
                {
                    try
                    {
                        Console.WriteLine("Deleting. " + ctl.Get<string>("accName"));
                        ctl.Invoke("Delete");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                Thread.Sleep(1000);
                dte.Invoke("Quit");

                Console.WriteLine("Done");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static object getDte(string signature)
        {
            var t = Type.GetTypeFromProgID(signature);
            return Activator.CreateInstance(t, true);
        }

        private static object getControl(IEnumerator controls, string name)
        {
            while (controls.MoveNext())
            {
                if (controls.Current.Get<string>("accName") == name)
                    return controls.Current;
            }
            return null;
        }

        private static IEnumerator getControls(object control)
        {
            if (control == null)
                return null;
            return control.Get("Controls").Invoke<IEnumerator>("GetEnumerator");
        }

        private static IEnumerator getCommandBars(object dte, string commandBar)
        {
            var enumerator = dte.Get("CommandBars").Invoke<IEnumerator>("GetEnumerator");
            while (enumerator.Invoke<bool>("MoveNext"))
            {
                var bar = enumerator.Current;
                if (bar.Get<string>("Name") == commandBar)
                    return bar.Get("Controls").Invoke<IEnumerator>("GetEnumerator");
            }
            return null;
        }
    }

    static class objectExtensions
    {
        public static object Invoke(this object obj, string methodName)
        {
            return obj.GetType().InvokeMember(methodName, System.Reflection.BindingFlags.InvokeMethod, null, obj, new object[] { });
        }

        public static T Invoke<T>(this object obj, string methodName)
        {
            return (T) obj.GetType().InvokeMember(methodName, System.Reflection.BindingFlags.InvokeMethod, null, obj, new object[] { });
        }

        public static object Get(this object obj, string propertyName)
        {
            return obj.GetType().InvokeMember(propertyName, System.Reflection.BindingFlags.GetProperty, null, obj, new object[] { });
        }

        public static T Get<T>(this object obj, string propertyName)
        {
            return (T) obj.GetType().InvokeMember(propertyName, System.Reflection.BindingFlags.GetProperty, null, obj, new object[] { });
        }
    }
}
