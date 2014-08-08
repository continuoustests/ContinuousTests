using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoTest.VM.Messages;

namespace AutoTest.Client.Handlers
{
    public interface IStartupHandler : ISpawnHandler, IConnectHandlers, IMessageHandlers
    {
    }
}
