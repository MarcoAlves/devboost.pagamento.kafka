using Confluent.Kafka;
using System.Threading.Tasks;

namespace devboost.dronedelivery.sb.domain.Interfaces
{
    public interface IProducerService
    {

        Task<DeliveryResult<Null, string>> SendMessage(string topic, string message);

    }
}
