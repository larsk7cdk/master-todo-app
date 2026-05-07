using FluentValidation;

namespace ToDo.API.DTO;

public class ToDoReadDetailsRequestValidator : AbstractValidator<ToDoReadDetailsRequest>
{
    public ToDoReadDetailsRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id skal være udfyldt.");
    }
}