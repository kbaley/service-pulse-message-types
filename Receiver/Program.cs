using System;

namespace Receiver
{
    using System.Threading.Tasks;
    using NServiceBus;

    class Program
    {
        static async Task Main()
        {
            Console.Title = "Samples.SimpleReceiver";
            var endpointConfiguration = new EndpointConfiguration("Samples.SimpleReceiver");
            var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>()
                .ConnectionString("--INSERT CONNECTION STRING--");
            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningEventsAs(type => type.Namespace == "Shared.Events");
            const string SERVICE_CONTROL_METRICS_ADDRESS = "Particular.Monitoring";
            var metrics = endpointConfiguration.EnableMetrics();

            metrics.SendMetricDataToServiceControl(
                serviceControlMetricsAddress: SERVICE_CONTROL_METRICS_ADDRESS,
                interval: TimeSpan.FromSeconds(10),
                instanceId: "INSTANCEID_1");
            endpointConfiguration.EnableInstallers();

            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }
}
