using Core.Minio;
using Core.Serilog;
using Culinary_Assistant.Core.Constants;
using Culinary_Assistant.Core.Options;
using Culinary_Assistant.Core.Shared.Middlewares;
using Culinary_Assistant_Main.Infrastructure;
using Culinary_Assistant_Main.Infrastructure.Mappers;
using Culinary_Assistant_Main.Infrastructure.Startups;
using Culinary_Assistant_Main.Services.Receipts;
using Culinary_Assistant_Main.Services.ReceiptsCollections;
using Culinary_Assistant_Main.Services.Seed;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ConnectionOptions>(builder.Configuration.GetSection(ConfigurationConstants.PostgreSQL));
builder.Services.Configure<RabbitMQOptions>(builder.Configuration.GetSection(ConfigurationConstants.RabbitMQ));
builder.Services.Configure<ElasticSearchOptions>(builder.Configuration.GetSection(ConfigurationConstants.ElasticSearchOptions));

builder.Services.AddCors(setup =>
{
	setup.AddPolicy(ConfigurationConstants.FrontendPolicy, config =>
	{
		config
	   .WithOrigins(builder.Configuration[ConfigurationConstants.FrontendHost]!)
	   .AllowAnyHeader()
	   .AllowAnyMethod()
	   .AllowCredentials();
	});
});

builder.Host.AddSerilog();
builder.Services.AddDbContext<CulinaryAppContext>();
builder.Services.AddAutoMapper(typeof(CulinaryAppMapper).Assembly);
builder.Services.UseMinioWithoutFileService(builder.Configuration);
builder.Services.AddFluentValidationAutoValidation().AddValidatorsFromAssemblyContaining<CulinaryAppContext>();
builder.Services.AddDomain();
builder.Services.AddCustomServices();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(c =>
{
	c.TokenValidationParameters = new TokenValidationParameters()
	{
		ClockSkew = TimeSpan.Zero,
		ValidateAudience = false,
		ValidateIssuer = false,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration[ConfigurationConstants.JWTSecretKey]!))
	};
});
builder.Services.AddProblemDetails();
builder.Services.AddControllers().AddNewtonsoftJson(config =>
{
	config.SerializerSettings.Converters.Add(new StringEnumConverter(typeof(CamelCaseNamingStrategy)));
	config.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
	config.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setup =>
{
	var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	var xmlFilePath = Path.Combine(AppContext.BaseDirectory, xmlFile);
	setup.IncludeXmlComments(xmlFilePath);
	setup.SupportNonNullableReferenceTypes();
});
builder.Services.AddSwaggerGenNewtonsoftSupport();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors(ConfigurationConstants.FrontendPolicy);

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
	var dbContext = scope.ServiceProvider.GetRequiredService<CulinaryAppContext>();
	await dbContext.Database.MigrateAsync();
	var seedService = scope.ServiceProvider.GetRequiredService<ISeedService>();
	await seedService.CreateAdministratorUserAsync();
	var elasticReceiptsService = scope.ServiceProvider.GetRequiredService<IElasticReceiptsService>();
	var elasticCollectionReceiptsService = scope.ServiceProvider.GetRequiredService<IElasticReceiptsCollectionsService>();
	await elasticReceiptsService.CreateReceiptsIndexAsync();
	await elasticCollectionReceiptsService.CreateReceiptsCollectionsIndexAsync();
}

app.Run();
