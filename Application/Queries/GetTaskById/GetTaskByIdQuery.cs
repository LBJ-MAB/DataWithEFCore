using Application.Dtos;
using MediatR;

namespace Application.Queries.GetTaskById;

public record GetTaskByIdQuery(
    int Id) : IRequest<TaskItemDto?>;