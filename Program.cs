using System.Text;
using System.Threading.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using AvalWebBackend.Infrastructure.Persistence;
using AvalWebBackend.Application.Services;
using AvalWebBackend.Application.Common.Interfaces;
using AvalWebBackend.Infrastructure.Filters;
using AvalWebBackend.Infrastructure.Middleware;
using AvalWebBackend.Infrastructure.Services;
using AvalWebBackend.Infrastructure.Settings;

var builder = WebApplication.CreateBuilder(args);

// ---------- JWT Settings ----------
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// ---------- Authentication & Authorization ----------
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.Request.Cookies["access_token"];
            if (!string.IsNullOrEmpty(token))
                context.Token = token;
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();
builder.Services.AddOpenApi();

// ---------- Antiforgery (FIXED) ----------
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.Cookie.Name = "XSRF-TOKEN";
    options.Cookie.HttpOnly = false;   // Must be readable by JavaScript on the frontend

    // SameSite=None + Secure required for cross-origin requests
    options.Cookie.SameSite = SameSiteMode.None;
    // Your locally running API uses HTTPS redirection, so Secure is valid
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// ---------- Database  ----------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=Infrastructure/avaldb.db;Foreign Keys=True"));

// ---------- Repositories ----------
builder.Services.AddScoped<IUserRepository, EfUserRepository>();
builder.Services.AddScoped<ITicketRepository, EfTicketRepository>();
builder.Services.AddScoped<IMessageRepository, EfMessageRepository>();
builder.Services.AddScoped<IStatsCacheRepository, EfStatsCacheRepository>();
builder.Services.AddScoped<IServiceRepository, EfServiceRepository>();
builder.Services.AddScoped<ITransactionRepository, EfTransactionRepository>();

// ---------- Application Services ----------
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IFinancialStatsService, FinancialStatsService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();

// ---------- Rate Limiting ----------
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";
        var json = System.Text.Json.JsonSerializer.Serialize(new { message = "Too many requests. Please try again later." });
        await context.HttpContext.Response.WriteAsync(json, cancellationToken);
    };

    options.AddFixedWindowLimiter("LoginPolicy", config =>
    {
        config.PermitLimit = 5;
        config.Window = TimeSpan.FromMinutes(1);
        config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        config.QueueLimit = 0;
    });

    options.AddFixedWindowLimiter("BatchPolicy", config =>
    {
        config.PermitLimit = 10;
        config.Window = TimeSpan.FromMinutes(1);
        config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        config.QueueLimit = 0;
    });

    options.AddFixedWindowLimiter("RegisterPolicy", config =>
    {
        config.PermitLimit = 3;
        config.Window = TimeSpan.FromMinutes(1);
        config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        config.QueueLimit = 0;
    });
});

// ---------- Request Size Limit ----------
builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = 10 * 1024 * 1024);

// ---------- Controllers & Filters ----------
builder.Services.AddControllers(options =>
{
    options.Filters.Add<DomainExceptionFilter>();
});

// ---------- CORS ----------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
        policy.WithOrigins("http://localhost:5173", "http://localhost:5175")
              .AllowCredentials()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

// ---------- Middleware Pipeline ----------
app.UseRouting();
app.UseCors("AllowReact");
app.UseRateLimiter();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();