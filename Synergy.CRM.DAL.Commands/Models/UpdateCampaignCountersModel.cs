using System;

namespace Synergy.CRM.DAL.Commands.Models
{
    public class UpdateCampaignCountersModel
    {
        public Guid Id { get; set; }

        public int TotalRecords { get; set; }

        public decimal TotalRecordsAmount { get; set; }
    }
}
