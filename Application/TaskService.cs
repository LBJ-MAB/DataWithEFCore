using Domain;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace Application;

public class TaskService : ITaskService
{
    private ITaskRepository _repo;
    private IValidator<TaskItem> _validator;
    public TaskService(ITaskRepository repo, IValidator<TaskItem> validator)
    {
        _repo = repo;
        _validator = validator;
    }
    
    // add
    public async Task<IResult> AddTask(TaskItem task)
    {
        ValidationResult validationResult = await _validator.ValidateAsync(task);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        await _repo.AddAsync(task);
        return TypedResults.Created($"/tasks/{task.Id}", task);
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