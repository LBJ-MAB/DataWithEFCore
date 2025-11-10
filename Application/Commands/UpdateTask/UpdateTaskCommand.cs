using Application.Dtos;
using Domain;
using MediatR;

namespace Application.Commands.UpdateTask;

public record UpdateTaskCommand(
    int Id,
    TaskItemDto InputTaskDto) : IRequest<TaskItem?>;