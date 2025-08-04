using Common.Libraries.EventSourcing;
using Common.Libraries.EventStore.EF.TestApi;
using Common.Libraries.EventStore.Projection;
using Common.Libraries.Services.EFCore.Repositories;
using Common.Libraries.Services.Repositories;
using Common.Libraries.Services.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
   
});

builder.Services.AddEventStoreEFPersistence("TestProjection");
builder.Services.AddScoped<IProjectorProvider<UserDetails>, UserDetailsProjectorProvider>();
builder.Services.AddScoped<ISubscription<UserDetails>,EFDbProjection<UserDetails>>();
builder.Services.AddHostedService<ProjectionWorker<UserDetails>>();
builder.Services.AddDbContext<TestDBContext>(
          opts => opts.UseMySql(builder.Configuration["ConnectionStrings:TestDB"],
          ServerVersion.AutoDetect(builder.Configuration["ConnectionStrings:TestDB"])));
builder.Services.AddScoped<ApplicationService<User>, UserCommandService>();
builder.Services.AddScoped<IRepository<UserDetails>, EFRepository<UserDetails, TestDBContext>>();
builder.Services.AddScoped<IService<UserDetails, UserDetailsDto>, Service<UserDetails, UserDetailsDto>>();

builder.Services.AddScoped<IRepository<Event>, EFRepository<Event, TestDBContext>>();
builder.Services.AddScoped<IService<Event, EventDto>, Service<Event, EventDto>>();

builder.Services.AddScoped<IRepository<Checkpoint>, EFRepository<Checkpoint, TestDBContext>>();
builder.Services.AddScoped<IService<Checkpoint, CheckpointDto>, Service<Checkpoint, CheckpointDto>>();


builder.Services.AddScoped<IRepository<Snapshot>, EFRepository<Snapshot, TestDBContext>>();
builder.Services.AddScoped<IService<Snapshot, SnapshotDto>, Service<Snapshot, SnapshotDto>>();

builder.Services.AddSingleton<IEventDeserializer>(sp =>
    new EventDeserializer(
    [
        typeof(UserCreated),
        typeof(UsernameChanged)
    ]));

var app = builder.Build();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
