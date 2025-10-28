namespace Domain;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public bool Status { get; set; }
    public int? Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}