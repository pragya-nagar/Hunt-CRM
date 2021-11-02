using FluentValidation;
using Synergy.CRM.Models.Opportunity;

namespace Synergy.CRM.Domain.Validators
{
    public class BorrowerValidator : AbstractValidator<OpportunityBorrowerCreateArgs>
    {
        public BorrowerValidator()
        {
            this.RuleFor(x => x.FirstName).NotEmpty().When(b => b.Order == 0);
            this.RuleFor(x => x.LastName).NotEmpty().When(b => b.Order == 0);
        }
    }
}