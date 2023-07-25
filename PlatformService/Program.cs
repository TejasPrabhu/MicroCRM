using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);


// Add configuration to the builder based on the environment
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);
}
else
{
    builder.Configuration.AddJsonFile("appsettings.Production.json", optional: false, reloadOnChange: true);
}

// Add services to the container.
if (builder.Environment.IsProduction())
{
    Console.WriteLine("--> Using SqlServer Db");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn")));
}
else
{
    Console.WriteLine("--> Using InMem Db");
    builder.Services.AddDbContext<AppDbContext>(options =>
         options.UseInMemoryDatabase("InMem"));
}


// builder.Services.AddDbContext<AppDbContext>(options =>
// {
//     options.UseInMemoryDatabase("InMem");
// });
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

PrepDb.PrePopulation(app, app.Environment.IsProduction());

app.Run();
