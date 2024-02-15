namespace Sample.Api.Endpoints;

public static class EndpointExtensions
{
    public static void MapProductEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/{id}", GetProduct);
        
        group.MapPost("/", CreateProduct);
    }

    private static async Task<IResult> GetProduct(Guid id, Sample.Data.Repositories.IRepository<Sample.Data.Entities.Products> products)
    {
        var product = await products.GetByIdAsync(id);

        if (product is null) return Results.NotFound();

        return Results.Ok(new DTOs.ProductDTO(product.Id, product.Name));
    }
    
    private static async Task<IResult> CreateProduct(string name, Data.Repositories.IRepository<Sample.Data.Entities.Products> products)
    {
        var addedProduct = new Sample.Data.Entities.Products()
        {
            Name = name
        };

        var result = await products.AddAsync(addedProduct);

        return Results.Ok(result);
    }
    
    
}