using FluentValidation;
using Synergy.CRM.Models;

namespace Synergy.CRM.Domain.Validators
{
    public class CampaignCreateArgsValidator : AbstractValidator<CampaignCreateArgs>
    {
        public CampaignCreateArgsValidator()
        {
            this.RuleFor(x => x.Name)
                .NotEmpty();

            this.RuleFor(x => x.StateId)
                .GreaterThan(0);

            this.RuleFor(x => x.TypeId)
                .GreaterThan(0);

            this.RuleFor(x => x.SubTypeId)
                .GreaterThan(0);

            this.RuleFor(x => x.AssignedToUserId)
                .NotEmpty();
        }
    }
}