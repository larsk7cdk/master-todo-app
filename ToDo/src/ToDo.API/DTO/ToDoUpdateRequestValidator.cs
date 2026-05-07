using FluentValidation;

namespace ToDo.API.DTO;

public class ToDoUpdateRequestValidator : AbstractValidator<ToDoUpdateRequest>
{
    public ToDoUpdateRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id skal være udfyldt.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Navn skal være udfyldt.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Beskrivelse skal være udfyldt.");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status skal være udfyldt.");
    }
}
