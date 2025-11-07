using Domain;
using MediatR;

namespace Application.Commands.UpdateTask;

public record UpdateTaskCommand(
    int Id,
    TaskItem InputTask) : IRequest<TaskItem?>;