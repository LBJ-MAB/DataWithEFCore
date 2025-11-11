using Application.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Queries.GetTaskById;

public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, TaskItemDto?>
{
    private readonly ITaskRepository _repo;
    private readonly IMapper _mapper;

    public GetTaskByIdQueryHandler(ITaskRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<TaskItemDto?> Handle(GetTaskByIdQuery query, CancellationToken cancellationToken)
    {
        var task = await _repo.GetByIdAsync(query.Id);
        if (task is null)
        {
            return null;
        }
        return _mapper.Map<TaskItemDto>(task);
    }
}