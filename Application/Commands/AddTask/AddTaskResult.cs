using Domain;
using FluentValidation.Results;

namespace Application.Commands.AddTask;

public class AddTaskResult
{
    public TaskItem? TaskItem { get; set; }
    public IDictionary<string, string[]>? Errors { get; set; }
    public bool Success => Errors == null || !Errors.Any();
}