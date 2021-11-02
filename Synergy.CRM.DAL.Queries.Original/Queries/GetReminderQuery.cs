using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Microsoft.EntityFrameworkCore;
using Synergy.CRM.DAL.Queries.Original.Interfaces;
using Synergy.CRM.DAL.Queries.Original.Models;
using Synergy.DataAccess.Abstractions;
using Synergy.DataAccess.Abstractions.Interfaces;
using Synergy.DataAccess.Context;
using Synergy.DataAccess.Entities;
using Synergy.DataAccess.Enum;

namespace Synergy.CRM.DAL.Queries.Original.Queries
{
    public class GetReminderQuery : BaseQuery<Reminder>, IGetReminderQuery
    {
        private readonly IMapper _mapper;
        private readonly DbSet<Reminder> _entity;
        private readonly ISynergyContext _context;

        #region query builder
        public GetReminderQuery(ISynergyContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this._entity = context.Reminder;
            this._context = context;
        }
        #endregion

        public int? TotalCount { get; private set; }

        public IEnumerable<ReminderModel> Exeсute()
        {
            IQueryable<Reminder> data = this.BuildQuery();

            if (this._skip != null || this._take != null)
            {
                this.TotalCount = this._entity.Where(this.GetPredicate()).Count();
            }

            return this._mapper.Map<IEnumerable<ReminderModel>>(data);
        }

        public async Task<IEnumerable<ReminderModel>> ExeсuteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            IQueryable<Reminder> data = this.BuildQuery();

            if (this._skip != null || this._take != null)
            {
                this.TotalCount = await _entity.Where(GetPredicate()).CountAsync(cancellationToken).ConfigureAwait(false);
            }

            var reminders = this._mapper.Map<IEnumerable<ReminderModel>>(data);

            return reminders;
        }

        public IGetReminderQuery UpcomingReminders(bool isTrue)
        {
            if (isTrue)
            {
                this.andAlsoPredicates.Add(u => u.SheduledDate.Add(u.SheduledTime) >= DateTime.Now && u.Status == 0);
            }

            return this;
        }

        public IGetReminderQuery FindById(Guid id)
        {
            this.andAlsoPredicates.Add(u => u.Id == id);
            return this;
        }

        public IGetReminderQuery FindByOpportunityIds(IEnumerable<Guid> ids)
        {
            this.andAlsoPredicates.Add(o => ids.Contains(o.OpportunityId ?? Guid.Empty));
            return this;
        }

        public IGetReminderQuery FindByLeadIds(IEnumerable<Guid> ids)
        {
            this.andAlsoPredicates.Add(o => ids.Contains(o.LeadId ?? Guid.Empty));
            return this;
        }

        public IGetReminderQuery FindByCreatedById(Guid id)
        {
            this.andAlsoPredicates.Add(u => u.CreatedById == id);
            return this;
        }

        public IGetReminderQuery FindExpiredNotMoreThanThirtyDays(bool isTrue)
        {
            if (isTrue)
            {
                var oldDate = DateTime.Now.AddDays(-30);
                this.andAlsoPredicates.Add(u =>
                    u.Status == 0 && (u.SheduledDate > oldDate && u.SheduledDate < DateTime.Now));
            }

            return this;
        }

        public IGetReminderQuery FindByDate(DateTime date)
        {
            this.andAlsoPredicates.Add(u => u.SheduledDate == date);
            return this;
        }

        public IGetReminderQuery FindByCreatedByIdAndUserId(Guid id)
        {
            this.andAlsoPredicates.Add(u => u.CreatedById == id || u.UserId == id);
            return this;
        }

        public IGetReminderQuery OrderBy(ReminderSortField sortField)
        {
            this._isSortAsc = true;
            this.SetSortSelector(sortField);

            return this;
        }

        public IGetReminderQuery OrderByDescending(ReminderSortField sortField)
        {
            this._isSortAsc = false;
            this.SetSortSelector(sortField);

            return this;
        }

        public IGetReminderQuery Search(string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return this;
            }

            search = $"%{search.ToLower()}%";

            this.andAlsoPredicates.Add(x => EF.Functions.Like(x.Description.ToLower(), search) ||
                                            EF.Functions.Like(x.LeadId.ToString().ToLower(), search) ||
                                            EF.Functions.Like(x.OpportunityId.ToString().ToLower(), search) ||
                                            EF.Functions.Like(x.ContactId.ToString().ToLower(), search));

            return this;
        }

        public IGetReminderQuery Skip(int skip)
        {
            this._skip = skip;
            return this;
        }

        public IGetReminderQuery Take(int take)
        {
            this._take = take;
            return this;
        }

        private IQueryable<Reminder> BuildQuery()
        {
            this.includes.Add(c => c.User);
            this.includes.Add(c => c.Contact);
            IQueryable<Reminder> query = this._entity
                .IncludeMultiple(this.includes.ToArray())
                .Where(this.GetPredicate())
                .OrderBy(this._sortSelector, this._isSortAsc)
                .ApplyPaging(this._skip, this._take);
            return query;
        }

        private void SetSortSelector(ReminderSortField sortField)
        {
            switch (sortField)
            {
                case ReminderSortField.SheduledDate:
                    this._sortSelector = e => e.SheduledDate;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(sortField), "No sorting exist for such field");
            }
        }
    }
}
