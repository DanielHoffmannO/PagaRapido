using Microsoft.AspNetCore.SignalR;

namespace PagaRapido.Pedidos.Api.Hubs;

public class PedidoHub : Hub
{
    public async Task AcompanharPedido(string pedidoId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, pedidoId);
    }
}
