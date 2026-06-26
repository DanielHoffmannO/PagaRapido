using MassTransit;
using PagaRapido.Notificacoes.Worker.Consumers;

var builder = Host.CreateApplicationBuilder(args);

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

var host2 = builder.Build();
host2.Run();
