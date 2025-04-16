namespace Server;
using MongoDB.Driver;
using Server.service;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDb");
        var mongoDatabaseName = builder.Configuration["DatabaseName"];

        // Register MongoClient
        builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
        {
            return new MongoClient(mongoConnectionString);
        });

        // Register IMongoDatabase
        builder.Services.AddSingleton(serviceProvider =>
        {
            var client = serviceProvider.GetRequiredService<IMongoClient>();
            return client.GetDatabase(mongoDatabaseName);
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowReactApp",
                policy =>
                {
                    policy.WithOrigins("http://localhost:5173")  // Frontend URL
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
        });

        builder.Services.AddControllers();
        builder.Services.AddScoped<UserService>();

        var app = builder.Build();

        app.UseCors("AllowReactApp");

        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}