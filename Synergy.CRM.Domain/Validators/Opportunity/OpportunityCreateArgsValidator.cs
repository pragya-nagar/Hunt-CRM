using FluentValidation;
using Synergy.CRM.Models.Opportunity;

namespace Synergy.CRM.Domain.Validators
{
    public class OpportunityCreateArgsValidator : AbstractValidator<OpportunityCreateArgs>
    {
        public OpportunityCreateArgsValidator()
        {
            this.RuleFor(x => x.LeadId)
                .NotEmpty();

            this.RuleFor(x => x.LoanType)
                .IsInEnum();

            this.RuleFor(x => x.Stage)
                .IsInEnum();

            this.RuleFor(x => x.AmountDue)
                .GreaterThan(0)
                .When(x => x.AmountDue.HasValue);

            this.RuleFor(x => x.LenderCredit)
                .GreaterThan(0)
                .When(x => x.LenderCredit.HasValue);

            this.RuleFor(x => x.ClosingCost)
                .GreaterThan(0)
                .When(x => x.ClosingCost.HasValue);

            this.RuleFor(x => x.CloseProbabilityPercent)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(100)
                .When(x => x.CloseProbabilityPercent.HasValue);

            this.RuleFor(x => x.OriginationPercent)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(100)
                .When(x => x.OriginationPercent.HasValue);

            this.RuleFor(x => x.InterestRate)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(100)
                .When(x => x.InterestRate.HasValue);

            this.RuleFor(x => x.OpportunityPropertyTypeId)
                .NotEmpty()
                .GreaterThan(0);

            this.RuleFor(x => x.Borrowers)
                .Must(x => x != null && x.Exists(o => o.Order == 0))
                .When(x => x.OpportunityPropertyTypeId != (int)OpportunityPropertyType.CommercialEntityOwned);

            this.RuleForEach(x => x.Borrowers)
                .NotNull()
                .SetValidator(new BorrowerValidator())
                .When(x => x.OpportunityPropertyTypeId != (int)OpportunityPropertyType.CommercialEntityOwned);

            this.RuleFor(x => x.CommercialBorrowers)
                .NotNull()
                .Must(x => x.Exists(o => o.Order == 0))
                .When(x => x.OpportunityPropertyTypeId == (int)OpportunityPropertyType.CommercialEntityOwned);

            this.RuleForEach(x => x.CommercialBorrowers)
                .NotNull()
                .SetValidator(new CommercialBorrowerValidator())
                .When(x => x.OpportunityPropertyTypeId == (int)OpportunityPropertyType.CommercialEntityOwned);
        }
    }
}