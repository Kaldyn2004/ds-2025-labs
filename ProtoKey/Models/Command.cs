namespace ProtoKey.Models;

public enum CommandType
{
    Set,
    Get,
    Keys
}

public record Command(
    CommandType Type,
    string Key,
    int? Value,
    string? Prefix,
    TaskCompletionSource<object> Completion
);