using LibrarySystemG.API.Class;
using LibrarySystemG.API.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ===================== SERVICES =====================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ===================== CORS =====================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ===================== DEPENDENCY INJECTION =====================
builder.Services.AddScoped<ILoginRepository, LoginClass>();
builder.Services.AddScoped<IRegisterRepository, RegisterClass>();
builder.Services.AddScoped<IAdminRepository, AdminClass>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddSingleton<IBookRepository, BookClass>();
builder.Services.AddSingleton<ITransactionRepository, TransactionClass>();

// ===================== JWT =====================
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

// ===================== BIND TO ALL INTERFACES =====================
builder.WebHost.UseUrls("http://0.0.0.0:5000");

var app = builder.Build();

// ===================== PIPELINE (CORRECT ORDER) =====================
app.UseCors("AllowAll");        // ✅ #1 FIRST - always before everything

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();