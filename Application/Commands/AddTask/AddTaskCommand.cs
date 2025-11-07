using Application.Dtos;
using Domain;
using MediatR;

namespace Application.Commands.AddTask;

public sealed record AddTaskCommand(
    TaskItemDto TaskItemDto) : IRequest<TaskItem?>;