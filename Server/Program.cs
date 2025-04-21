namespace Server;
using MongoDB.Driver;
using Server.service;
using Server.service.concrete;
public class Program
{
    public static void Main(string[] args)
    {
        //Injects dependencies and set up architecture
        var builder = WebApplication.CreateBuilder(args);


        //may change this in the future to work with PostgreSQL, for now though eh
        var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDb");
        var mongoDatabaseName = builder.Configuration["DatabaseName"];

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
                    policy.WithOrigins(builder.Configuration["Client"]!)  // Frontend URL
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials(); //Needed for some reason
                });
        });

        //all marked controllers are instanciated (I Think?)
        builder.Services.AddControllers();

        //Regenerate UserService each time a request is made
        builder.Services.AddScoped<IUserService, MongoUserService>();

        //Mix everything and haza we have a server
        var app = builder.Build();

        //this took me a while to find in stacktrace
        app.UseCors("AllowReactApp");

        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}