var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddSingleton<MongoDbService>();

var app = builder.Build();
app.MapControllers();
app.Run();