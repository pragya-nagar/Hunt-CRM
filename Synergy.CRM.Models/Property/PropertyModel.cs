using System;
using Synergy.Common.Domain.Models.Abstracts;
using Synergy.Common.Domain.Models.Common;

namespace Synergy.CRM.Models
{
    public class PropertyModel : IResultModel
    {
        public Guid Id { get; set; }

        public FastEntityModel<Guid> Lead { get; set; }

        public FastEntityModel<int> County { get; set; }

        public AddressModel Address { get; set; }

        public FastEntityModel<int> GeneralLandUseCode { get; set; }

        public string ParcelId { get; set; }

        public string CadId { get; set; }

        public string TaxId { get; set; }

        public decimal? AmountDue { get; set; }

        public decimal LatestYearDue { get; set; }

        public decimal AppraisedValue { get; set; }
    }
}