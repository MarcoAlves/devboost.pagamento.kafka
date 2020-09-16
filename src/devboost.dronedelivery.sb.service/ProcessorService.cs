using devboost.dronedelivery.sb.domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace devboost.dronedelivery.sb.service
{
    public class ProcessorService : IProcessorQueue
    {
        private readonly IConsumer _consumer;
        private readonly ILoginProvider _loginProvider;
        private readonly IPedidosService _pedidoService;
        private readonly IPagamentoService _pagamentoService;

        public ProcessorService(IConsumer consumer, ILoginProvider loginProvider, IPedidosService pedidosService, IPagamentoService pagamentoService)
        {
            _consumer = consumer;
            _loginProvider = loginProvider;
            _pedidoService = pedidosService;
            _pagamentoService = pagamentoService;

        }
        public async Task ProcessorQueueAsync()
        {
            using var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var messages = await _consumer.ExecuteAsync(cancellationToken.Token, "pedido");
            var token = await _loginProvider.GetTokenAsync();
            foreach (var message in messages)
            {
                await _pedidoService.ProcessPedidoAsync(token, message);
            }

            using var cancellationTokenPagamento = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var messagesPagamento = await _consumer.ExecuteAsync(cancellationTokenPagamento.Token, "pagamento-status");
            var tokenPagamento = await _loginProvider.GetTokenAsync();
            foreach (var message in messagesPagamento)
            {
                await _pagamentoService.ProcessPagamentoAsync(tokenPagamento, message);
            }

        }
    }
}
