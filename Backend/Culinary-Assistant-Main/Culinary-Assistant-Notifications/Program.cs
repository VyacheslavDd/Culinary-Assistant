using Core.Serilog;
using Culinary_Assistant.Core.Constants;
using Culinary_Assistant.Core.Email;
using Culinary_Assistant.Core.Options;
using Culinary_Assistant.Core.Shared.Middlewares;
using Culinary_Assistant_Notifications.Startups;
using Culinary_Assistant_Notifications_Infrastructure;
using Culinary_Assistant_Notifications_Services.PasswordRecoverService;
using Culinary_Assistant_Notifications_Services.PasswordRecoversService;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Quartz.AspNetCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ConnectionOptions>(builder.Configuration.GetSection(ConfigurationConstants.PostgreSQL));
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection(ConfigurationConstants.EmailOptions));
builder.Services.AddQuartzAndJobs();
builder.Services.AddQuartzHostedService(c =>
{
	c.AwaitApplicationStarted = true;
	c.WaitForJobsToComplete = true;
});
builder.Services.AddCors(s =>
{
	s.AddPolicy(ConfigurationConstants.FrontendPolicy, c =>
	{
		c.WithOrigins(builder.Configuration[ConfigurationConstants.FrontendHost]!, builder.Configuration[ConfigurationConstants.FrontendVMHost]!)
		 .AllowCredentials()
		 .AllowAnyHeader()
		 .AllowAnyMethod();
	});
});

builder.Services.AddDbContext<NotificationsContext>();
builder.Services.AddScoped<IPasswordRecoversService, PasswordRecoversService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddControllers();
builder.Host.AddSerilog();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(s =>
{
	var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	var xmlFilePath = Path.Combine(AppContext.BaseDirectory, xmlFile);
	s.IncludeXmlComments(xmlFilePath);
});

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
	var dbContext = scope.ServiceProvider.GetRequiredService<NotificationsContext>();
	await dbContext.Database.MigrateAsync();
}

app.Run();
