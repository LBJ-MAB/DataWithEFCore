using AutoMapper;
using Domain;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Application.Commands.AddTask;

public class AddTaskCommandHandler : IRequestHandler<AddTaskCommand, AddTaskResult>
{
    private readonly ITaskRepository _repo;
    private IValidator<TaskItem> _validator;
    private IMapper _mapper;

    public AddTaskCommandHandler(
        ITaskRepository repo,
        IValidator<TaskItem> validator,
        IMapper mapper)
    {
        _repo = repo;
        _validator = validator;
        _mapper = mapper;
    }
    
    public async Task<AddTaskResult> Handle(AddTaskCommand command, CancellationToken cancellationToken)
    {
        var taskItem = _mapper.Map<TaskItem>(command.TaskItemDto);
        
        var validationResult = await _validator.ValidateAsync(taskItem);
        if (!validationResult.IsValid)
        {
            return new AddTaskResult{ Errors = validationResult.ToDictionary() };
        }
        
        taskItem.CreatedAt = DateTime.UtcNow;

        await _repo.AddAsync(taskItem);
        return new AddTaskResult{ TaskItem = taskItem } ;
    }
}