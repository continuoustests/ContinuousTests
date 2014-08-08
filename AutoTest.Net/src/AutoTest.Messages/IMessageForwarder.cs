using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Messages
{
    public interface IMessageForwarder
    {
        void Forward(object message);
    }
}
