using Domain;
using MediatR;

namespace Application.Queries.GetCompleteTasks;

public class GetCompleteTasksQueryHandler : IRequestHandler<GetCompleteTasksQuery, List<TaskItem>?>
{
    private readonly ITaskRepository _repo;
    
    public GetCompleteTasksQueryHandler(ITaskRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<TaskItem>?> Handle(GetCompleteTasksQuery query, CancellationToken token)
    {
        var tasks = await _repo.GetCompleteAsync();
        if (tasks is null || !tasks.Any())
        {
            return null;
        }
        return tasks;
    }
}