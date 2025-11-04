using Domain;
using MediatR;

namespace Application.Query;

public record GetTaskByIdQuery(int Id) : IRequest<TaskItem>;