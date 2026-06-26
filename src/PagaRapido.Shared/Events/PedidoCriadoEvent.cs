namespace PagaRapido.Shared.Events;

public record PedidoCriadoEvent(Guid PedidoId, string ClienteNome, decimal Total);
