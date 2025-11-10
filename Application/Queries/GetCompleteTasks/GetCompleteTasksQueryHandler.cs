using Application.Dtos;
using AutoMapper;
using Domain;
using MediatR;

namespace Application.Queries.GetCompleteTasks;

public class GetCompleteTasksQueryHandler : IRequestHandler<GetCompleteTasksQuery, List<TaskItemDto>?>
{
    private readonly ITaskRepository _repo;
    private readonly IMapper _mapper;
    
    public GetCompleteTasksQueryHandler(ITaskRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<List<TaskItemDto>?> Handle(GetCompleteTasksQuery query, CancellationToken token)
    {
        var tasks = await _repo.GetCompleteAsync();
        if (tasks is null || !tasks.Any())
        {
            return null;
        }
        
        // map to dto
        var taskItemDtos = _mapper.Map<List<TaskItemDto>>(tasks);
        
        return taskItemDtos;
    }
}