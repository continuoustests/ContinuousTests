using System;

namespace AutoTest.VS.Util
{
     public class GenericNameMangler
    {
        static string ReadNameWithParameters(string fullname, ref int start)
        {
            if (fullname == null) return null; 
            string name = "";
            for (int i = start; i < fullname.Length; i++)
            {
                start = i;
                if (fullname[i] == '<')
                {
                    i++;
                    return name + ReadArgumentsWithNumber(fullname, ref i);
                }
                name += fullname[i];
            }
            return name;
        }

        static string ReadNameAndCount(string fullname, ref int start)
        {
            if (fullname == null) return null;
            string name = "";
            for (int i = start; i < fullname.Length; i++)
            {
                start = i;
                if (fullname[i] == '<')
                {
                    i++;
                    return name + OnlyReadNumberOfParams(fullname, ref i);
                }
                name += fullname[i];
            }
            return name;
        }

        static string ReadNameOnly(string fullname, ref int start)
        {
            string name = "";
            for (int i = start; i < fullname.Length; i++)
            {
                start = i;
                if (fullname[i] == '<')
                {
                    i++;
                    return name;
                }
                name += fullname[i];
            }
            return name;
        }

        static string ReadArgumentsWithNumber(string fullname, ref int start)
        {
            if(fullname == null) return null;
            string param = "";
            int count = 1;
            for (int i = start; i < fullname.Length; i++)
            {
                start = i;
                if (fullname[i] == '<')
                {
                    i++;
                    param += ReadArgumentsWithNumber(fullname, ref i);
                    continue;
                }
                if (fullname[i] == ',') count++;
                if (fullname[i] == '>')
                {
                    var number = "`" + count;
                    return number + "<" + param + ">";
                }
                param += fullname[i];
            }
            return param;
        }
        static string OnlyReadNumberOfParams(string fullname, ref int start)
        {
            if (fullname == null) return null;
            string param = "";
            int count = 1;
            for (int i = start; i < fullname.Length; i++)
            {
                start = i;
                if (fullname[i] == '<')
                {
                    i++;
                    ReadArgumentsWithNumber(fullname, ref i);
                    continue;
                }
                if (fullname[i] == ',') count++;
                if (fullname[i] == '>')
                {
                    var number = "`" + count;
                    return number;
                }
                param += fullname[i];
            }
            return param;
        }
         public static string MangleMethodName(string name)
         {
             try
             {
                 int x = 0;
                 return ReadNameOnly(name, ref x);
             }
             catch(Exception ex)
             {
                 AutoTest.Core.DebugLog.Debug.WriteDebug("failed to mangle " + name);
                 throw ex;
             }
         }

         public static object MangleTypeName(string name)
         {
             AutoTest.Core.DebugLog.Debug.WriteDebug("Mangling type " + name);
             int x = 0;
             return ReadNameAndCount(name, ref x);
         }

         public static string MangleParameterName(string name)
         {
             var x = 0;
             return ReadNameWithParameters(name, ref x);
         }

    }

     class Entry
     {
         public string Name;
         public int Count;

         public Entry(string name, int count)
         {
             Name = name;
             Count = count;
         }
     }
}
