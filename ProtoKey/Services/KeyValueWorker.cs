using System.Threading.Channels;
using ProtoKey.Models;

namespace ProtoKey.Services;

public class KeyValueWorker
{
    private readonly ChannelReader<Command> _commandReader;
    private readonly ChannelWriter<Command> _setLogWriter;
    private readonly Dictionary<string, int> _store = new();

    public KeyValueWorker(
        ChannelReader<Command> commandReader,
        ChannelWriter<Command> setLogWriter
    )
    {
        _commandReader = commandReader;
        _setLogWriter = setLogWriter;
        Utils.LoadFromDisk(_store);
    }

    public async Task RunAsync()
    {
        await foreach (var cmd in _commandReader.ReadAllAsync())
        {
            try
            {
                await HandleCommandAsync(cmd);
            }
            catch (Exception ex)
            {
                cmd.Completion.SetException(ex);
            }
        }
    }

    private Task HandleCommandAsync(Command cmd)
    {
        return cmd.Type switch
        {
            CommandType.Set => HandleSetAsync(cmd),
            CommandType.Get => HandleGetAsync(cmd),
            CommandType.Keys => HandleKeysAsync(cmd),
            _ => Task.FromException(
                new InvalidOperationException($"Unknown command type: {cmd.Type}")
            )
        };
    }

    private Task HandleSetAsync(Command cmd)
    {
        _store[cmd.Key] = cmd.Value!.Value;
        _ = _setLogWriter.TryWrite(cmd);
        cmd.Completion.SetResult(null);
        return Task.CompletedTask;
    }

    private Task HandleGetAsync(Command cmd)
    {
        _store.TryGetValue(cmd.Key, out var val);
        cmd.Completion.SetResult(val);
        return Task.CompletedTask;
    }

    private Task HandleKeysAsync(Command cmd)
    {
        var result = _store.Keys
            .Where(k => k.StartsWith(cmd.Prefix ?? ""))
            .ToList();
        cmd.Completion.SetResult(result);
        return Task.CompletedTask;
    }
}