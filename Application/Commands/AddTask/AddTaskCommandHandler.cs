using Domain;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.AddTask;

public class AddTaskCommandHandler : IRequestHandler<AddTaskCommand, TaskItem?>
{
    private readonly ITaskRepository _repo;
    private IValidator<TaskItem> _validator;

    public AddTaskCommandHandler(
        ITaskRepository repo,
        IValidator<TaskItem> validator)
    {
        _repo = repo;
        _validator = validator;
    }
    
    public async Task<TaskItem?> Handle(AddTaskCommand command, CancellationToken cancellationToken)
    {
        // use AutoMapper here from DTO -> TaskItem
        var taskItem = new TaskItem
        {
            Title = command.InputTask.Title,
            Status = command.InputTask.Status,
            CreatedAt = DateTime.UtcNow
        };
        
        ValidationResult validationResult = await _validator.ValidateAsync(taskItem);
        if (!validationResult.IsValid)
        {
            return null;
        }

        await _repo.AddAsync(taskItem);
        return taskItem;
    }
}