using Microsoft.AspNetCore.Mvc;
using OpenTelemetry;
using Question.Application;
using Question.Application.Services;
using Question.Application.Services.Abstractions;
using Shared.Telemetry.OpenTelemetry;
using Shared.Telemetry;
using System.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

var serviceName = builder.Configuration.GetValue<string>("ServiceName") ?? string.Empty;
var serviceDomain = builder.Configuration.GetValue<string>("ServiceDomain") ?? string.Empty;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddOpenTelemetry(config =>
{
    config.ServiceName = serviceName;
    config.ServiceDomain = serviceDomain;
});
builder.Services.AddTelemetryCore(config => config.ServiceName = serviceName);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/question", async ([FromServices] ITranslatorService translatorService,
    [FromServices] ActivitySource activitySource,
    [FromBody] QuestionRequest question,
    CancellationToken cancellationToken) =>
{
    using var activity = activitySource.StartActivity("Translating the question");
    
    try
    {
        var translatedQuestion = await translatorService.TranslateAsync(question.Question, "en-us", cancellationToken);
        activity!.SetStatus(ActivityStatusCode.Ok);

        return Results.Ok(new QuestionResponse(question.Question, translatedQuestion, "Hello World..."));
    } catch
    {
        activity!.SetStatus(ActivityStatusCode.Error);
        return Results.BadRequest();
    }
})
.WithName("Question")
.WithOpenApi();

app.Run();

internal record QuestionRequest(string Question);

internal record QuestionResponse(string Question, string TranslatedQuestion, string Response);