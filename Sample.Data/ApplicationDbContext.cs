using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sample.Data.Entities;

namespace Sample.Data;

public class ApplicationDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    
    // If you set the database connection string in your project's appsettings and Program.cs uncomment the lines below, for the DbContextOptions constructor.
    
    // If not, keep the parameterless constructor.

    // public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    // {
    //     
    // }
    
    public ApplicationDbContext()
    {
        // Feel free to use the dotnet/aspnet environment variable OR the hosting environment. I will keep the env var.
        
        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-8.0
        
        // string environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";
        string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .Build();
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // If it is already configured from your project's Program.cs it leaves.
        if (optionsBuilder.IsConfigured) return;
        
        // If it is not configured, we need to get the connection string from somewhere
        
        var connectionString = _configuration.GetConnectionString("Database")!;
        optionsBuilder.UseNpgsql(connectionString);
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // This is all optional, just some quality of life methods that might be useful for Postgres specifically.
        
        // builder.HasDefaultSchema("sample");
        
        // RenameIdentityTables(builder);
        RenamePostgresTables(builder);
    }
    
    // If you're using DbIdentityContext / Identity API uncomment above and below.

    // private static void RenameIdentityTables(ModelBuilder builder)
    // {
    //     builder.Entity<IdentityUser>(b =>
    //     {
    //         b.ToTable("users");
    //     });
    //
    //     builder.Entity<IdentityUserClaim<string>>(b =>
    //     {
    //         b.ToTable("user_claims");
    //     });
    //
    //     builder.Entity<IdentityUserLogin<string>>(b =>
    //     {
    //         b.ToTable("user_logins");
    //     });
    //
    //     builder.Entity<IdentityUserToken<string>>(b =>
    //     {
    //         b.ToTable("user_tokens");
    //     });
    //
    //     builder.Entity<IdentityRole>(b =>
    //     {
    //         b.ToTable("roles");
    //     });
    //
    //     builder.Entity<IdentityRoleClaim<string>>(b =>
    //     {
    //         b.ToTable("role_claims");
    //     });
    //
    //     builder.Entity<IdentityUserRole<string>>(b =>
    //     {
    //         b.ToTable("user_roles");
    //     });
    // }

    private static void RenamePostgresTables(ModelBuilder builder)
    {
        foreach(var entity in builder.Model.GetEntityTypes())
        {
            entity.SetTableName(ToSnakeCase(entity.GetTableName()!));
                
            foreach(var property in entity.GetProperties())
            {
                property.SetColumnName(ToSnakeCase(property.GetColumnName()!));
            }

            foreach(var key in entity.GetKeys())
            {
                key.SetName(ToSnakeCase(key.GetName()!));
            }

            foreach(var key in entity.GetForeignKeys())
            {
                key.SetConstraintName(ToSnakeCase(key.GetConstraintName()!));
            }

            foreach(var index in entity.GetIndexes())
            {
                index.SetDatabaseName(ToSnakeCase(index.GetDatabaseName()!));
            }
        }
    }
    
    private static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input)) { return input; }

        var startUnderscores = Regex.Match(input, @"^_+");
        return startUnderscores + Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
    }
    
    public DbSet<Products> Products { get; set; }
}
