using Application;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class DbTaskRepository : ITaskRepository
{
    private TaskDb _context;
    public DbTaskRepository(TaskDb context)
    {
        _context = context;
    }

    public async Task AddAsync(TaskItem task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
    }

    public async Task<List<TaskItem>> GetAllAsync()
    {
        return await _context.Tasks.ToListAsync();
    }

    public async Task<TaskItem> GetByIdAsync(int id)
    {
        return await _context.Tasks.FindAsync(id);
    }

    public async Task<List<TaskItem>> GetCompleteAsync()
    {
        return await _context.Tasks.Where(t => t.Status).ToListAsync();
    }

    public async Task DeleteAsync(TaskItem task)
    {
        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}