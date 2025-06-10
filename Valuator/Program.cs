using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using RabbitMQ.Client;
using Valuator;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();

        builder.Services.AddSingleton<RedisShardManager>(sp =>
        {
            var config = new Dictionary<string, string>
            {
                ["MAIN"] = builder.Configuration["Redis:Main"],
                ["RU"] = builder.Configuration["Redis:RU"],
                ["EU"] = builder.Configuration["Redis:EU"],
                ["ASIA"] = builder.Configuration["Redis:ASIA"]
            };
            return new RedisShardManager(new ConfigurationBuilder()
                .AddInMemoryCollection(config)
                .Build());
        });

        // RabbitMQ
        builder.Services.AddSingleton<IConnection>(sp =>
        {
            var factory = new ConnectionFactory()
            {
                HostName = builder.Configuration["RabbitMQ:HostName"],
                UserName = builder.Configuration["RabbitMQ:UserName"],
                Password = builder.Configuration["RabbitMQ:Password"]
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

        builder.Services.AddControllers();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }
        app.UseStaticFiles();

        app.MapControllers();

        app.UseRouting();

        app.UseAuthorization();

        app.MapRazorPages();

        app.Run();
    }
}