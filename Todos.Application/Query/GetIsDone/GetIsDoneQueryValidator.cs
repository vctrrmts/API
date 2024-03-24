using FluentValidation;

namespace Todos.Application.Query.GetIsDone
{
    public class GetIsDoneQueryValidator : AbstractValidator<GetIsDoneQuery>
    {
        public GetIsDoneQueryValidator() 
        {
            RuleFor(e => e.Id).GreaterThan(0);
        }
    }
}
