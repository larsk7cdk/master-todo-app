using FluentValidation;

namespace ToDo.API.DTO;

public class ToDoDeleteRequestValidator : AbstractValidator<ToDoDeleteRequest>
{
    public ToDoDeleteRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id skal være udfyldt.");
    }
}