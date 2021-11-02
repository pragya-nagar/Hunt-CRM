using System;

namespace Synergy.CRM.DAL.Commands.Models
{
    public class UpdateLeadCommentModel
    {
        public Guid Id { get; set; }

        public string Comment { get; set; }
    }
}