using System;
using Synergy.ServiceBus.Abstracts;

namespace Synergy.CRM.Models.Commands
{
    public class ContactUpdateCommand : Command
    {
        public Guid LeadId { get; set; }

        public int TypeId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public string Title { get; set; }

        public string CellPhone { get; set; }

        public string OfficePhone { get; set; }

        public string Email { get; set; }

        public AddressArg Address { get; set; }
    }
}