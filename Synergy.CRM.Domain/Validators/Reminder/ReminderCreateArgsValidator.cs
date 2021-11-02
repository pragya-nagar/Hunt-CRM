using System;
using Amazon.KeyManagementService.Model.Internal.MarshallTransformations;
using FluentValidation;
using Synergy.CRM.Models.Reminder;

namespace Synergy.CRM.Domain.Validators.Reminder
{
    public class ReminderCreateArgsValidator : AbstractValidator<ReminderCreateArgs>
    {
        public ReminderCreateArgsValidator()
        {
            this.RuleFor(x => x.UserId)
                .NotEmpty();

            this.RuleFor(x => x.LeadId).NotEmpty().When(x => x.OpportunityId == Guid.Empty);

            this.RuleFor(x => x.OpportunityId).NotEmpty().When(x => x.LeadId == Guid.Empty);

            this.RuleFor(x => x).Must(ValidateOpportunityAndLead).When(x => x.LeadId != Guid.Empty).When(x => x.OpportunityId != Guid.Empty).WithMessage("LeadId and OpportunityId can't be passed in single request");

            this.RuleFor(x => x.IsEmailNotification)
                .NotEmpty();

            this.RuleFor(x => x.IsPushNotification)
                .NotEmpty();

            this.RuleFor(x => x.Description)
                .NotEmpty();

            this.RuleFor(x => x.SheduledDate)
                .NotEmpty();

            this.RuleFor(x => x.SheduledTime)
                .NotEmpty();

            this.RuleFor(x => x.Status)
                .NotEmpty();
        }

        private bool ValidateOpportunityAndLead(ReminderCreateArgs args)
        {
            return args.LeadId == Guid.Empty || args.OpportunityId == Guid.Empty;
        }
    }
}