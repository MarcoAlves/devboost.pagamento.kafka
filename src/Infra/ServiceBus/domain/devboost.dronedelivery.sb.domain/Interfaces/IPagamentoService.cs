using System.Threading.Tasks;

namespace devboost.dronedelivery.sb.domain.Interfaces
{
    public interface IPagamentoService
    {

        Task ProcessPagamentoAsync(string token, string pedido);

    }
}
