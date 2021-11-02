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
    public class GetLeadsQuery : BaseQuery<Lead>, IGetLeadsQuery
    {
        private IMapper _mapper;
        private DbSet<Lead> _entity;
        private ISynergyContext _context;

        #region query builder
        public GetLeadsQuery(ISynergyContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this._entity = context.Lead;
            this._context = context;
        }

        public int? TotalCount { get; private set; }

        public IGetLeadsQuery FindById(Guid id)
        {
            this.andAlsoPredicates.Add(u => u.Id == id);
            return this;
        }

        public IGetLeadsQuery Skip(int skip)
        {
            this._skip = skip;
            return this;
        }

        public IGetLeadsQuery Take(int take)
        {
            this._take = take;
            return this;
        }

        public IGetLeadsQuery OrderBy(LeadSortField sortField)
        {
            this._isSortAsc = true;
            this.SetSortSelector(sortField);

            return this;
        }

        public IGetLeadsQuery OrderByDescending(LeadSortField sortField)
        {
            this._isSortAsc = false;
            this.SetSortSelector(sortField);

            return this;
        }

        public IGetLeadsQuery Search(string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return this;
            }

            var parts = search.ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            parts.Aggregate(this.andAlsoPredicates,
                (current, part) =>
                {
                    current.Add(x => x.AccountName.ToLower().Contains(part) ||
                                     x.MailingAddress1.ToLower().Contains(part) ||
                                     x.MailingAddress2.ToLower().Contains(part) ||
                                     x.MailingAddress3.ToLower().Contains(part));
                    return current;
                });

            return this;
        }
        #endregion

        public IEnumerable<LeadModel> Exeсute()
        {
            IQueryable<Lead> data = this.BuildQuery();

            if (this._skip != null || this._take != null)
            {
                this.TotalCount = this._entity.Where(this.GetPredicate()).Count();
            }

            List<Lead> leads = data.ToList();
            List<Guid> leadsId = leads.Select(l => l.Id).ToList();

            List<Contact> contacts = _context.Contact.Where(c => leadsId.Contains(c.LeadId)).ToList();
            List<Property> properties = _context.Property.Where(p => leadsId.Contains(p.LeadId) && p.Delinquencies.Any(d => d.IsLoan)).ToList();

            leads.ForEach(l =>
            {
                l.Contacts = contacts.Where(c => c.LeadId == l.Id);
                l.Properties = properties.Where(p => p.LeadId == l.Id);
            });

            return this._mapper.Map<IEnumerable<LeadModel>>(leads);
        }

        public async Task<IEnumerable<LeadModel>> ExeсuteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            IQueryable<Lead> data = this.BuildQuery();

            if (this._skip != null || this._take != null)
            {
                this.TotalCount = await _entity.Where(GetPredicate()).CountAsync(cancellationToken).ConfigureAwait(false);
            }

            List<Lead> leads = await data.ToListAsync(cancellationToken).ConfigureAwait(false);
            List<Guid> leadsId = leads.Select(l => l.Id).ToList();

            List<Contact> contacts = await _context.Contact.Where(c => leadsId.Contains(c.LeadId)).ToListAsync(cancellationToken).ConfigureAwait(false);
            List<Property> properties = await _context.Property.Where(p => leadsId.Contains(p.LeadId) && p.Delinquencies.Any(d => d.IsLoan)).ToListAsync(cancellationToken).ConfigureAwait(false);

            leads.ForEach(l =>
                {
                    l.Contacts = contacts.Where(c => c.LeadId == l.Id);
                    l.Properties = properties.Where(p => p.LeadId == l.Id);
                });

            return this._mapper.Map<IEnumerable<LeadModel>>(leads);
        }

        private IQueryable<Lead> BuildQuery()
        {
            this.includes.Add(l => l.MailingState);
            this.andAlsoPredicates.Add(l => l.Properties.Any(p => p.Delinquencies.Any(d => d.IsLoan)));
            IQueryable<Lead> query = this._entity
                        .IncludeMultiple(this.includes.ToArray())
                        .Where(this.GetPredicate())
                        .OrderBy(this._sortSelector, this._isSortAsc)
                        .ApplyPaging(this._skip, this._take);

            return query;
        }

        private void SetSortSelector(LeadSortField sortField)
        {
            switch (sortField)
            {
                case LeadSortField.AccountName:
                    this._sortSelector = e => e.AccountName;
                    break;
                case LeadSortField.MailingAddress1:
                    this._sortSelector = e => e.MailingAddress1;
                    break;
                case LeadSortField.MailingAddress2:
                    this._sortSelector = e => e.MailingAddress2;
                    break;
                case LeadSortField.MailingAddress3:
                    this._sortSelector = e => e.MailingAddress3;
                    break;
                case LeadSortField.MailingCity:
                    this._sortSelector = e => e.MailingCity;
                    break;
                case LeadSortField.MailingState:
                    this._sortSelector = e => e.MailingState;
                    break;
                case LeadSortField.MailingZipCode:
                    this._sortSelector = e => e.MailingZipCode;
                    break;
                case LeadSortField.PropertiesCount:
                    this._sortSelector = e => e.Properties.Count();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sortField), "No sorting exist for such field");
            }
        }
    }
}
