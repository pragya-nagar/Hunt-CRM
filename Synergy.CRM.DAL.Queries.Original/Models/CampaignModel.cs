using System;
using System.Collections.Generic;
using Synergy.DataAccess.Abstractions.Interfaces;
using Synergy.DataAccess.Abstractions.Models;

namespace Synergy.CRM.DAL.Queries.Original.Models
{
    public class CampaignModel : AuditModel, IModel
    {
        public Guid Id { get; set; }

        public DateTime? TargetDate { get; set; }

        public DateTime? CreateDate { get; set; }

        public string CampaignName { get; set; }

        public string Description { get; set; }

        public string Note { get; set; }

        public FastEntityModel<int> CampaignType { get; set; }

        public FastEntityModel<int> CampaignSubType { get; set; }

        public List<FastEntityModel<int>> Counties { get; set; }

        public FastEntityModel<int> State { get; set; }

        public FastEntityModel<Guid> AssignedTo { get; set; }

        public int TotalRecords { get; set; }

        public decimal TotalAmountRecords { get; set; }
    }
}
