using System;
using Synergy.DataAccess.Abstractions.Interfaces;
using Synergy.DataAccess.Abstractions.Models;

namespace Synergy.CRM.DAL.Queries.Original.Models
{
    public class ContactModel : AuditModel, IModel
    {
        public Guid Id { get; set; }

        public Guid LeadId { get; set; }

        public FastEntityModel<Guid> Lead { get; set; }

        public DataAccess.Enum.ContactType Type { get; set; }

        public string Title { get; set; }

        public string CellPhone { get; set; }

        public string OfficePhone { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public ContactAddressModel Address { get; set; }
    }
}
