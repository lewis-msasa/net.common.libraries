
using Common.Libraries.Services.Email;
using Common.Libraries.Services.Repositories;
using Common.Libraries.Services.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.RegisterEmailServices(builder.Configuration, "EmailConfiguration");
builder.Services.AddSingleton<EmailNotification>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/tryEmail", async (
	EmailNotification emailNotification
	) =>
{
	string message = $"Your keys have been generated on NovaPay";
	try
	{
		await emailNotification.SendNotification(new EmailMessage(message,
			new string[] { "lewis_msasa@berkeley.edu" },
			"Merchant Keys"));
	}
	catch (Exception ex)
	{
		string s = ex.Message;
	}
	return Results.Ok();
})
.WithName("Email")
.WithOpenApi();




app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
