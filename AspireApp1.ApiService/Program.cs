using AspireApp1.ApiService.DataAccess;
using AspireApp1.ApiService.Services;
using Microsoft.AspNetCore.Mvc;

[assembly: ApiController]
var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
//builder.AddAzureCosmosClient("cosmosConnectionName");

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddControllers();

builder.Services.AddScoped<IDistanceCalculatorService, DistanceCalculatorService>();
builder.Services.AddScoped<IDocumentDbService, DocumentDbService>();
builder.Services.AddSingleton<IDocumentDbRepo<Node>, DocumentDbRepo<Node>>(); 
builder.Services.AddSingleton<IDocumentDbRepo<Edge>, DocumentDbRepo<Edge>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

