using System;

namespace Synergy.CRM.DAL.Commands.Models
{
    public class CreateCampaignCommentModel
    {
        public Guid Id { get; set; }

        public Guid CampaignId { get; set; }

        public string Comment { get; set; }
    }
}
