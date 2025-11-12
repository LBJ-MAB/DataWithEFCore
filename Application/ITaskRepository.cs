using Domain;

namespace Application;

public interface ITaskRepository
{
    Task AddAsync(TaskItem task);
    Task<List<TaskItem>?> GetAllAsync();
    Task<TaskItem?> GetByIdAsync(int id);
    Task<List<TaskItem>?> GetCompleteAsync();
    Task DeleteAsync(TaskItem task);
    Task SaveChangesAsync();
}