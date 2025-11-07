using AutoMapper;
using Domain;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Application.Commands.AddTask;

public class AddTaskCommandHandler : IRequestHandler<AddTaskCommand, TaskItem?>
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
    
    public async Task<TaskItem?> Handle(AddTaskCommand command, CancellationToken cancellationToken)
    {
        var taskItem = _mapper.Map<TaskItem>(command.TaskItemDto);
        
        ValidationResult validationResult = await _validator.ValidateAsync(taskItem);
        if (!validationResult.IsValid)
        {
            return null;
        }
        
        taskItem.CreatedAt = DateTime.UtcNow;

        await _repo.AddAsync(taskItem);
        return taskItem;
    }
}