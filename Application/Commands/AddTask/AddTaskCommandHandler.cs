using Domain;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.AddTask;

public class AddTaskCommandHandler : IRequestHandler<AddTaskCommand, IResult>
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
    
    public async Task<IResult> Handle(AddTaskCommand request, CancellationToken cancellationToken)
    {
        // use AutoMapper here from DTO -> TaskItem
        var taskItem = new TaskItem
        {
            Title = request.Title,
            Status = request.Status,
            CreatedAt = DateTime.UtcNow
        };
        
        ValidationResult validationResult = await _validator.ValidateAsync(taskItem);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        await _repo.AddAsync(taskItem);
        return TypedResults.Created($"/tasks/{taskItem.Id}", taskItem);
    }
}