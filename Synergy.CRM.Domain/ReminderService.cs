using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Synergy.Common.Domain.Models.Common;
using Synergy.Common.Exceptions;
using Synergy.CRM.DAL.Queries.Original.Interfaces;
using Synergy.CRM.Domain.Abstracts;
using Synergy.CRM.Models;
using Synergy.DataAccess.Enum;

namespace Synergy.CRM.Domain
{
    public class ReminderService : IReminderService
    {
        private readonly IGetReminderQuery _reminderQuery;
        private readonly IMapper _mapper;

        public ReminderService(IGetReminderQuery reminderQuery,
            IMapper mapper)
        {
            this._reminderQuery = reminderQuery ?? throw new ArgumentNullException(nameof(reminderQuery));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<SearchResultModel<ReminderDetails>> GetListAsync(SearchArgsModel<ReminderFilterArgs, ReminderSortField> args, CancellationToken cancellationToken = default)
        {
            var query = this._reminderQuery;

            if (string.IsNullOrWhiteSpace(args?.FullSearch) == false)
            {
                var val = args.FullSearch.Trim();
                query.Search(val);
            }

            if (args?.SortField != null)
            {
                query = (args.SortOrder ?? SortOrder.Asc) == SortOrder.Asc
                    ? query.OrderBy(args.SortField.Value)
                    : query.OrderByDescending(args.SortField.Value);
            }

            if (args?.Filter?.LeadIds?.Any() == true)
            {
                var ids = args.Filter.LeadIds;
                query.FindByLeadIds(ids);
            }

            if (args?.Filter?.CreatedById != Guid.Empty && args?.Filter != null)
            {
                var ids = args.Filter.CreatedById;
                query.FindByCreatedById(ids);
            }

            if (args?.Filter?.Id != Guid.Empty && args?.Filter != null)
            {
                var ids = args.Filter.Id;
                query.FindById(ids);
            }

            if (args?.Filter?.SheduledDate != null && args?.Filter?.SheduledDate != default(DateTime))
            {
                var sheduledDate = args.Filter.SheduledDate;
                query.FindByDate(sheduledDate);
            }

            if (args?.Filter?.MyReminderUserId != Guid.Empty && args?.Filter != null)
            {
                var ids = args.Filter.MyReminderUserId;
                query.FindByCreatedByIdAndUserId(ids);
            }

            if (args?.Filter?.OpportunityIds?.Any() == true)
            {
                var ids = args.Filter.OpportunityIds;
                query.FindByOpportunityIds(ids);
            }

            if (args?.Filter?.IsExpiredNotMoreThanThirtyDays == true)
            {
                query.FindExpiredNotMoreThanThirtyDays(true);
            }

            if (args?.Filter?.IsUpcoming == true)
            {
                query.UpcomingReminders(true);
            }

            query.Skip(args?.Offset ?? 0).Take(args?.Limit ?? 50);

            var items = await query.ExeсuteAsync(cancellationToken).ConfigureAwait(false);
            var count = query.TotalCount ?? 0;

            var newItemList = new List<Synergy.CRM.DAL.Queries.Original.Models.ReminderModel>();
            foreach (var item in items)
            {
                var status = item.Status ?? 0;
                var scheduledDateTime = item.SheduledDate.Add(item.SheduledTime);

                var isEmailNotification = item.IsEmailNotification;
                var isPushNotification = item.IsPushNotification;
                var notificationType = string.Empty;
                if (isEmailNotification == true)
                {
                    notificationType = "Email" + ", ";
                }

                if (isPushNotification == true)
                {
                    notificationType = notificationType + "Push";
                }
                else
                {
                    notificationType = notificationType.Replace(", ", string.Empty);
                }

                var newStatus = scheduledDateTime > DateTime.Now ? "Upcoming" : "Expired";

                if (status == 1)
                {
                    newStatus = "Cancelled";
                }

                item.NewStatus = newStatus;
                item.NotificationType = notificationType;
                newItemList.Add(item);
            }

            return new SearchResultModel<ReminderDetails>
            {
                TotalCount = count,
                List = this._mapper.Map<IEnumerable<ReminderDetails>>(items),
            };
        }

        public async Task<ReminderDetails> FindAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var list = await this._reminderQuery.FindById(id).ExeсuteAsync(cancellationToken).ConfigureAwait(false);

            var item = list.FirstOrDefault();

            var newItemList = new List<Synergy.CRM.DAL.Queries.Original.Models.ReminderModel>();

            if (item != null)
            {
                var status = item.Status ?? 0;
                var scheduledDateTime = item.SheduledDate.Add(item.SheduledTime);

                var isEmailNotification = item.IsEmailNotification;
                var isPushNotification = item.IsPushNotification;
                var notificationType = string.Empty;
                if (isEmailNotification == true)
                {
                    notificationType = "Email" + ", ";
                }

                if (isPushNotification == true)
                {
                    notificationType = notificationType + "Push";
                }
                else
                {
                    notificationType = notificationType.Replace(", ", string.Empty);
                }

                var newStatus = scheduledDateTime > DateTime.Now ? "Upcoming" : "Expired";

                if (status == 1)
                {
                    newStatus = "Cancelled";
                }

                item.NewStatus = newStatus;
                item.NotificationType = notificationType;
            }

            newItemList.Add(item);

            return item == null ? throw new NotFoundException() : this._mapper.Map<ReminderDetails>(item);
        }
    }
}
