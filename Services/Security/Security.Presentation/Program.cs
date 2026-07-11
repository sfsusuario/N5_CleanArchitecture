using System.Reflection;
using FluentValidation;
using MediatR;
using Security.Application.Behaviours;
using Security.Application.Handlers.CommandHandler;
using Security.Application.Validators;
using Security.Domain.Repositories.Query;
using Security.Domain.Repositories.Query.Base;
using Security.Infrastructure.Repository.Command;
using Security.Infrastructure.Repository.Command.Base;
using Security.Infrastructure.Repository.Query;
using Security.Infrastructure.Repository.Query.Base;
using Security.Domain.Repositories.Command;
using Microsoft.EntityFrameworkCore;
using Security.Domain.Contracts.Persistence;
using Security.Infrastructure.Repository;
using Security.Domain.External.Command;
using Security.Infrastructure.External.Command;
using Serilog;
using Security.Domain.Entities.Config;
using Security.Domain.External.Command.Base;
using Security.Infrastructure.BackgroundServices;
using Security.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Route ASP.NET Core's built-in ILogger<T> (used by the controller) through Serilog.
builder.Host.UseSerilog((context, configuration) => configuration.WriteTo.Console());

// Add services to the container.

const string FrontendCorsPolicy = "FrontendCorsPolicy";
builder.Services.AddCors(options =>
{
    // Security.Frontend runs in the browser (not inside the Docker network), reaching this API
    // via its host-mapped port — hence localhost origins here, not Docker service hostnames.
    // Covers both `npm run dev` (Vite's default port) and the dockerized build (docker-compose
    // maps it to 3000).
    options.AddPolicy(FrontendCorsPolicy, policy =>
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SecurityContext>( options => options.UseNpgsql(connectionString));
builder.Services.Configure<ProjectConfiguration>(builder.Configuration.GetSection(nameof(ProjectConfiguration)));

// Register dependencies
builder.Services.AddMediatR(typeof(ModifyPermissionHandler).GetTypeInfo().Assembly);
builder.Services.AddScoped(typeof(IQueryRepository<>), typeof(QueryRepository<>));
builder.Services.AddScoped(typeof(ICommandExternal<>), typeof(CommandRepository<>));

// FluentValidation: validators run as a MediatR pipeline behaviour before each handler.
builder.Services.AddValidatorsFromAssemblyContaining<RequestPermissionCommandValidator>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

// Configure unit of work
// Note: IPermissionsQueryRepository/IPermissionsCommandRepository below are never actually
// resolved through the container — UnitOfWork builds them itself with `new` (lazily, on first
// access) so that both repositories share the same SecurityContext instance/transaction. They
// stay registered here only in case something outside UnitOfWork needs them directly.
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IPermissionsQueryRepository, PermissionsQueryRepository>();
builder.Services.AddTransient<IPermissionsCommandRepository, PermissionsCommandRepository>();

// Kafka/Elasticsearch clients open a real network connection in their constructor
// (Kafka producer, Elasticsearch index check), so they must live once per process.
builder.Services.AddSingleton<IKafkaCommandExternal, KafkaCommandExternal>();
builder.Services.AddSingleton<IElasticSearchCommandExternal, ElasticSearchCommandExternal>();

// Outbox pattern dispatcher: delivers the rows RequestPermissionHandler/ModifyPermissionHandler
// write to Kafka/Elasticsearch in the background, decoupling their availability from the
// request/response cycle.
builder.Services.AddHostedService<OutboxDispatcherService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Safety net for anything the per-action try/catch blocks don't cover.
app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
{
    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    context.Response.ContentType = "application/json";
    await context.Response.WriteAsync("{\"error\":\"Internal server error\"}");
}));

app.UseHttpsRedirection();

app.UseCors(FrontendCorsPolicy);

// Placeholder only: no authentication scheme is configured (no UseAuthentication(),
// no [Authorize] on any controller), so this middleware is currently a no-op.
// See DOCUMENTATION.md "Limitaciones conocidas" for the real-auth recommendation.
app.UseAuthorization();

app.MapControllers();

app.Run();

