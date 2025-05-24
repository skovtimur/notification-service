using AppAny.Quartz.EntityFrameworkCore.Migrations;
using AppAny.Quartz.EntityFrameworkCore.Migrations.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NotificationService.Domain.Entities;

namespace NotificationService.Infrastructure.Database;

public class MainContext(
    DbContextOptions<MainContext> options,
    IConfiguration configuration) : DbContext(options)
{
    public DbSet<TaskEntity> Tasks { get; set; }
    public DbSet<RecipientEntity> Recipients { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var utcConverter = new ValueConverter<DateTime, DateTime>(
            v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        modelBuilder.Entity<BaseEntity>((baseEntity) =>
        {
            baseEntity.HasKey(x => x.Id);
            baseEntity.Property(x => x.Id).IsRequired().HasColumnName("id");

            baseEntity.Property(x => x.IsDeleted).HasColumnName("is_deleted");
            baseEntity.Property(x => x.DeletedAt).HasConversion(utcConverter).HasColumnName("deleted_at");
            baseEntity.HasQueryFilter(e => e.IsDeleted == false);
            baseEntity.UseTpcMappingStrategy();
        });
        modelBuilder.Entity<RecipientEntity>((recipientEntity) =>
        {
            recipientEntity.ToTable("recipients");

            recipientEntity.Property(x => x.AddressType).IsRequired()
                .HasConversion<string>().HasColumnName("address_type");

            recipientEntity.Property(x => x.Address).IsRequired().HasColumnName("address");
            recipientEntity.Property(x => x.TaskId).IsRequired().HasColumnName("task_id");
            recipientEntity.Property(x => x.CompletedAt).HasConversion(utcConverter).HasColumnName("completed_at");

            recipientEntity.Property(x => x.ErrorText).HasColumnName("error_text");
            recipientEntity.Property(x => x.SendingStatus).IsRequired().HasColumnName("sending_status")
                .HasConversion<string>().HasDefaultValue(SendingStatus.None);

            // Relationship
            recipientEntity.HasOne(x => x.Task).WithMany(x => x.Recipients)
                .HasForeignKey(x => x.TaskId).IsRequired()
                .HasConstraintName("FK_Recipients_Tasks");
        });

        modelBuilder.Entity<TaskEntity>((taskBuilder) =>
        {
            taskBuilder.ToTable("tasks");

            // Content
            taskBuilder.OwnsOne(x => x.Content, contentBuilder =>
            {
                contentBuilder.Property(x => x.JsonContent)
                    .IsRequired().HasColumnName("json_content");
            });

            // Status
            taskBuilder.Property(x => x.Status)
                .HasConversion<string>().IsRequired().HasColumnName("status");

            // Dates
            taskBuilder.Property(x => x.CreatedAt).IsRequired().HasConversion(utcConverter).HasColumnName("created_at");
            taskBuilder.Property(x => x.MustBeginAt).HasConversion(utcConverter).HasColumnName("must_begin_at");
            taskBuilder.Property(x => x.CompletedAt).HasConversion(utcConverter).HasColumnName("completed_at");

            // Priority
            taskBuilder.Property(x => x.Priority).IsRequired()
                .HasConversion<string>().HasColumnName("priority");
        });
        //modelBuilder.AddQuartz(builder => builder.UsePostgreSql());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var postgresConnectionString = configuration["UserSecrets:PostgresConnectionString"];
        optionsBuilder.UseNpgsql(postgresConnectionString);

        optionsBuilder.UseLoggerFactory(CreateLoggerFactory());
    }

    private static ILoggerFactory CreateLoggerFactory() => LoggerFactory.Create(conf =>
    {
        conf.AddConsole(); //The Method from Microsoft.Extensions.Logging.Console
    });

    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var b in
                 ChangeTracker.Entries<BaseEntity>())
        {
            switch (b.State)
            {
                case EntityState.Added:
                    b.Entity.CreatedAt = DateTime.UtcNow;
                    break;

                case EntityState.Deleted:
                    b.Entity.IsDeleted = true;
                    b.Entity.DeletedAt = DateTime.UtcNow;

                    b.State = EntityState.Modified;
                    break;
            }
        }

        return base.SaveChangesAsync(ct);
    }
}