using CsvHelper;
using System.Globalization;

namespace MysteryBox.Api.Services;

public class ResourceLoader
{
    private readonly IWebHostEnvironment _env;
    public ResourceLoader(IWebHostEnvironment env) => _env = env;

    public IEnumerable<T> LoadCsv<T>(string csvFile, Func<string[], T> map, bool skipHeader = true)
    {
        var path = Path.Combine(_env.ContentRootPath, "ResourceDB", "CSV", csvFile);
        if (!File.Exists(path)) yield break;
        using var reader = new StreamReader(path);
        string? line;
        if (skipHeader) reader.ReadLine();
        while ((line = reader.ReadLine()) != null)
        {
            var parts = line.Split(',');
            yield return map(parts);
        }
    }
}
