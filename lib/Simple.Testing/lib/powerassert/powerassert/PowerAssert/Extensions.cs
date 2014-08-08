using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerAssert
{
    public static class Extensions
    {
        public static string CleanupName(this string name)
        {
            var tmp = name.CleanupUnderScores();
            return tmp.CleanupCamelCasing();
        }

        public static string CleanupUnderScores(this string name)
        {
            if (name.Contains('_'))
                return name.Replace('_', ' ');
            return name;
        }

        public static string CleanupCamelCasing(this string name)
        {
            return System.Text.RegularExpressions.Regex.Replace(name,
            "([A-Z])",
            " $1",
            System.Text.RegularExpressions.RegexOptions.Compiled
            ).Trim();
        }
    }
}
