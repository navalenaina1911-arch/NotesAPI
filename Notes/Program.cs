
using Microsoft.EntityFrameworkCore;
using Notes.Extensions;
using Notes.Handler;
using Notes.Interfaces;
using Notes.Mapping;
using Notes.Service;
using NotesDataAccess.Context;
using NotesDataAccess.Interfaces;
using NotesDataAccess.Models;
using NotesDataAccess.Repositories;
using Polly;
using Scalar.AspNetCore;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<NoteAppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("NotesApp");
    options.UseNpgsql(connectionString);
});
builder.Services.AddControllers();
builder.AddTelemetryExtensions();
builder.Services.AddStructuredTelemetry(builder.Configuration);
builder.Services.AddProblemDetails();
builder.Services.AddTransient<INotesService, NotesService>();
builder.Services.AddScoped<IReadRepository<Note>, NotesReadRepository>();
builder.Services.AddScoped<IWriteRepository<Note>, NotesWriteRepository>();
builder.Services.AddSingleton<NotesMapper>();
builder.Services.AddExceptionHandler<ExceptionHandler>();
builder.Services.AddOpenApi("v1");

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy
            .WithOrigins(
                "https://d1q6rp644iikc2.cloudfront.net",
                "http://localhost:4200"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});
var app = builder.Build();
app.MapOpenApi();
app.MapScalarApiReference(option =>
{
    option.Title = "Notes Management API";
    option.AddDocument("v1", "API Version 1.0", "/openapi/v1.json", isDefault: true);
});

app.UseExceptionHandler();
app.UseStructuredTelemetry();
app.UseAuthorization();
app.UseCors();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NoteAppDbContext>();
    db.Database.Migrate();
    Console.WriteLine("DB Created Migration Successfully");
}
app.Run();
