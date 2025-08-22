using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Exceptions;

namespace MysteryBox.Api.Services;

public class GeoIpService
{
    private readonly string _dbPath;
    public GeoIpService(IWebHostEnvironment env)
    {
        _dbPath = Path.Combine(env.ContentRootPath, "GeoLite2-City.mmdb");
    }

    public string? TryGetCountryIso(string ip)
    {
        if (!File.Exists(_dbPath)) return null;
        try
        {
            using var reader = new DatabaseReader(_dbPath);
            var resp = reader.City(ip);
            return resp?.Country?.IsoCode;
        }
        catch (AddressNotFoundException)
        {
            return null;
        }
        catch
        {
            return null;
        }
    }
}
