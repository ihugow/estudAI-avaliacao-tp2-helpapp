using StockApp.Infra.IoC;
using Microsoft.OpenApi.Models;
using StockApp.API.Middlewares;
using StockApp.Application.Services;
using StockApp.Application.Configurations;
using StockApp.API.Services;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddInfrastructureAPI(builder.Configuration);

        // Configure Stock Replenishment Settings
        builder.Services.Configure<StockReplenishmentSettings>(builder.Configuration.GetSection("StockReplenishmentSettings"));
        builder.Services.AddHostedService<StockReplenishmentService>(); // Register the background service
        builder.Services.AddScoped<StockApp.Application.Interfaces.IImageService, StockApp.API.Services.ImageService>(); // Register ImageService

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                    new string[] {}
                }
            });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseMiddleware<ErrorHandlingMiddleware>(); // Add this line

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}