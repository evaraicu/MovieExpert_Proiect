using Microsoft.EntityFrameworkCore;
using MovieExpert_Proiect.Data;
using MovieExpert_Proiect.Services;
using System.Globalization;
using MovieTrivia_GrpcService; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient<MoviePredictionService>();

builder.Services.AddGrpcClient<TriviaService.TriviaServiceClient>(o =>
{
    o.Address = new Uri("https://localhost:7052");
});

var app = builder.Build();

app.MapGrpcService<MovieRecommenderService>();

app.MapGet("/", () => "Service running");

app.Run();