using Synergy.DataAccess.Abstractions.Interfaces;

namespace Synergy.CRM.DAL.Queries.Original.Models
{
    public class CampaignLogicTypeModel : IModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string FieldDataType { get; set; }
    }
}
