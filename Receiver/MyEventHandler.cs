namespace Receiver
{
    using System.Threading.Tasks;
    using NServiceBus;
    using NServiceBus.Logging;
    using Shared.Events;

    public class MyEventHandler : IHandleMessages<MyEvent>
    {
        static ILog log = LogManager.GetLogger<MyEventHandler>();

        public Task Handle(MyEvent eventMessage, IMessageHandlerContext context)
        {
            log.Info($"Hello from {nameof(MyEventHandler)}");
            return Task.CompletedTask;
        }
    }

    public class AgentHandler : IHandleMessages<IAgentTerminalListWasExported>
    {
        static ILog log = LogManager.GetLogger<IAgentTerminalListWasExported>();

        public Task Handle(IAgentTerminalListWasExported eventMessage, IMessageHandlerContext context)
        {
            log.Info($"Hello from {nameof(IAgentTerminalListWasExported)}");
            return Task.CompletedTask;
        }
    }
}