namespace Server;
using Server.service;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddSingleton<MongoDbService>();

        var app = builder.Build();

        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}