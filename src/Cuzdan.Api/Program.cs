using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Cuzdan.Api.Handlers;
using Cuzdan.Infrastructure.Data;
using Cuzdan.Application.Interfaces;
using Cuzdan.Application.Services;
using Cuzdan.Infrastructure.Repositories;
using Cuzdan.Infrastructure.Authentication;
using Cuzdan.Infrastructure.Gateways;
using Npgsql;
using Cuzdan.Domain.Enums;
using System.Text.Json.Serialization;
using FluentValidation;
using Cuzdan.Application.Validators;
using Serilog;
using Cuzdan.Application.Decorators;
using Cuzdan.Api.Filters;
using Microsoft.AspNetCore.Mvc;
using FluentValidation.AspNetCore;
using CorrelationId.DependencyInjection;
using CorrelationId;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .Enrich.FromLogContext()
    .Enrich.WithCorrelationId()
    .CreateBootstrapLogger();
try
{
    Log.Information("Application Starting...");

    var builder = WebApplication.CreateBuilder(args);



    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

    dataSourceBuilder.MapEnum<UserRole>("user_role");
    dataSourceBuilder.MapEnum<CurrencyType>("currency_type");
    dataSourceBuilder.MapEnum<TransactionStatus>("transaction_status");
    dataSourceBuilder.MapEnum<TransactionType>("transaction_type");

    var dataSource = dataSourceBuilder.Build();

    builder.Services.AddDbContext<CuzdanContext>(options =>
            options.UseNpgsql(
            dataSource,
            o =>
            {
                o.MapEnum<CurrencyType>("currency_type");
                o.MapEnum<TransactionStatus>("transaction_status");
                o.MapEnum<TransactionType>("transaction_type");
                o.MapEnum<UserRole>("user_role");
            }
        )
    );

    builder.Host.UseSerilog();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddDefaultCorrelationId();

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.Decorate<IAuthService, LoggingAuthServiceDecorator>();

    builder.Services.AddScoped<IWalletService, WalletService>();
    builder.Services.Decorate<IWalletService, LoggingWalletServiceDecorator>();

    builder.Services.AddScoped<ITransactionService, TransactionService>();
    builder.Services.Decorate<ITransactionService, LoggingTransactionServiceDecorator>();

    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.Decorate<IUserService, LoggingUserServiceDecorator>();

    builder.Services.AddScoped<IAdminService, AdminService>();
    builder.Services.Decorate<IAdminService, LoggingAdminServiceDecorator>();

    builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
    builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
    builder.Services.AddScoped<IWalletRepository, WalletRepository>();
    builder.Services.AddScoped<IPaymentGatewayService, PaymentGatewayService>();
    builder.Services.AddHttpClient<ICurrencyConversionService, CurrencyConversionService>(client =>
    {
        client.BaseAddress = new Uri("https://openexchangerates.org");
    });

    builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();


    builder.Services.AddMemoryCache();
    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<AsyncValidationFilter>();
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
    builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserDtoValidator>();




    builder.Services.AddHttpClient();

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                    .GetBytes(builder.Configuration.GetSection("Jwt:Key").Value!)),
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration.GetSection("Jwt:Issuer").Value,
                ValidateAudience = true,
                ValidAudience = builder.Configuration.GetSection("Jwt:Audience").Value
            };
        });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Please enter your JWT token in this field."
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
        });
    });
    builder.Services.AddProblemDetails();
    var app = builder.Build();

    app.UseExceptionHandler();
    app.UseCorrelationId();
    app.UseSerilogRequestLogging();


    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    Log.Information("Application is Running");
    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.Information("Application is shutting down...");
    Log.CloseAndFlush();
}
public partial class Program { }