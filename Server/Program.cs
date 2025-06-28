namespace Server;   
using MongoDB.Driver;
using Server.service.concrete;
using Server.service.interfaces;
using Amazon.S3;

public class Program
{
    public static void Main(string[] args)
    {
        //Injects dependencies and set up architecture
        var builder = WebApplication.CreateBuilder(args);

        var mongoConnectionString = Environment.GetEnvironmentVariable("MONGODB_URI");
        mongoConnectionString ??= builder.Configuration.GetConnectionString("MongoDb");
        var mongoDatabaseName = builder.Configuration["DatabaseName"];

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(5000);
        });

        //only need one for everything
        builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
        {
            return new MongoClient(mongoConnectionString);
        });

        //Only need one here to
        builder.Services.AddSingleton(serviceProvider =>
        {
            return serviceProvider.GetRequiredService<IMongoClient>().GetDatabase(mongoDatabaseName);
        });

        //Allows access to backend routes, will need this this to accept a differnet frontend url for production
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowReactApp",
                policy =>
                {
                    policy.WithOrigins(builder.Configuration["Client"]!)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
        });

        //all marked controllers are instanciated
        builder.Services.AddControllers();

        builder.Services.AddAWSService<IAmazonS3>();
        //Regenerate Services each time a request is made
        builder.Services.AddScoped<IUserService, MongoUserService>();
        builder.Services.AddScoped<IHabitService, MongoHabitService>();
        builder.Services.AddScoped<IHabitHistoryService, MongoHabitHistoryService>();
        builder.Services.AddScoped<IHabitStatisticService, MongoHabitStatisticService>();
        builder.Services.AddScoped<PhotoService>();

        //Mix everything and haza we have a server
        var app = builder.Build();

        //Creates indexed session keys to make lookup faster when making multiple db queries
        using (var scope = app.Services.CreateScope())
        {
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
            userService.CreateSessionKeyIndexes();
        }
        app.MapGet("/api/", () => "API is running!");

        //this took me a while to find in stacktrace
        app.UseCors("AllowReactApp");

        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}