using Domain;

namespace Application.Commands.UpdateTask;

public class UpdateTaskResult
{
    public TaskItem? TaskItem { get; set; }
    public IDictionary<string, string[]>? Errors { get; set; }
    public bool NotFound { get; set; }
    public bool Success { get; set; }
}