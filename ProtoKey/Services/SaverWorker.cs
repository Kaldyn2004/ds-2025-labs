using System.Threading.Channels;
using ProtoKey.Models;

namespace ProtoKey.Services;

public class SaverWorker
{
    private readonly ChannelReader<Command> _reader;
    private readonly string _filePath = "ProtoKey.data";
    private readonly List<string> _buffer = new();

    public SaverWorker(ChannelReader<Command> reader)
    {
        _reader = reader;
    }

    public async Task RunAsync(CancellationToken token)
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

        while (!token.IsCancellationRequested)
        {
            while (_reader.TryRead(out var cmd))
            {
                if (cmd.Type == CommandType.Set)
                {
                    _buffer.Add($"set {cmd.Key} {cmd.Value}");
                }
            }

            if (_buffer.Count <= 0 || !await timer.WaitForNextTickAsync(token))
            {
                continue;
            }
            await File.AppendAllLinesAsync(_filePath, _buffer, token);
            _buffer.Clear();
        }
    }
}