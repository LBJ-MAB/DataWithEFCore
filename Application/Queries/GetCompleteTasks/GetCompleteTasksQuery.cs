using Domain;
using MediatR;

namespace Application.Queries.GetCompleteTasks;

public record GetCompleteTasksQuery() : IRequest<List<TaskItem>?>;