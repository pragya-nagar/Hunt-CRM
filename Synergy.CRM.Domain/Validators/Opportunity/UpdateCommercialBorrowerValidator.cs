using FluentValidation;
using Synergy.CRM.Models.Opportunity;

namespace Synergy.CRM.Domain.Validators
{
    public class UpdateCommercialBorrowerValidator : AbstractValidator<OpportunityCommercialBorrowerUpdateArgs>
    {
        public UpdateCommercialBorrowerValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().When(b => b.Order == 0);
            RuleFor(x => x.LastName).NotEmpty().When(b => b.Order == 0);
            RuleFor(x => x.EntityName).NotEmpty().When(b => b.Order == 0);
            RuleFor(x => x.Title).NotEmpty().When(b => b.Order == 0);
        }
    }
}
