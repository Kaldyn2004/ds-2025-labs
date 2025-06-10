using System.Text.Json;

namespace ProtoCli;

class Program
{
    private const string ProtoKeyHost = "http://localhost:5014";
    private static readonly HttpClient Client = new();

    static async Task<int> Main(string[] args)
    {
        ShowUsage();

        while (true)
        {
            Console.Write("> ");
            var input = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input))
                continue;

            var commandParts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var command = commandParts[0].ToLower();

            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                break;

            switch (command)
            {
                case "exit":
                    break;
                case "help":
                     ShowUsage();
                     break;
                case "set":
                    await HandleSet(commandParts);
                    break;
                case "get":
                    await HandleGet(commandParts);
                    break;
                case "keys":
                    await HandleKeys(commandParts);
                    break;
                default:
                    Error("Unknown command");
                    break;
            }
        }
        return 0;

        try
        {

        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync($"Error: {ex.Message}");
            return 1;
        }
    }

    private static int Error(string message)
    {
        Console.Error.WriteLine(message);
        return 1;
    }

    private static int ShowUsage()
    {
        Console.WriteLine("Available commands: set <key> <value>, get <key>,  keys <prefix>, exit");
        Console.WriteLine("Type 'help' for information");
        Console.WriteLine("Type 'exit' for exit");
        return 1;
    }

    private static async Task<int> HandleSet(string[] args)
    {
        if (args.Length != 3)
        {
            return Error("Usage: set <key> <value>");
        }

        string key = args[1];
        if (!int.TryParse(args[2], out int value))
        {
            return Error("Value must be a 32-bit integer");
        }

        var url = $"{ProtoKeyHost}/protoKey/set?key={Uri.EscapeDataString(key)}&value={value}";
        var response = await Client.PostAsync(url, null);
        Console.WriteLine($"Response: {response.StatusCode}");

        return 0;
    }

    private static async Task<int> HandleGet(string[] args)
    {
        if (args.Length != 2)
        {
            return Error("Usage: get <key>");
        }

        string key = args[1];
        var url = $"{ProtoKeyHost}/protoKey/get?key={Uri.EscapeDataString(key)}";
        var response = await Client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine(content);

        return 0;
    }

    private static async Task<int> HandleKeys(string[] args)
    {
        string prefix = args.Length == 2 ? args[1] : string.Empty;
        var url = $"{ProtoKeyHost}/protoKey/keys?prefix={Uri.EscapeDataString(prefix)}";
        var response = await Client.GetAsync(url);
        var json = await response.Content.ReadAsStringAsync();

        var keys = JsonSerializer.Deserialize<List<string>>(json);
        if (keys != null)
        {
            foreach (var key in keys)
            {
                Console.WriteLine(key);
            }
        }

        return 0;
    }
}