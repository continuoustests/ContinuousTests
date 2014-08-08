using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Diagnostics
{
    class ListItemInfo<T>
    {
        public T Message { get; private set; }

        public ListItemInfo(T message)
        {
            Message = message;
        }
    }
}
