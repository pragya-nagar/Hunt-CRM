using System;
using System.Collections.Generic;
using Synergy.CRM.DAL.Queries.Original.Models;
using Synergy.DataAccess.Abstractions.Interfaces;
using Synergy.DataAccess.Enum;

namespace Synergy.CRM.DAL.Queries.Original.Interfaces
{
    public interface IGetReminderQuery : IQuery<IGetReminderQuery, IEnumerable<ReminderModel>>,
                                         IHavePagination<IGetReminderQuery>,
                                         IHaveSorting<IGetReminderQuery, ReminderSortField>,
                                         IHaveSearch<IGetReminderQuery>
    {
        IGetReminderQuery FindByOpportunityIds(IEnumerable<Guid> ids);

        IGetReminderQuery FindByLeadIds(IEnumerable<Guid> ids);

        IGetReminderQuery FindByCreatedById(Guid id);

        IGetReminderQuery FindExpiredNotMoreThanThirtyDays(bool isTrue);

        IGetReminderQuery UpcomingReminders(bool isTrue);

        IGetReminderQuery FindByDate(DateTime date);

        IGetReminderQuery FindByCreatedByIdAndUserId(Guid id);
    }
}
