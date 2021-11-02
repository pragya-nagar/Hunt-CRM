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
    public class GetPropertiesQuery : BaseQuery<Property>, IGetPropertiesQuery
    {
        private IMapper _mapper;
        private DbSet<Property> _entity;
        private ISynergyContext _context;

        #region query builder

        public GetPropertiesQuery(ISynergyContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this._entity = context.Property;
            this._context = context;
        }

        public int? TotalCount { get; private set; }

        public IGetPropertiesQuery FindById(Guid id)
        {
            this.andAlsoPredicates.Add(p => p.Id == id);
            return this;
        }

        public IGetPropertiesQuery Skip(int skip)
        {
            this._skip = skip;
            return this;
        }

        public IGetPropertiesQuery Take(int take)
        {
            this._take = take;
            return this;
        }

        public IGetPropertiesQuery OrderBy(PropertySortField sortField)
        {
            this._isSortAsc = true;
            this.SetSortSelector(sortField);

            return this;
        }

        public IGetPropertiesQuery OrderByDescending(PropertySortField sortField)
        {
            this._isSortAsc = false;
            this.SetSortSelector(sortField);

            return this;
        }

        public IGetPropertiesQuery FilterByLeads(IEnumerable<Guid> ids)
        {
            this.andAlsoPredicates.Add(p => ids.Contains(p.LeadId));
            return this;
        }

        public IGetPropertiesQuery FilterByOpportunities(IEnumerable<Guid> ids)
        {
            this.andAlsoPredicates.Add(p => p.OpportunityProperties.Any(op => ids.Contains(op.OpportunityId)));
            return this;
        }

        public IGetPropertiesQuery IncludeContacts()
        {
            this.includes.Add(p => p.Lead);
            return this;
        }

        public IGetPropertiesQuery IncludeValuation()
        {
            this.includes.Add(p => p.PropertyValuations);
            return this;
        }

        public IGetPropertiesQuery IncludeDelinquency()
        {
            this.includes.Add(p => p.Delinquencies);
            return this;
        }

        public IGetPropertiesQuery IncludeLead()
        {
            this.includes.Add(p => p.Lead);
            return this;
        }

        public IGetPropertiesQuery Search(string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return this;
            }

            search = $"%{search.ToLower()}%";

            this.andAlsoPredicates.Add(x => EF.Functions.Like(x.Address.ToLower(), search) ||
                                    EF.Functions.Like(x.Lead.AccountName.ToLower(), search) ||
                                    EF.Functions.Like(x.ParcelId.ToLower(), search) ||
                                    EF.Functions.Like(x.CADId.ToLower(), search) ||
                                    EF.Functions.Like(x.TAXId.ToLower(), search));

            return this;
        }
        #endregion

        public IEnumerable<PropertyModel> Exeсute()
        {
            IQueryable<Property> data = this.BuildQuery();

            if (this._skip != null || this._take != null)
            {
                this.TotalCount = this._entity.Where(this.GetPredicate()).Count();
            }

            var properties = data.ToList();
            properties.ForEach(x => x.Delinquencies = x.Delinquencies.Where(d => d.IsLoan));

            var result = this._mapper.Map<List<PropertyModel>>(properties);
            this.AddContacts(result);
            this.AddCollectingEntities(result.SelectMany(x => x.Delinquencies).ToList());
            return result;
        }

        public async Task<IEnumerable<PropertyModel>> ExeсuteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            IQueryable<Property> data = this.BuildQuery();

            if (this._skip != null || this._take != null)
            {
                this.TotalCount = await _entity.Where(GetPredicate()).CountAsync(cancellationToken).ConfigureAwait(false);
            }

            var properties = await data.ToListAsync(cancellationToken).ConfigureAwait(false);
            properties.ForEach(x => x.Delinquencies = x.Delinquencies.Where(d => d.IsLoan));

            var result = this._mapper.Map<List<PropertyModel>>(properties);
            this.AddContacts(result);
            this.AddCollectingEntities(result.SelectMany(x => x.Delinquencies).ToList());
            return result;
        }

        private IQueryable<Property> BuildQuery()
        {
            this.includes.Add(x => x.State);
            this.includes.Add(x => x.County);
            this.andAlsoPredicates.Add(x => x.Delinquencies.Any(d => d.IsLoan));
            IQueryable<Property> query = this._entity.IncludeMultiple(this.includes.ToArray())
                .Where(this.GetPredicate())
                .OrderBy(this._sortSelector, this._isSortAsc)
                .ApplyPaging(this._skip, this._take);

            return query;
        }

        private void AddContacts(List<PropertyModel> properties)
        {
            var ids = properties.Select(p => p.Lead.Id).ToList();
            var contacts = this._mapper.Map<IEnumerable<ContactModel>>(this._context.Contact.Where(e => ids.Contains(e.LeadId)));
            properties.ForEach(p => p.Contacts = contacts.Where(c => c.LeadId == p.Lead.Id));
        }

        private void AddCollectingEntities(List<DelinquencyModel> delinquencies)
        {
            var ids = delinquencies.Select(d => d.Id).ToList();
            var collectingEntities = this._context.CollectingEntity.Where(e => ids.Contains(e.DelinquencyId)).ToList();

            delinquencies.ForEach(x =>
            {
                x.CollectingEntities = this._mapper.Map<List<CollectingEntityModel>>(collectingEntities.Where(c => x.Id == c.DelinquencyId));
            });
        }

        private void SetSortSelector(PropertySortField sortField)
        {
            switch (sortField)
            {
                case PropertySortField.AccountName:
                    this._sortSelector = e => e.Lead.AccountName;
                    break;
                case PropertySortField.Address:
                    this._sortSelector = e => e.Address;
                    break;
                case PropertySortField.CADId:
                    this._sortSelector = e => e.CADId;
                    break;
                case PropertySortField.GeneralLandUseCode:
                    this._sortSelector = e => e.GeneralLandUseCode;
                    break;
                case PropertySortField.ParcelId:
                    this._sortSelector = e => e.ParcelId;
                    break;
                case PropertySortField.TAXId:
                    this._sortSelector = e => e.TAXId;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sortField), "No sorting exist for such field");
            }
        }
    }
}
