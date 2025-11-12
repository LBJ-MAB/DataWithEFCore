using Application.Dtos;
using MediatR;

namespace Application.Queries.GetCompleteTasks;

public record GetCompleteTasksQuery() : IRequest<List<TaskItemDto>?>;