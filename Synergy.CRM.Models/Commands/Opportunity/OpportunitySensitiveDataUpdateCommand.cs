using System;
using System.Collections.Generic;
using Synergy.ServiceBus.Abstracts;

namespace Synergy.CRM.Models.Commands.Opportunity
{
    public class OpportunitySensitiveDataUpdateCommand : Command
    {
        public Guid OpportunityId { get; set; }

        public List<OpportunitySensitiveDataUpdateArgs> BorrowersSensitiveData { get; set; }
    }
}
