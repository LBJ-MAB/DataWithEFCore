using FluentValidation;

namespace Domain;

public class TaskItemValidator : AbstractValidator<TaskItem>
{
    public TaskItemValidator()
    {
        RuleFor(task => task.Id).NotNull();
        RuleFor(task => task.Title).NotNull().Length(1, 20);
        RuleFor(task => task.Status).Equal(false);
    }
}