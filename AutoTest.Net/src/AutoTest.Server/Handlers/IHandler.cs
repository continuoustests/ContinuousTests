using System;
using System.Collections.Generic;

namespace AutoTest.Server.Handlers
{
    interface IHandler
    {
        void DispatchThrough(Action<string, object> dispatcher);
    }

	interface IClientHandler
	{
        Dictionary<string, Action<dynamic>> GetClientHandlers();
	}

    interface IClientResponseHandler
    {
        Dictionary<string, Action<dynamic, Action<object>>> GetClientResponseHandlers();
    }

    interface IInternalMessageHandler
    {
        void OnInternalMessage(object message);
    }
}