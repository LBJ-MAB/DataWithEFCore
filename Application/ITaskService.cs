using Domain;
using Microsoft.AspNetCore.Http;

namespace Application;

public interface ITaskService
{
    Task<IResult> AddTask(TaskItem task);
    Task<IResult> GetAllTasks();
    Task<IResult> GetTaskById(int id);
    Task<IResult> GetCompleteTasks();
    Task<IResult> UpdateTask(int id, TaskItem inputTask);
    Task<IResult> DeleteTask(int id);
}