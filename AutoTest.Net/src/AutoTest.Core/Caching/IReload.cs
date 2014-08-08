using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Core.Caching
{
    public interface IReload<T>
    {
        void MarkAsDirty(T record);
    }
}
