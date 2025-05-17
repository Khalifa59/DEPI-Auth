using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using UserAuthenticationAPI.Services;
using UserAuthenticationAPI.Services.Interfaces;
using UserAuthenticationAPI.Data;
using Swashbuckle.AspNetCore.SwaggerGen;
using UserAuthenticationAPI.Models;
using UserAuthenticationAPI.Models.AuthModels;  // Add this line to import LoginRequest

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddSingleton<InMemoryUserRepository>();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "User Authentication API", Version = "v1" });

    // Add description for the Username field in LoginRequest
    c.SchemaFilter<LoginRequestSchemaFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Authentication API v1");
        c.RoutePrefix = string.Empty; // Serve the Swagger UI at the app's root
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Add this class somewhere in your project
public class LoginRequestSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(LoginRequest))
        {
            if (schema.Properties.TryGetValue("username", out var usernameProperty))
            {
                usernameProperty.Description = "Enter either your username or email address";
            }
        }
    }
}