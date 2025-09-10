using System.Text.Json.Serialization;
using NotificationService.Application.Options;
using NotificationService.Infrastructure.Database;
using NotificationService.WebApi.Controllers;
using NotificationService.WebApi.Extensions;
using NotificationService.WebApi.Mapper;
using Quartz;
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


builder.Services.AddQuartz(x => { x.UseMicrosoftDependencyInjectionJobFactory(); });
builder.Services.AddQuartzHostedService(x => x.WaitForJobsToComplete = true);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters
            .Add(new JsonStringEnumConverter());
    });
builder.Services.AddSwaggerGen();
builder.Services.AddDependencyInjection();

builder.Services.Configure<EmailOptions>(builder.Configuration.GetRequiredSection(EmailOptions.PathToOption));

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

    app.MapGet("/", () => Results.Redirect("/swagger/index.html"))
        .ExcludeFromDescription();
}

app.UseSerilogRequestLogging();
app.ApplyMigration();


app.UseCors(x => { x.AllowAnyHeader().WithHeaders(TaskController.TaskQuantityHeader).AllowAnyOrigin(); });
app.UseRouting();

app.MapControllers();
app.Run();