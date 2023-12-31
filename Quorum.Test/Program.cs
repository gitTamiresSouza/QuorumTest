using Quorum.Test.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/processData", (HttpRequest request) => new MainService(builder.Configuration).ProcessData());

app.Run();
