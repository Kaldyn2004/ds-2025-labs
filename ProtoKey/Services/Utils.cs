namespace ProtoKey.Services;

public static class Utils
{
    public static void LoadFromDisk(Dictionary<string, int> store, string path = "ProtoKey.data")
    {
        if (!File.Exists(path))
            return;

        foreach (var line in File.ReadAllLines(path))
        {
            var parts = line.Split(' ');
            if (parts.Length != 3 || parts[0] != "set")
            {
                continue;
            }
            var key = parts[1];
            if (int.TryParse(parts[2], out var val))
            {
                store[key] = val;
            }
        }
    }
}