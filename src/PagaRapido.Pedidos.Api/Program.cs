using MassTransit;
using Microsoft.EntityFrameworkCore;
using PagaRapido.Pedidos.Api.Consumers;
using PagaRapido.Pedidos.Api.Data;
using PagaRapido.Pedidos.Api.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.AddDbContext<PagaRapidoDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PagamentoConfirmadoConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var host = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
        var user = builder.Configuration["RabbitMQ:Username"] ?? "guest";
        var pass = builder.Configuration["RabbitMQ:Password"] ?? "guest";
        var vhost = builder.Configuration["RabbitMQ:VHost"] ?? "/";
        var useSsl = host != "localhost";

        cfg.Host(host, vhost, h =>
        {
            h.Username(user);
            h.Password(pass);
            if (useSsl) h.UseSsl(s => s.Protocol = System.Security.Authentication.SslProtocols.Tls12);
        });
        cfg.ConfigureEndpoints(context);
    });
});

var allowedOrigins = builder.Configuration["AllowedOrigins"] ?? "http://localhost:5173";
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials()
              .WithOrigins(allowedOrigins.Split(',')));
});

var app = builder.Build();

// Auto-create DB em qualquer ambiente (projeto pessoal sem migrations)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PagaRapidoDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseCors();
app.MapControllers();
app.MapHub<PedidoHub>("/hubs/pedido");
app.Run();
