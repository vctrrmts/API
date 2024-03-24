using FluentValidation;

namespace Users.Application.Query.GetById
{
    public class GetByIdQueryValidator : AbstractValidator<GetByIdQuery>
    {
        public GetByIdQueryValidator() 
        {
            RuleFor(e => e.Id).GreaterThan(0);
        }
    }
}
