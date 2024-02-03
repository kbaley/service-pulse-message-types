using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using Shared.Events;

class Program
{
    static async Task Main()
    {
        Console.Title = "Samples.SimpleSender";

        var endpointConfiguration = new EndpointConfiguration("Samples.SimpleSender");
        var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>()
            .ConnectionString("-- INSERT CONNECTION STRING --");
        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.AuditProcessedMessagesTo("audit");
        var conventions = endpointConfiguration.Conventions();
        conventions.DefiningEventsAs(type => type.Namespace == "Shared.Events");
        const string SERVICE_CONTROL_METRICS_ADDRESS = "Particular.Monitoring";
        var metrics = endpointConfiguration.EnableMetrics();

        metrics.SendMetricDataToServiceControl(
            serviceControlMetricsAddress: SERVICE_CONTROL_METRICS_ADDRESS,
            interval: TimeSpan.FromSeconds(10),
            instanceId: "INSTANCEID_1");

        transport.Routing().RouteToEndpoint(typeof(MyCommand), "Samples.SimpleReceiver");
        endpointConfiguration.EnableInstallers();

        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);
        await SendMessages(endpointInstance);
        await endpointInstance.Stop()
            .ConfigureAwait(false);
    }

    static async Task SendMessages(IMessageSession messageSession)
    {
        Console.WriteLine("Press [c] to send a command, or [e] to publish an event. Press [Esc] to exit.");
        while (true)
        {
            var input = Console.ReadKey();
            Console.WriteLine();

            switch (input.Key)
            {
                case ConsoleKey.C:
                    await messageSession.Send(new MyCommand());
                    break;
                case ConsoleKey.E:
                    await messageSession.Publish<IAgentTerminalListWasExported>(msg => {
                        msg.BetDate = DateTime.Now;
                        msg.AgentTerminals = new List<AgentTerminal>();
                    });
                    break;
                case ConsoleKey.Escape:
                    return;
            }
        }
    }
}