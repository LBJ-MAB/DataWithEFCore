using Application.Dtos;
using MediatR;

namespace Application.Queries.GetAllTasks;

public record GetAllTasksQuery() : IRequest<List<TaskItemDto>?>;