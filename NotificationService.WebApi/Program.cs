using System.Collections.Specialized;
using System.Text.Json.Serialization;
using NotificationService.Application.Options;
using NotificationService.Infrastructure.Database;
using NotificationService.WebApi;
using NotificationService.WebApi.Extensions;
using NotificationService.WebApi.Mapper;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.AdoJobStore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddAutoMapper(typeof(MainMapper));
builder.Services.AddCors();
builder.Services.AddDbContext<MainContext>(optionsLifetime: ServiceLifetime.Singleton);
builder.Services.AddDbContextFactory<MainContext>();

builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration.GetRequiredSection("Serilog")).WriteTo.Console());


builder.Services.AddQuartz(x =>
{
    x.UseMicrosoftDependencyInjectionJobFactory();
    // x.UsePersistentStore(store =>
    // {
    //     var connectionString = builder.Configuration.GetRequredValue("UserSecrets:PostgresConnectionString");
    //     var tablePrefix = builder.Configuration.GetRequredValue("Quartz:TablePrefix");
    //
    //     store.UseProperties = true;
    //     store.UseNewtonsoftJsonSerializer();
    //
    //     store.UsePostgres(postgresOptions =>
    //     {
    //         postgresOptions.UseDriverDelegate<PostgreSQLDelegate>();
    //         postgresOptions.ConnectionString = connectionString;
    //         postgresOptions.TablePrefix = tablePrefix;
    //     });
    //     store.PerformSchemaValidation = true;
    // });
});
builder.Services.AddQuartzHostedService(x => x.WaitForJobsToComplete = true);

//Чтобы в swagger enum были видны как строки
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters
            .Add(new JsonStringEnumConverter());
    });
builder.Services.AddSwaggerGen();
builder.Services.AddDependencyInjection();

// Options
builder.Services.Configure<EmailOptions>(builder.Configuration
    .GetRequiredSection(EmailOptions.PathToOption));

var app = builder.Build();
app.UseExceptionHandlerMiddleware();

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSerilogRequestLogging();
app.ApplyMigration();


app.UseCors();
app.UseRouting();

app.MapControllers();
app.Run();