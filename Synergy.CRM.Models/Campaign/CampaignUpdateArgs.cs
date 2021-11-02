using System;
using System.Collections.Generic;

namespace Synergy.CRM.Models
{
    public class CampaignUpdateArgs
    {
        public string Name { get; set; }

        public int TypeId { get; set; }

        public int SubTypeId { get; set; }

        public DateTime? TargetDate { get; set; }

        public Guid AssignedToUserId { get; set; }

        public string Note { get; set; }

        public string Description { get; set; }

        public List<int> CountyIds { get; set; }

        public int StateId { get; set; }
    }
}
