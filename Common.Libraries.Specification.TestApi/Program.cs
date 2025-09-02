using Common.Libraries.Services.Specification;
using Common.Libraries.Services.Specification.EFCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{

});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("TestDb"));

builder.Services.AddEFRepositoriesDependencies(new[] { Assembly.GetExecutingAssembly() }, typeof(AppDbContext));
builder.Services.AddEFUnitOfWorkDependencies(typeof(AppDbContext));
builder.Services.AddEFServicesDependencies(new[] { Assembly.GetExecutingAssembly() }, typeof(AppDbContext));
builder.Services.AddScoped<IEntityMapper<Product, ProductDto>, ProductMapper>();


var app = builder.Build();

app.MapPost("/product", async (CreateProductCommand command, IService<Product,ProductDto> productService) =>
{
    var result = await productService.CreateAsync(new Product { Name = command.Name });
    return Results.Ok(result);
});
app.MapGet("/products", async (IService<Product, ProductDto> productService) =>
{
    var results = await productService.GetAsync();  
    return Results.Ok(results);
});
app.MapGet("/products/{id:int}", async (int id,IService<Product, ProductDto> productService) =>
{
    var productById = new Specification<Product>()
    .Where(c => c.Id == id).And(new Specification<Product>().Where( c => c.CreatedAt < DateTimeOffset.Now));
    var results = await productService.GetOneAsync(productById);
    return Results.Ok(results);
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public record CreateProductCommand(string Name);

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
}

public class Product : ISpecEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
public class ProductDto : ISpecDto
{
   
    public string Name { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
public class ProductMapper : IEntityMapper<Product, ProductDto>
{
    public ProductDto Map(Product entity)
    {
        return new ProductDto
        {
            Name = entity.Name
        };
    }

    public ICollection<ProductDto> Map(ICollection<Product> entities)
    {
        return entities.Select(Map).ToList();
    }
}

