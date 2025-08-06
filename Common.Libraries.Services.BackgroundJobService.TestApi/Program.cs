using Common.Libraries.Services.BackgroundJobService.TestApi;
using Common.Libraries.Services.BackgroundWork;
using Common.Libraries.Services.BackgroundWork.Hangfire;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{

});

builder.Services.AddHangfireDependencies(builder.Configuration, true);

var app = builder.Build();

app.MapPost("/run-report", (IBackgroundJobService jobService) =>
{
    jobService.Enqueue<ReportService>(svc => svc.GenerateDailyReport(CancellationToken.None));
    return Results.Ok("Report enqueued.");
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


