using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace InvestmentManager.QueueMessage
{
    public interface IQueueMessage
    {
        Task<bool> SendMessage(string message);
    }

    public class RabbitMqMessage : IQueueMessage
    {
        private const string _queueName = "InvestmentManagerHealthChecks";
        private const string _host = "localhost";
        private ILogger<RabbitMqMessage> _logger;

        public RabbitMqMessage(ILogger<RabbitMqMessage> logger)
        {
            _logger = logger;
        }

        public Task<bool> SendMessage(string message)
        {
            var factory = new ConnectionFactory {HostName = _host};
            using(var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false,
                    autoDelete: false, arguments: null);

                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: null, body: body);
                _logger.LogInformation($"Sent message of length {message.Length}");

            }

            return Task.FromResult(true);
        }
    }
}
