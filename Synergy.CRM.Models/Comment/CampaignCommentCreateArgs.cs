using System;
using Synergy.Common.Domain.Models.Abstracts;
using Synergy.Common.Domain.Models.Common;

namespace Synergy.CRM.Models
{
    public class CampaignCommentCreateArgs
    {
        public Guid CampaignId { get; set; }

        public string Comment { get; set; }
    }
}
