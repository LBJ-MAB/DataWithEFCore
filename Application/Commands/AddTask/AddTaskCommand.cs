using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.AddTask;

// public sealed record AddTaskCommand(TaskItem taskItem);

// add task command should probably take a task, or task dto instead
public sealed record AddTaskCommand(
    int Id,
    string Title,
    string? Description,
    bool Status,
    int? Priority,
    DateTime? DueDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt) : IRequest<IResult>;