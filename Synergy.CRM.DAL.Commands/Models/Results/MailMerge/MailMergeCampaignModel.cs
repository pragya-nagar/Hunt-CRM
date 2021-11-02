using System;
using System.Collections.Generic;
using System.Text;

namespace Synergy.CRM.DAL.Commands.Models.Results.MailMerge
{
    public class MailMergeCampaignModel
    {
        public string CampaignName { get; set; }

        public string CampaignType { get; set; }

        public string CampaignSubType { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string Description { get; set; }

        public DateTime? TargetDate { get; set; }

        public string Note { get; set; }

        public string AssignedUser { get; set; }
    }
}
