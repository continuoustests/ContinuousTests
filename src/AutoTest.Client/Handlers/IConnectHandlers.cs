using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.Client.Handlers
{
    public interface IConnectHandlers
    {
        void Connecting(int port, bool startPaused);
        void Disconnecting(int port);
    }
}
