using Application.Dtos;
using Domain;
using FluentValidation;
using MediatR;

namespace Application.Commands.UpdateTask;

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, UpdateTaskResult>
{
    private readonly ITaskRepository _repo;
    private readonly IValidator<TaskItemDto> _validator;

    public UpdateTaskCommandHandler(ITaskRepository repo, IValidator<TaskItemDto> validator)
    {
        _repo = repo;
        _validator = validator;
    }

    public async Task<UpdateTaskResult> Handle(UpdateTaskCommand command, CancellationToken cancellationToken)
    {
        // validate the command.Dto first
        var validationResult = await _validator.ValidateAsync(command.InputTaskDto);
        if (!validationResult.IsValid)
        {
            return new UpdateTaskResult{ Success = false, NotFound = false, Errors = validationResult.ToDictionary() } ;
        }
        
        var task = await _repo.GetByIdAsync(command.Id);
        if (task is null)
        {
            return new UpdateTaskResult{ Success = false, NotFound = true };
        }

        task.Title = command.InputTaskDto.Title;
        task.Description = command.InputTaskDto.Description;
        task.Status = command.InputTaskDto.Status;
        task.Priority = command.InputTaskDto.Priority;
        task.DueDate = command.InputTaskDto.DueDate;
        
        // current time
        task.UpdatedAt = DateTime.UtcNow;

        await _repo.SaveChangesAsync();

        return new UpdateTaskResult { Success = true, TaskItem = task };
    }
}