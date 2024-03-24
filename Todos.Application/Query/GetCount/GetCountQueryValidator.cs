using FluentValidation;

namespace Todos.Application.Query.GetCount
{
    public class GetCountQueryValidator : AbstractValidator<GetCountQuery>
    {
        public GetCountQueryValidator() 
        {
            RuleFor(e => e.LabelFreeText).MaximumLength(100);
            RuleFor(e => e.OwnerId).GreaterThan(0).When(e => e.OwnerId.HasValue);
        }
    }
}
