namespace PagaRapido.Pedidos.Api.Dtos;

public record CriarPedidoRequest(string ClienteNome, string ClienteEmail, List<ItemPedidoDto> Itens);
public record ItemPedidoDto(string Produto, int Quantidade, decimal PrecoUnitario);
