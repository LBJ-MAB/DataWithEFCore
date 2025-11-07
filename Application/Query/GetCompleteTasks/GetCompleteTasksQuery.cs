using Domain;
using MediatR;

namespace Application.Query.GetCompleteTasks;

public record GetCompleteTasksQuery() : IRequest<List<TaskItem>?>;