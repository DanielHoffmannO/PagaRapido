using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagaRapido.Pedidos.Api.Data;
using PagaRapido.Pedidos.Api.Dtos;
using PagaRapido.Shared.Entities;
using PagaRapido.Shared.Events;

namespace PagaRapido.Pedidos.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController(PagaRapidoDbContext db, IPublishEndpoint publishEndpoint) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarPedidoRequest request)
    {
        var pedido = new Pedido
        {
            ClienteNome = request.ClienteNome,
            ClienteEmail = request.ClienteEmail,
            Itens = request.Itens.Select(i => new ItemPedido
            {
                Produto = i.Produto,
                Quantidade = i.Quantidade,
                PrecoUnitario = i.PrecoUnitario
            }).ToList()
        };

        db.Pedidos.Add(pedido);
        await db.SaveChangesAsync();

        await publishEndpoint.Publish(new PedidoCriadoEvent(pedido.Id, pedido.ClienteNome, pedido.Total));

        return CreatedAtAction(nameof(ObterPorId), new { id = pedido.Id }, pedido);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var pedido = await db.Pedidos.Include(p => p.Itens).FirstOrDefaultAsync(p => p.Id == id);
        return pedido is null ? NotFound() : Ok(pedido);
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var pedidos = await db.Pedidos.Include(p => p.Itens).OrderByDescending(p => p.CriadoEm).ToListAsync();
        return Ok(pedidos);
    }
}
