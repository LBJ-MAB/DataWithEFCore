using MediatR;

namespace Application.Commands;

public sealed record CreateTaskCommand(
    int Id,
    string Title,
    string? Description,
    bool Status,
    int? Priority,
    DateTime? DueDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt) : IRequest;