using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Queries.GetTaskById;

public record GetTaskByIdQuery(
    int Id) : IRequest<TaskItem?>;