using System;
using System.Collections.Generic;

namespace Synergy.CRM.Models
{
    public class ContactFilterArgs
    {
        public IEnumerable<Guid> LeadIds { get; set; }
    }
}