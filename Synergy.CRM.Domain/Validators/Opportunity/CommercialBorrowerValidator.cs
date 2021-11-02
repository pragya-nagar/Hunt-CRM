using FluentValidation;
using Synergy.CRM.Models.Opportunity;

namespace Synergy.CRM.Domain.Validators
{
    public class CommercialBorrowerValidator : AbstractValidator<OpportunityCommercialBorrowerCreateArgs>
    {
        public CommercialBorrowerValidator()
        {
            this.RuleFor(x => x.FirstName).NotEmpty().When(b => b.Order == 0);
            this.RuleFor(x => x.LastName).NotEmpty().When(b => b.Order == 0);
            this.RuleFor(x => x.EntityName).NotEmpty().When(b => b.Order == 0);
            this.RuleFor(x => x.Title).NotEmpty().When(b => b.Order == 0);
        }
    }
}