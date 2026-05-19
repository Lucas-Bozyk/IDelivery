namespace IDelivery.Infrastructure.Config;

public static class DotEnvLoader
{
    public static void Load(string path)
    {
        if (!File.Exists(path)) return;
        foreach (var raw in File.ReadAllLines(path))
        {
            var line = raw.Trim();
            if (line.Length == 0 || line.StartsWith('#')) continue;
            var idx = line.IndexOf('=');
            if (idx <= 0) continue;
            var key = line[..idx].Trim();
            var value = line[(idx + 1)..].Trim().Trim('"');
            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(key)))
                Environment.SetEnvironmentVariable(key, value);
        }
    }
}
