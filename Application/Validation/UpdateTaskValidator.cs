using Application.Dtos;
using FluentValidation;

namespace Application.Validation;

public class UpdateTaskValidator : AbstractValidator<TaskItemDto>
{
    public UpdateTaskValidator()
    {
        RuleFor(taskDto => taskDto.Title).NotNull().Length(1, 30);
        RuleFor(taskDto => taskDto.Title).NotEmpty();
    }
}