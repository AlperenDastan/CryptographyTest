using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using CryptographyTest.Models;
using CryptographyTest.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DetectiveApiDbContext>(options =>
        options.UseSqlite("Data Source=detectiveapi.db"));

builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
{
    // Configure password requirements
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false; // Disable the requirement for non-alphanumeric characters
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<DetectiveApiDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddScoped<AuthService>();


// Configure RSA for JWT
var rsa = new RSACryptoServiceProvider(2048);
var signingKey = new RsaSecurityKey(rsa.ExportParameters(true));

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
        ValidIssuer = "CryptographyTest",
        ValidAudience = "CryptographyTestAPI",
        IssuerSigningKey = signingKey
    };
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DetectiveApiDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    await dbContext.Database.MigrateAsync();
    await DbContextExtensions.EnsureSeedDataAsync(dbContext, userManager);
}

app.Run();
