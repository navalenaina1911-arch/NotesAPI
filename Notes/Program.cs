
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
var app = builder.Build();

app.Run(); // ✅ only reached if migration succeeded

app.MapOpenApi();

app.MapScalarApiReference(option =>
{
    option.Title = "Notes Management API";
    option.AddDocument("v1", "API Version 1.0", "/openapi/v1.json", isDefault: true);
});
app.UseHsts();
app.UseExceptionHandler();
app.UseStructuredTelemetry();
app.UseAuthorization();
app.MapControllers();
app.Run();
