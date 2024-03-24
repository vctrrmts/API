using FluentValidation;

namespace Users.Application.Query.GetCount
{
    internal class GetCountQueryValidator : AbstractValidator<GetCountQuery>
    {
        public GetCountQueryValidator() 
        {
            RuleFor(e => e.NameFreeText).MaximumLength(100);
        }
    }
}
