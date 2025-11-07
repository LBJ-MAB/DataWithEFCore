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
        
        // might need mapping from DTO to taskItem here as well
        // the command should have DTO not task Item
        // that way we only overwrite the properties shared with the DTO - won't need createdAt anymore
        // set UpdatedAt date separately

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