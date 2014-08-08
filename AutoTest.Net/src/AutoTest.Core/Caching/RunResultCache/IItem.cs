using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Core.Caching.RunResultCache
{
    public interface IItem
    {
        string ToString();
        void HandleLink(string link);
    }
}
