using System;
using Synergy.Common.Domain.Models.Abstracts;
using Synergy.Common.Domain.Models.Common;

namespace Synergy.CRM.Models
{
    public class ContactModel : IResultModel
    {
        public Guid Id { get; set; }

        public FastEntityModel<Guid> Lead { get; set; }

        public FastEntityModel<int> Type { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public string Title { get; set; }

        public string CellPhone { get; set; }

        public string OfficePhone { get; set; }

        public string Email { get; set; }

        public AddressModel Address { get; set; }
    }
}