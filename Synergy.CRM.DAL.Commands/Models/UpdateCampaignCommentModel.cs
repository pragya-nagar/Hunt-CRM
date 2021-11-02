using System;

namespace Synergy.CRM.DAL.Commands.Models
{
    public class UpdateCampaignCommentModel
    {
        public Guid Id { get; set; }

        public string Comment { get; set; }
    }
}