using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PagaRapido.Pedidos.Api.Data;
using PagaRapido.Pedidos.Api.Hubs;
using PagaRapido.Shared.Enums;
using PagaRapido.Shared.Events;

namespace PagaRapido.Pedidos.Api.Consumers;

public class PagamentoConfirmadoConsumer(
    PagaRapidoDbContext db,
    IHubContext<PedidoHub> hubContext,
    ILogger<PagamentoConfirmadoConsumer> logger) : IConsumer<PagamentoConfirmadoEvent>
{
    public async Task Consume(ConsumeContext<PagamentoConfirmadoEvent> context)
    {
        var msg = context.Message;
        var pedido = await db.Pedidos.FirstOrDefaultAsync(p => p.Id == msg.PedidoId);

        if (pedido is not null)
        {
            pedido.Status = StatusPedido.PagamentoConfirmado;
            pedido.PagoEm = msg.PagoEm;
            await db.SaveChangesAsync();

            await hubContext.Clients.Group(msg.PedidoId.ToString())
                .SendAsync("PagamentoConfirmado", new { msg.PedidoId, msg.Valor, msg.PagoEm });

            logger.LogInformation("Pedido {PedidoId} atualizado para PagamentoConfirmado e cliente notificado via SignalR", msg.PedidoId);
        }
    }
}
