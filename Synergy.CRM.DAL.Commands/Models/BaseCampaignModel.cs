using System;
using System.Collections.Generic;

namespace Synergy.CRM.DAL.Commands.Models
{
    public class BaseCampaignModel
    {
        public Guid Id { get; set; }

        public Guid AssignedUserId { get; set; }

        public DateTime TargetDate { get; set; }

        public int CampaignTypeId { get; set; }

        public int? CampaignSubTypeId { get; set; }

        public string CampaignName { get; set; }

        public string Description { get; set; }

        public string Note { get; set; }

        public int StateId { get; set; }

        public List<int> CountyIds { get; set; }
    }
}
