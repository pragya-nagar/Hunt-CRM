using FluentValidation;
using Synergy.CRM.Models;

namespace Synergy.CRM.Domain.Validators
{
    public class ContactUpdateArgsValidator : AbstractValidator<ContactUpdateArgs>
    {
        public ContactUpdateArgsValidator()
        {
            this.RuleFor(x => x.LeadId)
                .NotEmpty();

            this.RuleFor(x => x.Type)
                .IsInEnum();

            this.RuleFor(x => x.FirstName)
                .NotEmpty();

            this.RuleFor(x => x.LastName)
                .NotEmpty();
        }
    }
}
