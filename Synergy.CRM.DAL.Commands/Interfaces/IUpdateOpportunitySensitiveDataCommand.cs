using Synergy.CRM.DAL.Commands.Models.Opportunity;
using Synergy.DataAccess.Abstractions.Commands.Interfaces;

namespace Synergy.CRM.DAL.Commands.Interfaces
{
    public interface IUpdateOpportunitySensitiveDataCommand : ICommand<UpdateOpportunitySensitiveDataModel>
    {
    }
}
