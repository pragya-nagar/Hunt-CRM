using Synergy.CRM.DAL.Commands.Models;
using Synergy.DataAccess.Abstractions.Commands.Interfaces;

namespace Synergy.CRM.DAL.Commands.Interfaces
{
    public interface ICreateOpportunityCommand : ICommand<CreateOpportunityModel>
    {
    }
}
