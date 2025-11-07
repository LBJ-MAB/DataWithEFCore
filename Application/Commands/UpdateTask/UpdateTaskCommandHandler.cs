using Domain;
using MediatR;

namespace Application.Commands.UpdateTask;

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, TaskItem?>
{
    private readonly ITaskRepository _repo;

    public UpdateTaskCommandHandler(ITaskRepository repo)
    {
        _repo = repo;
    }

    public async Task<TaskItem?> Handle(UpdateTaskCommand command, CancellationToken cancellationToken)
    {
        var task = await _repo.GetByIdAsync(command.Id);
        if (task is null)
        {
            return null;
        }

        task.Title = command.InputTask.Title;
        task.Description = command.InputTask.Description;
        task.Status = command.InputTask.Status;
        task.Priority = command.InputTask.Priority;
        task.DueDate = command.InputTask.DueDate;
        task.CreatedAt = command.InputTask.CreatedAt;
        task.UpdatedAt = command.InputTask.UpdatedAt;

        await _repo.SaveChangesAsync();
        
        return task;
    }
}