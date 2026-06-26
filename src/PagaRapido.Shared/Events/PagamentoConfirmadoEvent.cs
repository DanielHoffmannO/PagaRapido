namespace PagaRapido.Shared.Events;

public record PagamentoConfirmadoEvent(Guid PedidoId, string ClienteEmail, decimal Valor, DateTime PagoEm);
