using FluentValidation;

namespace Users.Application.Query.GetList
{
    public class GetListQueryValidator : AbstractValidator<GetListQuery>
    {
        public GetListQueryValidator() 
        {
            RuleFor(e => e.Limit).GreaterThan(0).When(e => e.Limit.HasValue);
            RuleFor(e => e.Offset).GreaterThan(0).When(e => e.Offset.HasValue);
            RuleFor(e => e.NameFreeText).MaximumLength(100);
        }
    }
}
