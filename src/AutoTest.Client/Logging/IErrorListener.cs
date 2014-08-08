using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Client.Logging
{
    public interface IErrorListener
    {
        void OnError(string message);
    }
}
