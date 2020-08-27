using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;

namespace InvestmentManager.QueueMessage
{
    public class HealthCheckPublisher : IHealthCheckPublisher
    {
        private readonly IQueueMessage _queueMessage;

        public HealthCheckPublisher(IQueueMessage queueMessage)
        {
            _queueMessage = queueMessage;
        }

        public Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
        {
            var message = JsonConvert.SerializeObject(report, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return _queueMessage.SendMessage(message);
        }
    }
}
