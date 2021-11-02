using System;

namespace Synergy.CRM.Models
{
    public class LeadCommentCreateArgs
    {
        public Guid LeadId { get; set; }

        public string Comment { get; set; }
    }
}
