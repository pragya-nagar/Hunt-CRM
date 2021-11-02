using FluentValidation;
using Synergy.CRM.Models;

namespace Synergy.CRM.Domain.Validators
{
    public class CampaignCommentCreateArgsValidator : AbstractValidator<CampaignCommentCreateArgs>
    {
        public CampaignCommentCreateArgsValidator()
        {
            this.RuleFor(x => x.CampaignId)
                .NotEmpty();

            this.RuleFor(x => x.Comment)
                .NotEmpty();
        }
    }
}
