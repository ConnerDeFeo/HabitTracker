namespace Server;
using Server.service;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

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

        builder.Services.AddSingleton<MongoDbService>();

        var app = builder.Build();

        app.UseCors("AllowReactApp");

        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}