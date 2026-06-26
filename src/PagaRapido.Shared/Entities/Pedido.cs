using PagaRapido.Shared.Enums;

namespace PagaRapido.Shared.Entities;

public class Pedido
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ClienteNome { get; set; } = string.Empty;
    public string ClienteEmail { get; set; } = string.Empty;
    public StatusPedido Status { get; set; } = StatusPedido.Criado;
    public decimal Total => Itens.Sum(i => i.Quantidade * i.PrecoUnitario);
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? PagoEm { get; set; }
    public string? PixQrCode { get; set; }

    public List<ItemPedido> Itens { get; set; } = [];
}
