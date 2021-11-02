using System;
using Synergy.CRM.DAL.Queries.Original.Models;
using Synergy.DataAccess.Abstractions.Interfaces;

namespace Synergy.CRM.DAL.Queries.Original.Interfaces
{
    public interface IGetCommercialBorrowerSensitiveDataQuery : ISingleQuery<IGetCommercialBorrowerSensitiveDataQuery, BorrowerSensetiveData>
    {
        IGetCommercialBorrowerSensitiveDataQuery FilterByOpportunity(Guid id);
    }
}
