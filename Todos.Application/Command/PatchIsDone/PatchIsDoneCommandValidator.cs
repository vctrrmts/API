using FluentValidation;

namespace Todos.Application.Command.PatchIsDone
{
    public class PatchIsDoneCommandValidator : AbstractValidator<PatchIsDoneCommand>
    {
        public PatchIsDoneCommandValidator() 
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
}
