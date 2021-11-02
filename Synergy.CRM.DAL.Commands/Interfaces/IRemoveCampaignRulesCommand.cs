using System;
using Synergy.DataAccess.Abstractions.Commands.Interfaces;

namespace Synergy.CRM.DAL.Commands.Interfaces
{
    public interface IRemoveCampaignRulesCommand : ICommand<Guid>
    {
    }
}
