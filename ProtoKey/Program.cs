using System.Threading.Channels;
using ProtoKey.Models;
using ProtoKey.Services;

namespace ProtoKey;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();

        var commandChannel = Channel.CreateUnbounded<Command>();
        builder.Services.AddSingleton(commandChannel);
        var setLogChannel = Channel.CreateUnbounded<Command>();


        var app = builder.Build();
        app.MapControllers();

        app.MapGet("/", () => "ProtoKey running");

        _ = Task.Run(() =>
        {
            var storeWorker = new KeyValueWorker(commandChannel.Reader, setLogChannel.Writer);
            return storeWorker.RunAsync();
        });

        _ = Task.Run(() =>
        {
            var saver = new SaverWorker(setLogChannel.Reader);
            return saver.RunAsync(CancellationToken.None);
        });

        await app.RunAsync();
    }
}