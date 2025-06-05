namespace Valuator;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using RabbitMQ.Client;
using Valuator.Hubs;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();

        builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var configuration = builder.Configuration.GetSection("Redis:Configuration").Value;
            return ConnectionMultiplexer.Connect(configuration);
        });

        // RabbitMQ
        builder.Services.AddSingleton<IConnection>(sp =>
        {
            var factory = new ConnectionFactory()
            {
                HostName = builder.Configuration.GetSection("RabbitMQ:HostName").Value,
                UserName = builder.Configuration.GetSection("RabbitMQ:UserName").Value,
                Password = builder.Configuration.GetSection("RabbitMQ:Password").Value
            };
            return factory.CreateConnection();
        });

        builder.Services.AddSingleton<IModel>(sp =>
        {
            var connection = sp.GetRequiredService<IConnection>();
            var channel = connection.CreateModel();
            channel.QueueDeclare(queue: "rank_queue",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            return channel;
        });

        builder.Services.AddSignalR(); // подключема сервисы SignalR

        builder.Services.AddControllers();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }
        app.UseStaticFiles();

        app.MapControllers();

        app.MapHub<ResultsHub>("/resultsHub");   // ChatHub будет обрабатывать запросы по пути /resultsHub

        app.UseRouting();

        app.UseAuthorization();

        app.MapRazorPages();

        app.Run();
    }
}