using System;
using Synergy.DataAccess.Enum;

namespace Synergy.CRM.Models
{
    public class ContactUpdateArgs
    {
        public Guid LeadId { get; set; }

        public ContactType Type { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public string Title { get; set; }

        public string CellPhone { get; set; }

        public string OfficePhone { get; set; }

        public string Email { get; set; }

        public AddressCreateArgs Address { get; set; }
    }
}
