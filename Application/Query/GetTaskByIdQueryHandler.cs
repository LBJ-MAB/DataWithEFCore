using Domain;
using MediatR;

namespace Application.Query;

public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, TaskItem>
{
    private readonly ITaskRepository _repo;

    public GetTaskByIdQueryHandler(ITaskRepository repo)
    {
        _repo = repo;
    }

    public async Task<TaskItem> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repo.GetByIdAsync(request.Id);
    }
}