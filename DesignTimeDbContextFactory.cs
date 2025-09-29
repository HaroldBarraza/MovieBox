using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MovieBox.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        
        // Tu connection string convertida de Railway
        var connectionString = "Host=maglev.proxy.rlwy.net;Port=31543;Database=railway;Username=postgres;Password=RZoFDldrPfTiaSwKojxDeTXADUWRSUNE";
        
        optionsBuilder.UseNpgsql(connectionString);
        
        return new AppDbContext(optionsBuilder.Options);
    }
}