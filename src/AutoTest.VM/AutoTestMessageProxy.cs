using System;
using AutoTest.Core.Presenters;
using AutoTest.Messages;
using AutoTest.Core.Caching.RunResultCache;
using AutoTest.VM.Messages;
using AutoTest.Core.Configuration;
using AutoTest.VM.Messages.Communication;
using AutoTest.Core.Notifiers;
using System.Collections.Generic;
using AutoTest.Core.Messaging;

namespace AutoTest.VM
{
    class AutoTestMessageProxy : MessageProxy, IConsumerOf<AssembliesMinimizedMessage>
    {
        public AutoTestMessageProxy(IRunFeedbackPresenter presenter, IInformationFeedbackPresenter infoPresenter, IConfiguration configuration, ISendNotifications notifier)
            : base(presenter, infoPresenter, configuration, notifier)
        {
        }

        public void Consume(AssembliesMinimizedMessage message)
        {
            Logger.WriteDetails("handling minimize completed");
            trySend(message);
        }
    }
}
