using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class TaskDb : DbContext
{
    public DbSet<TaskItem> Tasks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=TaskDB;Trusted_Connection=True;")
            .UseSeeding((context, _) =>
            {
                var testTask = context.Set<TaskItem>().FirstOrDefault(t => t.Title == "test task");
                if (testTask is null)
                {
                    context.Set<TaskItem>().Add(new TaskItem { Title = "test task" });
                    context.SaveChanges();
                }
            })
            .UseAsyncSeeding(async (context, _, cancellationToken) =>
            {
                var testTask = await context.Set<TaskItem>().FirstOrDefaultAsync(t => t.Title == "test task", cancellationToken);
                if (testTask is null)
                {
                    context.Set<TaskItem>().Add(new TaskItem { Title = "test task" });
                    await context.SaveChangesAsync(cancellationToken);
                }
            });
    }
}