using MassTransit;
using PagaRapido.Shared.Events;

namespace PagaRapido.Notificacoes.Worker.Consumers;

public class PagamentoConfirmadoConsumer(ILogger<PagamentoConfirmadoConsumer> logger) : IConsumer<PagamentoConfirmadoEvent>
{
    public Task Consume(ConsumeContext<PagamentoConfirmadoEvent> context)
    {
        var msg = context.Message;
        logger.LogInformation(
            "📧 Notificação enviada para {Email}: Pedido {PedidoId} pago - R${Valor}",
            msg.ClienteEmail, msg.PedidoId, msg.Valor);

        return Task.CompletedTask;
    }
}
