using System;
using Synergy.CRM.DAL.Queries.Original.Models;
using Synergy.DataAccess.Abstractions.Interfaces;

namespace Synergy.CRM.DAL.Queries.Original.Interfaces
{
    public interface IGetBorrowerSensitiveDataQuery : ISingleQuery<IGetBorrowerSensitiveDataQuery, BorrowerSensetiveData>
    {
        IGetBorrowerSensitiveDataQuery FilterByOpportunity(Guid id);
    }
}
