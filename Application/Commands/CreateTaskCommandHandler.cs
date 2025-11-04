using Domain;
using MediatR;

namespace Application.Commands;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, int>
{
    private readonly ITaskRepository _repo;

    public CreateTaskCommandHandler(ITaskRepository repo)
    {
        _repo = repo;
    }

    public async Task<int> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var taskItem = new TaskItem
        {
            Title = "create task command",
            Status = false,
            CreatedAt = new DateTime()
        };

        await _repo.AddAsync(taskItem);

        return taskItem.Id;
    }
}