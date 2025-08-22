using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MysteryBox.Api.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // Fallback connection string for migrations/design-time.
        var connStr = Environment.GetEnvironmentVariable("MYSTERYBOX_CONNSTR")
            ?? "Server=localhost;Database=mysterybox;User=root;Password=yourpassword;TreatTinyAsBoolean=false;";

        optionsBuilder.UseMySql(connStr, ServerVersion.AutoDetect(connStr));
        return new AppDbContext(optionsBuilder.Options);
    }
}
