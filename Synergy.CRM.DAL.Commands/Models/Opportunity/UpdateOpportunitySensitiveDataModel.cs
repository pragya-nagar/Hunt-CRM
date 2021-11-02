using System;
using System.Collections.Generic;

namespace Synergy.CRM.DAL.Commands.Models.Opportunity
{
    public class UpdateOpportunitySensitiveDataModel
    {
        public Guid Id { get; set; }

        public List<OpportunitySensitiveDataModel> BorrowersSensitiveData { get; set; }
    }
}
