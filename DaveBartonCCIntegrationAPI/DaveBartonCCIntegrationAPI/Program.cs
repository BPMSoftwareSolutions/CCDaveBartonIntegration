using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DaveBartonCCIntegrationAPI.DataAccess;
using DaveBartonCCIntegrationAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Code to retrieve secret from Azure Key Vault
var kvUri = builder.Configuration["KeyVault:KeyVaultURL"];
var kvClientId = builder.Configuration["KeyVault:ClientId"];
var kvSecret = builder.Configuration["KeyVault:ClientSecret"];
var kvDirectoryId = builder.Configuration["KeyVault:DirectoryId"];

// Create ClientSecretCredential
var kvCredential = new ClientSecretCredential(kvDirectoryId, kvClientId, kvSecret);

// Add Azure Key Vault to Configuration
builder.Configuration.AddAzureKeyVault(new Uri(kvUri), kvCredential);

var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());

KeyVaultSecret jwtSecret = client.GetSecret("JwtSecret");
KeyVaultSecret aesKeyScecret = client.GetSecret("AESKey");

var jwtSecretKey = jwtSecret.Value;
KeyVaultSecret connectionSecret = client.GetSecret("ConnectionString");
var connectionString = connectionSecret.Value;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
            ValidateIssuer = false, // Set to true and specify Issuer if needed
            ValidateAudience = false, // Set to true and specify Audience if needed
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Add SqlServerRepository to the services collection
builder.Services.AddScoped<ServiceScope>(serviceProvider =>
{
    // Assuming the connection string is stored in appsettings.json
    return new ServiceScope()
    {
        Repository = new SqlServerRepository(connectionString, aesKeyScecret.Value),
        ClientSecret = jwtSecret.Value,
        AESKey = aesKeyScecret.Value,
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
