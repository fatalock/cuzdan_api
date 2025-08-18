using System.Text;
using Cuzdan.Api.Data;
using Cuzdan.Api.Models;
using Cuzdan.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Cuzdan.Api.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// --- Veritabanı ve Servis Kayıtları ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CuzdanContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllers();

// --- JWT Authentication Ayarları (EN ÖNEMLİ EKSİK KISIM) ---
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

// --- Swagger (OpenAPI) Ayarları ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Güvenlik şemasını OpenAPI 3.0 standardına uygun olarak tanımla
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http, // Tip: Http
        Scheme = "bearer", // Şema: bearer
        BearerFormat = "JWT",
        Description = "Lütfen JWT token'ınızı bu alana girin."
    });

    // Güvenlik gereksinimini tanımla
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer" // Yukarıda tanımlanan şemanın adı
                }
            },
            new List<string>()
        }
    });
});

// --- Uygulamayı İnşa Et ---
var app = builder.Build();

// --- HTTP Request Pipeline Ayarları ---
if (app.Environment.IsDevelopment())
{
    // Doğru Swagger kullanımı bu şekildedir.
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Önce kimliğini doğrula, sonra yetkilerini kontrol et. Sıralama önemli!
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();