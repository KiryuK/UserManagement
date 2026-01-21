using Application;
using WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddControllers();

// ---------------------------------------------------------
// HIER IST DIE NEUE ZEILE FÜR DIE DEPENDENCY INJECTION:
// ---------------------------------------------------------
// Aktuell nutzen wir die Datei-Logik (users.json).
// Später ändern wir "FileUserService" einfach zu "GraphUserService".
builder.Services.AddScoped<IUserService, FileUserService>();
// ---------------------------------------------------------

builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
        policy.WithOrigins(
            "https://localhost:7166",
            "http://localhost:5252"
        )
        .AllowAnyMethod()
        .AllowAnyHeader());
});

var app = builder.Build();

app.UseCors("AllowBlazor");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();