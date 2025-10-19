using Starter.Api.Requests;
using Starter.Api.Responses;
using Starter.Api.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddTransient<IMoveService, MoveService>();
builder.Services.AddTransient<ITargetService, TargetService>();
builder.Services.AddTransient<ICoordinateChecker, CoordinateChecker>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();