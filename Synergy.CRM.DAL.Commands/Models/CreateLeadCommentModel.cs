using System;

namespace Synergy.CRM.DAL.Commands.Models
{
    public class CreateLeadCommentModel
    {
        public Guid Id { get; set; }

        public Guid LeadId { get; set; }

        public string Comment { get; set; }
    }
}
