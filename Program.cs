using MovieRecommender_GrpcService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<MovieRecommenderService>();

app.MapGet("/", () => "Service running");

app.Run();