using System.Collections.Generic;
using Synergy.CRM.DAL.Commands.Models;
using Synergy.DataAccess.Abstractions.Commands.Interfaces;

namespace Synergy.CRM.DAL.Commands.Interfaces
{
    public interface ICreateCampaignGeneralRuleCommand : ICommand<CreateRuleModel>
    {
    }
}
