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
                
            })
            .UseAsyncSeeding();
    }
    
    // include seeding!
}