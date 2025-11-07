using Domain;
using MediatR;

namespace Application.Query.GetAllTasks;

public record GetAllTasksQuery() : IRequest<List<TaskItem>?>;