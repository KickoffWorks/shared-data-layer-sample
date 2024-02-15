using Sample.Api.Endpoints;

namespace Sample.Api;

public static class Configuration
{
    public static void RegisterServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<Sample.Data.ApplicationDbContext>();
        builder.Services.AddScoped(typeof(Sample.Data.Repositories.IRepository<>), typeof(Sample.Data.Repositories.Repository<>));
    }
    
    public static void RegisterMiddlewares(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        
        app.MapGroup("/products").MapProductEndpoints();
    }
}