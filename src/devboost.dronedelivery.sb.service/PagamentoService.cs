using devboost.dronedelivery.sb.domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace devboost.dronedelivery.sb.service
{
    public class PagamentoService : HttpServiceBase, IPagamentoService
    {

        private const string MediaType = "application/json";
        private const string PagamentoUri = "api/Pedidos/atualizar-pedido";
        private const string AuthorizationType = "Bearer";

        public PagamentoService(IConfiguration configuration) : base(configuration)
        {


        }

        public async Task ProcessPagamentoAsync(string token, string pagamento)
        {
            using var httpClient = new HttpClient
            {
                BaseAddress = new Uri(_urlPedidos)
            };

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationType, token);
            var request = new StringContent(pagamento, Encoding.UTF8, MediaType);
            var response = await httpClient.PostAsync(PagamentoUri, request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error to post pagamento");
            }
        }

    }
}
