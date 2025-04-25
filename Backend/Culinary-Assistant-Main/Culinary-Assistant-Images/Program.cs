using Core.Minio;
using Core.Serilog;
using Culinary_Assistant.Core.Constants;
using Culinary_Assistant.Core.Options;
using Culinary_Assistant.Core.Shared.Middlewares;
using Culinary_Assistant_Images.Services.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RabbitMQOptions>(builder.Configuration.GetSection(ConfigurationConstants.RabbitMQ));
builder.Services.AddHostedService<UploadsConsumerService>();
builder.Services.AddHostedService<RemoveUploadsConsumerService>();
builder.Services.UseMinioWithFileService(builder.Configuration);
builder.Host.AddSerilog();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
