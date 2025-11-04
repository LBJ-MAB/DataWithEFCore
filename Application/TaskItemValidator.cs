using Domain;
using FluentValidation;

namespace Application;

public class TaskItemValidator : AbstractValidator<TaskItem>
{
    public TaskItemValidator()
    {
        RuleFor(task => task.Id).NotNull();
        RuleFor(task => task.Title).NotNull().Length(1, 30);
        RuleFor(task => task.Title).NotEmpty();
        RuleFor(task => task.Status).Equal(false);
    }
}