using MediatR;

namespace Application.Commands.DeleteTask;

public record DeleteTaskCommand(int Id) : IRequest<bool>;