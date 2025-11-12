using Domain;
using FluentValidation;

namespace Application.Validation;

public class AddTaskValidator : AbstractValidator<TaskItem>
{
    public AddTaskValidator()
    {
        RuleFor(task => task.Title).NotNull().Length(1, 30);
        RuleFor(task => task.Title).NotEmpty();
        RuleFor(task => task.Status).Equal(false);
    }
}