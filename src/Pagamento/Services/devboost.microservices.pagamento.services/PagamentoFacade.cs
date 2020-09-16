using devboost.dronedelivery.core.domain;
using devboost.dronedelivery.core.domain.Entities;
using devboost.dronedelivery.core.domain.Enums;
using devboost.dronedelivery.pagamento.domain.Interfaces;
using devboost.dronedelivery.pagamento.EF.Integration.Interfaces;
using devboost.dronedelivery.pagamento.EF.Repositories.Interfaces;
using devboost.dronedelivery.sb.domain.Interfaces;
using devboost.dronedelivery.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace devboost.dronedelivery.pagamento.services
{
    public class PagamentoFacade : IPagamentoFacade
    {
        private readonly IPagamentoRepository _pagamentoRepository;
        private readonly IPagamentoIntegration _pagamentoIntegration;
        private readonly IProducerService _producerService;

        public PagamentoFacade(IPagamentoRepository pagamentoRepository, IPagamentoIntegration pagamentoIntegration, IProducerService producerService)
        {
            _pagamentoRepository = pagamentoRepository;
            _pagamentoIntegration = pagamentoIntegration;
            _producerService = producerService;
        }

        public async Task<Pagamento> CriarPagamento(PagamentoCreateDto pagamento)
        {
            var newPagamento = new Pagamento
            {
                DadosPagamentos = pagamento.DadosPagamentos,
                TipoPagamento = pagamento.TipoPagamento,
                StatusPagamento = EStatusPagamento.EM_ANALISE,
                DataCriacao = DateTime.Now,
                Descricao = pagamento.Descricao
            };

            var result = await _pagamentoRepository.AddAsync(newPagamento);
            if(result != null)
            {
                var pagamentoMessage = JsonConvert.SerializeObject(result);
                var resultMessage = await _producerService.SendMessage("pagamento-status", pagamentoMessage);
            }

            return newPagamento;
        }

        private EStatusPagamento RandomPagamento()
        {
            return (EStatusPagamento)new Random().Next(1, 2);
        }


        public async Task<IEnumerable<PagamentoStatusDto>> VerificarStatusPagamentos()
        {
            var pagamentosResult = await _pagamentoRepository.GetPagamentosEmAnaliseAsync();

            List<PagamentoStatusDto> pagamentos = new List<PagamentoStatusDto>();

            foreach (var pagamento in pagamentosResult)
            {
                var status = RandomPagamento();

                var pagamentoStatusDto = new PagamentoStatusDto
                {
                    IdPagamento = pagamento.Id,
                    Status = status,
                    Descricao = pagamento.Descricao
                };

                pagamentos.Add(pagamentoStatusDto);

                AtualizarStatusPagamento(status, pagamento);
            }

            if (await _pagamentoIntegration.ReportarResultadoAnalise(pagamentos))
                await _pagamentoRepository.SaveAsync();
            else
                pagamentos = null;

            return pagamentos;
        }

        private void AtualizarStatusPagamento(EStatusPagamento eStatusPagamento, Pagamento pagamento)
        {
            pagamento.StatusPagamento = eStatusPagamento;
            _pagamentoRepository.SetState(pagamento, EntityState.Modified);
        }

    }
}
