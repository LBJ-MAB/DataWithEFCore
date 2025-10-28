using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Http;

namespace Application;

public class TaskService : ITaskService
{
    private ITaskRepository _repo;
    public TaskService(ITaskRepository repo)
    {
        _repo = repo;
    }
    
    // add
    public async Task<IResult> AddTask(TaskItem task)
    {
        try
        {
            await _repo.AddAsync(task);
        }
        catch
        {
            return TypedResults.BadRequest("could not add task");
        }
        return TypedResults.Created();
    }
    
    // get all
    public async Task<IResult> GetAllTasks()
    {
        var tasks = await _repo.GetAllAsync();
        if (tasks is null || !tasks.Any())
        {
            return TypedResults.BadRequest("no tasks were found");
        }
        return TypedResults.Ok(tasks);
    }
    
    // get by id
    public async Task<IResult> GetTaskById(int id)
    {
        var task = await _repo.GetByIdAsync(id);
        if (task is null)
        {
            return TypedResults.BadRequest($"could not find task with id {id}");
        }
        return TypedResults.Ok(task);
    }
    
    // get complete
    public async Task<IResult> GetCompleteTasks()
    {
        var tasks = await _repo.GetCompleteAsync();
        if (tasks is null || !tasks.Any())
        {
            return TypedResults.BadRequest("could not find any complete tasks");
        }
        return TypedResults.Ok(tasks);
    }
    
    // update
    public async Task<IResult> UpdateTask(int id, TaskItem inputTask)
    {
        var task = await _repo.GetByIdAsync(id);
        if (task is null)
        {
            return TypedResults.BadRequest($"could not find task with id {id}");
        }

        task.Title = inputTask.Title;
        task.Description = inputTask.Description;
        task.Status = inputTask.Status;
        task.Priority = inputTask.Priority;
        task.DueDate = inputTask.DueDate;
        task.CreatedAt = inputTask.CreatedAt;
        task.UpdatedAt = inputTask.UpdatedAt;

        await _repo.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
    
    // delete
    public async Task<IResult> DeleteTask(int id)
    {
        var task = await _repo.GetByIdAsync(id);
        if (task is null)
        {
            return TypedResults.BadRequest($"could not find task with id {id}");
        }

        await _repo.DeleteAsync(task);
        return TypedResults.NoContent();
    }
}