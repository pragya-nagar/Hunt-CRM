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
    public class GetContactsQuery : BaseQuery<Contact>, IGetContactsQuery
    {
        private IMapper _mapper;
        private DbSet<Contact> _entity;

        #region query builder
        public GetContactsQuery(ISynergyContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this._entity = context.Contact;
        }

        public int? TotalCount { get; private set; }

        public IGetContactsQuery FindById(Guid id)
        {
            this.andAlsoPredicates.Add(u => u.Id == id);
            return this;
        }

        public IGetContactsQuery Skip(int skip)
        {
            this._skip = skip;
            return this;
        }

        public IGetContactsQuery Take(int take)
        {
            this._take = take;
            return this;
        }

        public IGetContactsQuery OrderBy(ContactSortField sortField)
        {
            this._isSortAsc = true;
            this.SetSortSelector(sortField);

            return this;
        }

        public IGetContactsQuery OrderByDescending(ContactSortField sortField)
        {
            this._isSortAsc = false;
            this.SetSortSelector(sortField);

            return this;
        }

        public IGetContactsQuery FilterByLeads(IEnumerable<Guid> ids)
        {
            this.andAlsoPredicates.Add(u => ids.Contains(u.LeadId));
            return this;
        }

        public IGetContactsQuery Search(string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return this;
            }

            search = $"%{search.ToLower()}%";

            this.andAlsoPredicates.Add(x => EF.Functions.Like(x.ContactType.Description.ToLower(), search) ||
                                    EF.Functions.Like(x.FirstName.ToLower(), search) ||
                                    EF.Functions.Like(x.LastName.ToLower(), search) ||
                                    EF.Functions.Like(x.CellPhone.ToLower(), search) ||
                                    EF.Functions.Like(x.OfficePhone.ToLower(), search) ||
                                    EF.Functions.Like(x.Email.ToLower(), search) ||
                                    EF.Functions.Like(x.Title.ToLower(), search));

            return this;
        }
        #endregion

        public IEnumerable<ContactModel> Exeсute()
        {
            IQueryable<Contact> data = this.BuildQuery();

            if (this._skip != null || this._take != null)
            {
                this.TotalCount = this._entity.Where(this.GetPredicate()).Count();
            }

            return this._mapper.Map<IEnumerable<ContactModel>>(data);
        }

        public async Task<IEnumerable<ContactModel>> ExeсuteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            IQueryable<Contact> data = this.BuildQuery();

            if (this._skip != null || this._take != null)
            {
                this.TotalCount = await _entity.Where(GetPredicate()).CountAsync(cancellationToken).ConfigureAwait(false);
            }

            return this._mapper.Map<IEnumerable<ContactModel>>(await data.ToListAsync(cancellationToken).ConfigureAwait(false));
        }

        private IQueryable<Contact> BuildQuery()
        {
            this.includes.Add(c => c.MailingState);
            this.includes.Add(c => c.Lead);
            IQueryable<Contact> query = this._entity
                        .IncludeMultiple(this.includes.ToArray())
                        .Where(this.GetPredicate())
                        .OrderBy(this._sortSelector, this._isSortAsc)
                        .ApplyPaging(this._skip, this._take);

            return query;
        }

        private void SetSortSelector(ContactSortField sortField)
        {
            switch (sortField)
            {
                case ContactSortField.CellPhone:
                    this._sortSelector = e => e.CellPhone;
                    break;
                case ContactSortField.ContactType:
                    this._sortSelector = e => e.ContactType;
                    break;
                case ContactSortField.Email:
                    this._sortSelector = e => e.Email;
                    break;
                case ContactSortField.Name:
                    this._sortSelector = e => e.LastName;
                    break;
                case ContactSortField.Title:
                    this._sortSelector = e => e.Title;
                    break;
                case ContactSortField.WorkPhone:
                    this._sortSelector = e => e.OfficePhone;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sortField), "No sorting exist for such field");
            }
        }
    }
}
