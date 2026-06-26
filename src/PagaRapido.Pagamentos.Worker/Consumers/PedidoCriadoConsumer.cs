using MassTransit;
using PagaRapido.Shared.Events;

namespace PagaRapido.Pagamentos.Worker.Consumers;

public class PedidoCriadoConsumer(ILogger<PedidoCriadoConsumer> logger, IPublishEndpoint publishEndpoint) : IConsumer<PedidoCriadoEvent>
{
    public async Task Consume(ConsumeContext<PedidoCriadoEvent> context)
    {
        var pedido = context.Message;
        logger.LogInformation("Processando pagamento Pix para pedido {PedidoId} - R${Total}", pedido.PedidoId, pedido.Total);

        // Simula processamento Pix (2-5 segundos)
        await Task.Delay(Random.Shared.Next(2000, 5000));

        logger.LogInformation("Pagamento Pix confirmado para pedido {PedidoId}", pedido.PedidoId);

        await publishEndpoint.Publish(new PagamentoConfirmadoEvent(
            pedido.PedidoId,
            pedido.ClienteNome,
            pedido.Total,
            DateTime.UtcNow));
    }
}
