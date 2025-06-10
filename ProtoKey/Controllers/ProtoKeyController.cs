using System.Text.RegularExpressions;
using System.Threading.Channels;
using Microsoft.AspNetCore.Mvc;
using ProtoKey.Models;

namespace ProtoKey.Controllers;

[ApiController]
[Route("[controller]")]
public class ProtoKeyController : ControllerBase
{
    private static readonly Regex KeyRegex = new("^[a-zA-Z0-9_\\-.]{1,1000}$", RegexOptions.Compiled);
    private readonly Channel<Command> _commandChannel;

    public ProtoKeyController(Channel<Command> commandChannel)
    {
        _commandChannel = commandChannel;
    }

    [HttpPost("set")]
    public async Task<IActionResult> Set([FromQuery] string key, [FromQuery] int value)
    {
        if (!IsValidKey(key))
        {
            return BadRequest("Invalid key");
        }

        var tcs = new TaskCompletionSource<object>();
        var command = new Command(CommandType.Set, key, value, null, tcs);
        await _commandChannel.Writer.WriteAsync(command);
        await tcs.Task;
        return Ok();
    }

    [HttpGet("get")]
    public async Task<IActionResult> Get([FromQuery] string key)
    {
        if (!IsValidKey(key))
        {
            return BadRequest("Invalid key");
        }

        var tcs = new TaskCompletionSource<object>();
        var command = new Command(CommandType.Get, key, null, null, tcs);
        await _commandChannel.Writer.WriteAsync(command);
        var result = await tcs.Task;
        return Ok(result);
    }

    [HttpGet("keys")]
    public async Task<IActionResult> Keys([FromQuery] string? prefix = "")
    {
        if (!string.IsNullOrEmpty(prefix) && !IsValidKey(prefix))
        {
            return BadRequest("Invalid prefix");
        }

        var tcs = new TaskCompletionSource<object>();
        var command = new Command(CommandType.Keys, null!, null, prefix, tcs);
        await _commandChannel.Writer.WriteAsync(command);
        var result = await tcs.Task;
        return Ok(result);
    }

    private bool IsValidKey(string key)
    {
        return KeyRegex.IsMatch(key);
    }
}