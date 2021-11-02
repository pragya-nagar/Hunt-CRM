namespace Synergy.CRM.Models
{
    using System.Collections.Generic;

    public class LeadDetailsModel : LeadModel
    {
        public bool DoNotContact { get; set; }

        public IEnumerable<ContactModel> Contacts { get; set; }
    }
}
