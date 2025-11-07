using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.AddTask;

// public sealed record AddTaskCommand(TaskItem taskItem);

// add task command should probably take a task, or task dto instead
public sealed record AddTaskCommand(
    TaskItem InputTask) : IRequest<TaskItem?>;