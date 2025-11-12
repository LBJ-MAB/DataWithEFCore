namespace Application.Dtos;

public class TaskItemDto
{
    public string Title { get; set; }
    public string? Description { get; set; }
    public bool Status { get; set; }
    public int? Priority { get; set; }
    public DateTime? DueDate { get; set; }
}