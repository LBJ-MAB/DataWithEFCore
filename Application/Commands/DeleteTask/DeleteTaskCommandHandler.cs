using MediatR;

namespace Application.Commands.DeleteTask;

public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, bool>
{
    private readonly ITaskRepository _repo;

    public DeleteTaskCommandHandler(ITaskRepository repo)
    {
        _repo = repo;
    }

    public async Task<bool> Handle(DeleteTaskCommand command, CancellationToken cancellationToken)
    {
        var task = await _repo.GetByIdAsync(command.Id);
        if (task is null)
        {
            return false;
        }

        await _repo.DeleteAsync(task);
        return true;
    }
}