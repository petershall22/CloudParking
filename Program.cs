using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swagger =>
{
    swagger.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "Cloud Parking API",
            Version = "v1",
            TermsOfService = new Uri("https://example.com/terms"),
            Contact = new OpenApiContact
            {
                Email = "test@test.com",
            }
        });

    // Enabled OAuth security in Swagger
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement() {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                },
                Scheme = "oauth2",
                Name = "oauth2",
                In = ParameterLocation.Header
            },
            new List <string> ()
        }
    });
    swagger.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            Implicit = new OpenApiOAuthFlow()
            {
                AuthorizationUrl = new Uri("https://parkingusers.ciamlogin.com/0f32278d-31be-4bb0-a690-9a744ed41d0c/oauth2/v2.0/authorize"),
                TokenUrl = new Uri("https://parkingusers.ciamlogin.com/0f32278d-31be-4bb0-a690-9a744ed41d0c/oauth2/v2.0/token"),
                Scopes = new Dictionary<string, string>() { { "api://6e64ffb6-c9ee-46b3-a146-4f0682f203cc/api", "API Read" } }
            }
        }
    });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("Entra"));
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
app.UseSwagger();
    app.UseSwaggerUI(
        options =>
        {
            options.OAuthAppName("Swagger Client");
            options.OAuthClientId(builder.Configuration.GetSection("Entra")["ClientId"]);
            options.OAuthClientSecret(Environment.GetEnvironmentVariable("ParkingClientSecret"));
            options.OAuthUseBasicAuthenticationWithAccessCodeGrant();
        }
        );
}

string? connectionUri = Environment.GetEnvironmentVariable("MONGODB_URI", EnvironmentVariableTarget.User); 
if (connectionUri == null)
{
    Console.WriteLine("MONGODB_URI not set.");
    Environment.Exit(0);
}

var settings = MongoClientSettings.FromConnectionString(connectionUri);

// Set the ServerApi field of the settings object to set the version of the Stable API on the client
settings.ServerApi = new ServerApi(ServerApiVersion.V1);

// Create a new client and connect to the server
var client = new MongoClient(settings);

// Send a ping to confirm a successful connection
try
{
    var result = client.GetDatabase("admin").RunCommand<BsonDocument>(new BsonDocument("ping", 1));
    Console.WriteLine("Pinged your deployment. You successfully connected to MongoDB!");
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
