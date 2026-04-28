using Felersnake.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddTransient<IPathFinder, PathFinder>();
builder.Services.AddTransient<ITargetLocator, TargetLocator>();
builder.Services.AddTransient<ICoordinateChecker, CoordinateChecker>();
builder.Services.AddTransient<IFloodFiller, FloodFiller>();
builder.Services.AddTransient<ITailSeeker, TailSeeker>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();