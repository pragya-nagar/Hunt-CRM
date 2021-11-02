using System;

namespace Synergy.CRM.Models.Commands.Opportunity
{
    public abstract class BorrowerBaseArgs
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public string Email { get; set; }

        public string CellPhone { get; set; }

        public string WorkPhone { get; set; }

        public string Fax { get; set; }

        public int Order { get; set; }
    }
}
