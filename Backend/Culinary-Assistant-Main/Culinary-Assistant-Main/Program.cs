using Core.Serilog;
using Culinary_Assistant.Core.Constants;
using Culinary_Assistant.Core.Options;
using Culinary_Assistant.Core.Shared.Middlewares;
using Culinary_Assistant_Main.Infrastructure;
using Culinary_Assistant_Main.Infrastructure.Startups;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ConnectionOptions>(builder.Configuration.GetSection(ConfigurationConstants.PostgreSQL));
builder.Services.Configure<RabbitMQOptions>(builder.Configuration.GetSection(ConfigurationConstants.RabbitMQ));

builder.Host.AddSerilog();
builder.Services.AddDbContext<CulinaryAppContext>();
builder.Services.AddCustomServices();
builder.Services.AddProblemDetails();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setup =>
{
	var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	var xmlFilePath = Path.Combine(AppContext.BaseDirectory, xmlFile);
	setup.IncludeXmlComments(xmlFilePath);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
	var dbContext = scope.ServiceProvider.GetRequiredService<CulinaryAppContext>();
	dbContext.Database.Migrate();
}

app.Run();
