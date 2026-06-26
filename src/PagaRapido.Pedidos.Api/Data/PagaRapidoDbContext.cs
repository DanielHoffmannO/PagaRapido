using Microsoft.EntityFrameworkCore;
using PagaRapido.Shared.Entities;

namespace PagaRapido.Pedidos.Api.Data;

public class PagaRapidoDbContext(DbContextOptions<PagaRapidoDbContext> options) : DbContext(options)
{
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<ItemPedido> ItensPedido => Set<ItemPedido>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pedido>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.ClienteNome).HasMaxLength(200).IsRequired();
            e.Property(p => p.ClienteEmail).HasMaxLength(200).IsRequired();
            e.Property(p => p.Status).HasConversion<int>();
            e.Ignore(p => p.Total);
            e.HasMany(p => p.Itens).WithOne().HasForeignKey(i => i.PedidoId);
        });

        modelBuilder.Entity<ItemPedido>(e =>
        {
            e.HasKey(i => i.Id);
            e.Property(i => i.Produto).HasMaxLength(200).IsRequired();
            e.Property(i => i.PrecoUnitario).HasPrecision(18, 2);
        });
    }
}
