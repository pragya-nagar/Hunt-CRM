using Synergy.ServiceBus.Abstracts;

namespace Synergy.CRM.Models.Commands
{
    public class LeadCommentUpdateCommand : Command
    {
        public string Comment { get; set; }
    }
}