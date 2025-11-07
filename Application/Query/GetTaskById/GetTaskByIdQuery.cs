using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Query.GetTaskById;

public record GetTaskByIdQuery(
    int Id) : IRequest<TaskItem?>;