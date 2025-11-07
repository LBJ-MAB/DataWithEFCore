using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Queries.GetTaskById;

public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, TaskItem?>
{
    private readonly ITaskRepository _repo;

    public GetTaskByIdQueryHandler(ITaskRepository repo)
    {
        _repo = repo;
    }

    public async Task<TaskItem?> Handle(GetTaskByIdQuery query, CancellationToken cancellationToken)
    {
        // use AutoMapper to map from TaskItem to DTO
        return await _repo.GetByIdAsync(query.Id);
    }
}