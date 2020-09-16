using Confluent.Kafka;
using devboost.dronedelivery.sb.domain.Interfaces;
using System.Threading.Tasks;

namespace devboost.dronedelivery.sb.service
{
    public class ProducerService : IProducerService
    {


        public async Task<DeliveryResult<Null, string>> SendMessage(string topic, string message)
        {
            string bootstrapServers = "localhost:9092";
            var nomeTopic = topic;

            var config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers
            };

            using var producer = new ProducerBuilder<Null, string>(config).Build();

            return await producer.ProduceAsync(
                nomeTopic,
                new Message<Null, string>
                { Value = message });
        }
    }
}
