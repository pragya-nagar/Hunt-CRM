using System;
using Synergy.Common.Domain.Models.Abstracts;
using Synergy.Common.Domain.Models.Common;

namespace Synergy.CRM.Models
{
    public class CampaignModel : IResultModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DateTime CreateDate { get; set; }

        public FastEntityModel<int> Type { get; set; }

        public FastEntityModel<int> SubType { get; set; }

        public DateTime? TargetDate { get; set; }

        public FastEntityModel<Guid> AssignedTo { get; set; }

        public string Note { get; set; }

        public string Description { get; set; }
    }
}
