// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org.
// ****************************************************************

using System;
using System.Reflection;

namespace NUnit.Core
{
    public class AssemblyHelper
    {
        #region GetAssemblyPath

        public static string GetAssemblyPath(Type type)
        {
            return GetAssemblyPath(type.Assembly);
        }

        public static string GetAssemblyPath(Assembly assembly)
        {
            string uri = assembly.CodeBase;
            
            if (IsFileUri(uri) && !typeof(AssemblyHelper).Assembly.Location.Contains("#"))
                return GetAssemblyPathFromFileUri(uri);
            else
                return assembly.Location;
        }

        #endregion

        #region

        // Public for testing purposes
        public static string GetAssemblyPathFromFileUri(string uri)
        {
            // Skip over the file://
            int start = Uri.UriSchemeFile.Length + Uri.SchemeDelimiter.Length;

            if (System.IO.Path.DirectorySeparatorChar == '\\')
            {
                // Handle Windows Drive specifications
                if (uri[start] == '/' && uri[start + 2] == ':')
                    ++start;
            }
            else
            {
                // Assume all Linux paths are absolute
                if (uri[start] != '/')
                    --start;
            }

            return uri.Substring(start);
        }

        #endregion

        #region GetDirectoryName
        public static string GetDirectoryName( Assembly assembly )
        {
            return System.IO.Path.GetDirectoryName(GetAssemblyPath(assembly));
        }
        #endregion

        #region Helper Methods

        private static bool IsFileUri(string uri)
        {
            return uri.ToLower().StartsWith(Uri.UriSchemeFile);
        }

        #endregion
    }
}
