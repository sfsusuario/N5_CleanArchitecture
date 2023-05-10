using System.Reflection;
using Security.Application.Handlers.CommandHandler;
using Security.Domain.Repositories.Query;
using Security.Domain.Repositories.Query.Base;
using Security.Infrastructure.Repository.Command;
using Security.Infrastructure.Repository.Command.Base;
using Security.Infrastructure.Repository.Query;
using Security.Infrastructure.Repository.Query.Base;
using MediatR;
using Security.Domain.Repositories.Command;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Security.Domain.Contracts.Persistence;
using HR.LeaveManagement.Persistence.Repositories;
using Security.Domain.External.Command;
using Security.Infrastructure.External.Command;
using Serilog;
using Security.Domain.Entities.Config;
using Security.Domain.External.Command.Base;
using Security.Infrastructure.Data;
//using Microsoft.Extensions.DependencyInjection.Abstractions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRazorPages();
builder.Services.AddMvc();

// Configure SQL server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SecurityContext>( options => options.UseSqlServer(connectionString));
builder.Services.Configure<ProjectConfiguration>(builder.Configuration.GetSection(nameof(ProjectConfiguration)));

// Register dependencies
builder.Services.AddMediatR(typeof(ModifyPermissionHandler).GetTypeInfo().Assembly);
builder.Services.AddScoped(typeof(IQueryRepository<>), typeof(QueryRepository<>));
builder.Services.AddScoped(typeof(ICommandExternal<>), typeof(CommandRepository<>));

// Configure unit of work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IPermissionsQueryRepository, PermissionsQueryRepository>();
builder.Services.AddTransient<IPermissionsCommandRepository, PermissionsCommandRepository>();

/// Configure Kafka
builder.Services.AddTransient<IKafkaCommandExternal, KafkaCommandExternal>();
builder.Services.AddTransient<IElasticSearchCommandExternal, ElasticSearchCommandExternal>();

using var log = new LoggerConfiguration() //new
    .WriteTo.Console()
    .CreateLogger();
Log.Logger = log; //new 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseFileServer(new FileServerOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "dist")),
    RequestPath = "/dist",
    EnableDirectoryBrowsing = true
});

app.MapControllers();
app.MapRazorPages();
app.UseStaticFiles();

app.Run();

