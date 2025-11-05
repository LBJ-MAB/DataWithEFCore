using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Query.GetTaskById;

public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, IResult>
{
    private readonly ITaskRepository _repo;

    public GetTaskByIdQueryHandler(ITaskRepository repo)
    {
        _repo = repo;
    }

    public async Task<IResult> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var taskItem = await _repo.GetByIdAsync(request.Id);
        // use AutoMapper to map from TaskItem to DTO
        if (taskItem is null)
        {
            return TypedResults.BadRequest($"could not find task with id {request.Id}");
        }
        return TypedResults.Ok(taskItem);
    }
}