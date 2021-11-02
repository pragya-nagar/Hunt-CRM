using FluentValidation;
using Synergy.CRM.Models.Opportunity;

namespace Synergy.CRM.Domain.Validators
{
    public class UpdateBorrowerValidator : AbstractValidator<OpportunityBorrowerUpdateArgs>
    {
        public UpdateBorrowerValidator()
        {
            this.RuleFor(x => x.FirstName).NotEmpty().When(b => b.Order == 0);
            this.RuleFor(x => x.LastName).NotEmpty().When(b => b.Order == 0);
        }
    }
}