using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class TaskDb : DbContext
{
    public TaskDb (DbContextOptions<TaskDb> options) : base(options) { }
    
    public DbSet<TaskItem> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>().HasData(
            new TaskItem { Id = 1, Title = "Seed Data", Status = false});
    }
    
}