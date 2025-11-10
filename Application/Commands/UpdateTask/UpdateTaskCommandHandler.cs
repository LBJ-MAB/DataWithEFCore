using Application.Dtos;
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

        task.Title = command.InputTaskDto.Title;
        task.Description = command.InputTaskDto.Description;
        task.Status = command.InputTaskDto.Status;
        task.Priority = command.InputTaskDto.Priority;
        task.DueDate = command.InputTaskDto.DueDate;
        
        // current time
        task.UpdatedAt = DateTime.UtcNow;

        await _repo.SaveChangesAsync();
        
        return task;
    }
}