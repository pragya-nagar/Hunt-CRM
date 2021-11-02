using System;
using Synergy.ServiceBus.Abstracts;

namespace Synergy.CRM.Models.Commands
{
    public class LeadCommentCreateCommand : Command
    {
        public Guid LeadId { get; set; }

        public string Comment { get; set; }
    }
}
