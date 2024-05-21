using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Shared.Telemetry.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);

var serviceName = builder.Configuration.GetValue<string>("ServiceName") ?? string.Empty;
var serviceDomain = builder.Configuration.GetValue<string>("ServiceDomain") ?? string.Empty;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenTelemetry(config =>
{
    config.ServiceName = serviceName;
    config.ServiceDomain = serviceDomain;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/translate", async ([FromBody] TranslateRequest request, CancellationToken cancellationToken) =>
{
    await Task.Delay(5000, cancellationToken);
    return new TranslatedResponse(request.Question);
})
.WithName("Translator")
.WithOpenApi();

app.Run();

internal record TranslateRequest([Required] string Question, [Required] string Language);

internal record TranslatedResponse(string Question);