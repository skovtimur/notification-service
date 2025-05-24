using System.Data;
using FluentValidation;
using NotificationService.Domain.Entities;

namespace NotificationService.Domain.Validators;

public class TaskEntityValidator : AbstractValidator<TaskEntity>
{
    public TaskEntityValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id cannot be empty");
        RuleFor(x => x.Content).NotEmpty().NotNull().WithMessage("Content cannot be empty");
    }

    public static bool IsValid(TaskEntity taskEntity) => new TaskEntityValidator().Validate(taskEntity).IsValid;
}