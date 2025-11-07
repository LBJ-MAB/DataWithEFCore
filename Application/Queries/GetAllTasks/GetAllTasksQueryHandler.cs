using Application.Dtos;
using AutoMapper;
using Domain;
using MediatR;

namespace Application.Queries.GetAllTasks;

public class GetAllTasksQueryHandler : IRequestHandler<GetAllTasksQuery, List<TaskItemDto>?>
{
    private readonly ITaskRepository _repo;
    private readonly IMapper _mapper;
    
    public GetAllTasksQueryHandler(ITaskRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<List<TaskItemDto>?> Handle(GetAllTasksQuery query, CancellationToken cancellationToken)
    {
        var tasks = await _repo.GetAllAsync();
        if (tasks is null || !tasks.Any())
        {
            return null;
        }
        
        // map from TaskItem to TaskItemDto
        var taskDtos = _mapper.Map<List<TaskItemDto>>(tasks);
        
        return taskDtos;
    }
}