using Synergy.DataAccess.Abstractions.Interfaces;

namespace Synergy.CRM.DAL.Queries.Original.Models
{
    public class BorrowerSensetiveData : IModel
    {
        public string SSN { get; set; }

        public string TaxIdNumber { get; set; }

        public string DateOfBirth { get; set; }
    }
}
