using Microsoft.OpenApi.Models;
using System.Reflection;
using UnitConversionApi.Middleware;
using UnitConversionApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add controllers support
builder.Services.AddControllers();

// Add dependency injection for the conversion service
builder.Services.AddScoped<IConversionService, ConversionService>();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Unit Conversion API",
        Version = "v1",
        Description = "An ASP.NET Core 8 Web API built on clean architecture principles to handle physical unit conversions (Length, Temperature, Weight)."
    });

    // Configure Swagger to use XML comments
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Register the global exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment() || builder.Configuration.GetValue<bool>("EnableSwaggerUI", true))
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Unit Conversion API v1");
        options.RoutePrefix = "swagger"; // Standard swagger UI path
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
