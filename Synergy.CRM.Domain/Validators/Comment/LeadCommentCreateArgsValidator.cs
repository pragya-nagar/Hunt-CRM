using FluentValidation;
using Synergy.CRM.Models;

namespace Synergy.CRM.Domain.Validators
{
    public class LeadCommentCreateArgsValidator : AbstractValidator<LeadCommentCreateArgs>
    {
        public LeadCommentCreateArgsValidator()
        {
            this.RuleFor(x => x.LeadId)
                .NotEmpty();

            this.RuleFor(x => x.Comment)
                .NotEmpty();
        }
    }
}