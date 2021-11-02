using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Synergy.CRM.DAL.Queries.Original.Interfaces;
using Synergy.CRM.DAL.Queries.Original.Models;
using Synergy.DataAccess.Abstractions;
using Synergy.DataAccess.Context;
using Synergy.DataAccess.Entities;
using Synergy.DataAccess.Enum;

namespace Synergy.CRM.DAL.Queries.Original.Queries
{
    public class GetCampaignsQuery : BaseQuery<Campaign>, IGetCampaignsQuery
    {
        private IMapper _mapper;
        private DbSet<Campaign> _entity;

        #region query builder
        public GetCampaignsQuery(ISynergyContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this._entity = context.Campaign;
        }

        public int? TotalCount { get; private set; }

        public IGetCampaignsQuery FindById(Guid id)
        {
            this.andAlsoPredicates.Add(u => u.Id == id);
            return this;
        }

        public IGetCampaignsQuery FindByLeadId(Guid leadId)
        {
            this.andAlsoPredicates.Add(u => u.CampaignLeads.Any(l => l.LeadId == leadId));
            return this;
        }

        public IGetCampaignsQuery Paging(int page, int pageSize)
        {
            this._skip = (page - 1) * pageSize;
            this._take = pageSize;
            return this;
        }

        public IGetCampaignsQuery Skip(int skip)
        {
            this._skip = skip;
            return this;
        }

        public IGetCampaignsQuery Take(int take)
        {
            this._take = take;
            return this;
        }

        public IGetCampaignsQuery OrderBy(CampaignSortField sortField)
        {
            this._isSortAsc = true;
            this.SetSortSelector(sortField);

            return this;
        }

        public IGetCampaignsQuery OrderByDescending(CampaignSortField sortField)
        {
            this._isSortAsc = false;
            this.SetSortSelector(sortField);

            return this;
        }

        public IGetCampaignsQuery FilterByLeads(IEnumerable<Guid> ids)
        {
            this.andAlsoPredicates.Add(u => u.CampaignLeads.Any(l => ids.Contains(l.LeadId)));
            return this;
        }

        public IGetCampaignsQuery Search(string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return this;
            }

            if (DateTime.TryParse(search, out var date) && date != DateTime.MinValue)
            {
                this.andAlsoPredicates.Add(x => (x.TargetDate.Value.Date == date.Date));
                return this;
            }

            var parts = search.ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            parts.Aggregate(this.andAlsoPredicates,
                (current, part) =>
                {
                    current.Add(x => x.CampaignType.Description.ToLower().Contains(part) ||
                                     x.CampaignName.ToLower().StartsWith(part) ||
                                     x.AssignedUser.FirstName.ToLower().StartsWith(part) ||
                                     x.AssignedUser.LastName.ToLower().StartsWith(part));
                    return current;
                });

            return this;
        }
        #endregion

        public IEnumerable<CampaignModel> Exeсute()
        {
            IQueryable<Campaign> data = this.BuildQuery();

            if (this._skip != null || this._take != null)
            {
                this.TotalCount = this._entity.Where(this.GetPredicate()).Count();
            }

            return this._mapper.Map<IEnumerable<CampaignModel>>(data);
        }

        public async Task<IEnumerable<CampaignModel>> ExeсuteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            IQueryable<Campaign> data = this.BuildQuery();

            if (this._skip != null || this._take != null)
            {
                this.TotalCount = await _entity.Where(GetPredicate()).CountAsync(cancellationToken).ConfigureAwait(false);
            }

            return this._mapper.Map<IEnumerable<CampaignModel>>(await data.ToListAsync(cancellationToken).ConfigureAwait(false));
        }

        private IQueryable<Campaign> BuildQuery()
        {
            this.includes.Add(c => c.AssignedUser);
            this.includes.Add(c => c.CampaignType);
            this.includes.Add(c => c.CampaignSubType);
            this.includes.Add(c => c.State);
            IQueryable<Campaign> query = this._entity.IncludeMultiple(this.includes.ToArray())
                .Include(x => x.CampaignCounty).ThenInclude(cc => cc.County)
                .Where(this.GetPredicate())
                .OrderBy(this._sortSelector, this._isSortAsc)
                .ApplyPaging(this._skip, this._take);

            return query;
        }

        private void SetSortSelector(CampaignSortField sortField)
        {
            switch (sortField)
            {
                case CampaignSortField.AssignedUser:
                    this._sortSelector = e => e.AssignedUser;
                    break;
                case CampaignSortField.CampaignName:
                    this._sortSelector = e => e.CampaignName;
                    break;
                case CampaignSortField.CampaignType:
                    this._sortSelector = e => e.CampaignType;
                    break;
                case CampaignSortField.TargetDate:
                    this._sortSelector = e => e.TargetDate;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sortField), "No sorting exist for such field");
            }
        }
    }
}
