using Application.Query.GetAllTasks;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Query.GetAllTasks;

public class GetAllTasksQueryHandler : IRequestHandler<GetAllTasksQuery, List<TaskItem>?>
{
    private readonly ITaskRepository _repo;
    
    public GetAllTasksQueryHandler(ITaskRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<TaskItem>?> Handle(GetAllTasksQuery query, CancellationToken cancellationToken)
    {
        var tasks = await _repo.GetAllAsync();
        if (tasks is null || !tasks.Any())
        {
            return null;
        }
        return tasks;
    }
}