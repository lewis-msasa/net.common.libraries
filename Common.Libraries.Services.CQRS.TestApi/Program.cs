using Common.Libraries.Services.CQRS;
using Common.Libraries.Services.CQRS.Notification;
using Common.Libraries.Services.CQRS.PipelineBehaviors;
using Common.Libraries.Services.CQRS.TestApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{

});

builder.Services.AddMemoryCache();
//builder.Services.AddScoped<IDispatcher, Dispatcher>();
//builder.Services.AddScoped<IRequestHandler<CreateOrderCommand, Guid>, CreateOrderHandler>();
//builder.Services.AddScoped<IRequestHandler<GetOrderByIdQuery, OrderDto>, GetOrderByIdHandler>();
builder.Services.AddRequestHandlers([typeof(CreateOrderHandler).Assembly]);
builder.Services.AddNotificationHandlers([typeof(SendOrderEmailHandler).Assembly]);

//builder.Services.RegisterCORSBehaviorsServices();
builder.Services.AddPipelines([]);
builder.Services.AddNotificationPipelines([]);

builder.Services.AddScoped<ICacheStrategy<GetOrderByIdQuery, OrderDto>, GetOrderByIdCache>();
builder.Services.AddScoped<IMetricsStrategy<GetOrderByIdQuery, OrderDto>, BasicMetrics<GetOrderByIdQuery, OrderDto>>();
builder.Services.AddScoped<IPermissionStrategy<CreateOrderCommand>, CreateOrderPermissionStrategy>();
builder.Services.AddScoped<IAuditStrategy<CreateOrderCommand>, CreateOrderAuditStrategy>();



var app = builder.Build();

app.MapPost("/orders", async (CreateOrderCommand command, IDispatcher dispatcher, INotificationDispatcher notificationDispatcher) =>
{
    var orderId = await dispatcher.SendWithPipelines(command); //.Send(command);
    await notificationDispatcher.Publish(new OrderCreatedNotification(orderId, "lmsasajnr@gmail.com"));
    return Results.Ok(orderId);
});

app.MapGet("/orders/{id:guid}", async (Guid id, IDispatcher dispatcher) =>
{
    var query = new GetOrderByIdQuery(id);
    var order = await dispatcher.Send(query);
    return Results.Ok(order);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



app.Run();


