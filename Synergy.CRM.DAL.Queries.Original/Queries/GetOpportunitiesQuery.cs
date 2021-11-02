using System;
using System.Collections.Generic;
using System.Globalization;
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
using Synergy.DataAccess.Entities.OpportunityEntities;
using Synergy.DataAccess.Enum;

namespace Synergy.CRM.DAL.Queries.Original.Queries
{
    public class GetOpportunitiesQuery : BaseQuery<Opportunity>, IGetOpportunitiesQuery
    {
        private IMapper _mapper;
        private DbSet<Opportunity> _entity;
        private ISynergyContext _context;

        #region query builder

        public GetOpportunitiesQuery(ISynergyContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this._entity = context.Opportunity;
            this._context = context;
        }

        public int? TotalCount { get; private set; }

        public IGetOpportunitiesQuery Skip(int skip)
        {
            this._skip = skip;
            return this;
        }

        public IGetOpportunitiesQuery Take(int take)
        {
            this._take = take;
            return this;
        }

        public IGetOpportunitiesQuery OrderBy(OpportunitySortField sortField)
        {
            this._isSortAsc = true;
            this.SetSortSelector(sortField);

            return this;
        }

        public IGetOpportunitiesQuery OrderByDescending(OpportunitySortField sortField)
        {
            this._isSortAsc = false;
            this.SetSortSelector(sortField);

            return this;
        }

        public IGetOpportunitiesQuery FindById(Guid id)
        {
            this.andAlsoPredicates.Add(u => u.Id == id);
            return this;
        }

        public IGetOpportunitiesQuery FindByLeadIds(IEnumerable<Guid> ids)
        {
            this.andAlsoPredicates.Add(o => ids.Contains(o.LeadId));
            return this;
        }

        public IGetOpportunitiesQuery FindByCampaignIds(IEnumerable<Guid> ids)
        {
            this.andAlsoPredicates.Add(o => ids.Contains(o.CampaignId ?? Guid.Empty));
            return this;
        }

        public IGetOpportunitiesQuery FindByUserIds(IEnumerable<Guid> ids)
        {
            this.andAlsoPredicates.Add(o => ids.Contains(o.UserId));
            return this;
        }

        public IGetOpportunitiesQuery FilterByLeads(IEnumerable<Guid> ids)
        {
            this.andAlsoPredicates.Add(u => ids.Contains(u.LeadId));
            return this;
        }

        public IGetOpportunitiesQuery Search(string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return this;
            }

            var parts = search.ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            parts.Aggregate(this.andAlsoPredicates,
                (current, part) =>
                {
                    current.Add(x => x.Lead.AccountName.ToLower().Contains(part) ||
                                     x.OpportunityNumber.ToLower().StartsWith(part) ||
                                     x.OpportunityPropertyType.Description.ToLower().Contains(part) ||
                                     (x.OpportunityBorrowers.Any() &&
                                      x.OpportunityBorrowers.OrderBy(o => o.Order).Take(2).Any(o =>
                                          o.FirstName.ToLower().StartsWith(part) ||
                                          o.LastName.ToLower().StartsWith(part) ||
                                          o.Email.ToLower().Contains(part) ||
                                          o.CellPhone.ToLower().StartsWith(part) ||
                                          o.WorkPhone.ToLower().StartsWith(part))));
                    return current;
                });

            return this;
        }

        public IGetOpportunitiesQuery IncludeContact()
        {
            this.includes.Add(o => o.Contact);
            return this;
        }

        #endregion

        public IEnumerable<OpportunityModel> Exeсute()
        {
            IQueryable<Opportunity> data = this.BuildQuery();

            if (this._skip != null || this._take != null)
            {
                this.TotalCount = this._entity.Where(this.GetPredicate()).Count();
            }

            var opportunities = this._mapper.Map<IEnumerable<OpportunityModel>>(data);
            var opportunityIds = opportunities.Select(x => x.Id).ToList();
            var properties = this.GetProperties(opportunityIds).ToList();

            foreach (var opportunity in opportunities)
            {
                opportunity.Properties = this._mapper.Map<List<PropertyModel>>(properties.Where(x =>
                    x.OpportunityProperties.Any(op => op.OpportunityId == opportunity.Id)));
            }

            return opportunities;
        }

        public async Task<IEnumerable<OpportunityModel>> ExeсuteAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            IQueryable<Opportunity> data = this.BuildQuery();

            if (this._skip != null || this._take != null)
            {
                this.TotalCount =
                    await _entity.Where(GetPredicate()).CountAsync(cancellationToken).ConfigureAwait(false);
            }

            var opportunities = this._mapper.Map<IEnumerable<OpportunityModel>>(data);
            var opportunityIds = opportunities.Select(x => x.Id).ToList();
            var properties = await GetProperties(opportunityIds).ToListAsync(cancellationToken).ConfigureAwait(false);
            foreach (var opportunity in opportunities)
            {
                opportunity.Properties = this._mapper.Map<List<PropertyModel>>(properties.Where(x =>
                    x.OpportunityProperties.Any(op => op.OpportunityId == opportunity.Id)));
            }

            return opportunities;
        }

        public PdfExportModel GetPdfToExport(string opportunityNo)
        {
            var opportunity = (from s in _context.Opportunity
                               where s.OpportunityNumber == opportunityNo
                               select s).SingleOrDefault();

            var opportunityBorrowers = string.Empty;
            var propertyAddress = string.Empty;
            decimal monthlyPayment = 0;
            decimal estimateTaxDueCounty = 0;

            if (opportunity != null)
            {
                var opportunityBorrower = (from opb in _context.OpportunityBorrower
                    where opb.Opportunity.Id == opportunity.Id
                    select new
                    {
                        Name = opb.FirstName + " " + opb.LastName,
                    }).ToList();

                foreach (var item in opportunityBorrower)
                {
                    if (item.Name != null)
                    {
                        opportunityBorrowers = opportunityBorrowers + item.Name + ", ";
                    }
                }

                var property = this._context.Property
                    .Include(x => x.OpportunityProperties)
                    .Where(x => x.OpportunityProperties.Any(op => op.OpportunityId == opportunity.Id));

                foreach (var prop in property)
                {
                    estimateTaxDueCounty = estimateTaxDueCounty + prop.TotalAmountDue;
                }

                propertyAddress = property.Count() > 1 ? "Multiple Property" : property.SingleOrDefault()?.Address;

                var newLoanAmount = opportunity?.AmountDue ?? 0 +
                                    ((opportunity?.AmountDue ?? 0 * opportunity?.OriginationPercent ?? 0) / 100) +
                                    opportunity?.CurrentLoanBalance ?? 0 +
                                    opportunity?.ThirdPartyLoanBalance ?? 0 +
                                    opportunity?.ClosingCost ?? 0 -
                                    opportunity?.LenderCredit ?? 0;

                var n = opportunity?.Term == null ? 0 : opportunity.Term * 12;
                var r = opportunity?.InterestRate == null ? 0 : opportunity.InterestRate / 100 / 12;
                var r1 = 1 + r;
                var pow = Math.Pow(Convert.ToDouble(r1, CultureInfo.InvariantCulture),
                    Convert.ToDouble(n, CultureInfo.InvariantCulture));

                var y = newLoanAmount * r * Convert.ToDecimal(pow);

                if (n > 0)
                {
                    monthlyPayment = Convert.ToDecimal(y, CultureInfo.InvariantCulture) / Convert.ToDecimal(pow - 1);
                }

                monthlyPayment = Math.Round(monthlyPayment, 2);
            }

            var borrowersName = string.Empty;
            if (opportunityBorrowers != null && opportunityBorrowers.Contains(", "))
            {
                borrowersName = opportunityBorrowers.Remove(opportunityBorrowers.Length - 2);
            }

            return new PdfExportModel()
                {
                    BorrowersName = borrowersName,

                    PropertyAddress = propertyAddress,

                    PreparationDate = System.DateTime.Now,

                    InterestRate = opportunity?.InterestRate ?? 0,

                    PaymentAmount = monthlyPayment,

                    EstimatedClosingDate = opportunity?.CloseDate,

                    TaxLoanTerm = opportunity?.Term,

                    FirstPaymentDate = opportunity?.CloseDate == null
                        ? opportunity?.CloseDate
                        : new DateTime(
                            Convert.ToDateTime(opportunity?.CloseDate, CultureInfo.InvariantCulture).AddMonths(2).Year,
                            Convert.ToDateTime(opportunity?.CloseDate, CultureInfo.InvariantCulture).AddMonths(2).Month,
                            1),

                    EstimatedCountyTaxDue = estimateTaxDueCounty,

                    EstimatedIsdTaxDue = Math.Round(0m, 2),

                    EstimatedMudTaxDue = 0m,

                    EstimatedOtherTaxDue = 0m,

                    AttorneyCountyFee = 0m,

                    AttorneyIsdFee = 0m,

                    CountyConstableFees = 0m,

                    TotalEstimatedTaxDisbursements = 0m,

                    BaseClosingCost = opportunity?.ClosingCost ?? 0m,

                    OrganisationPercent = opportunity?.OriginationPercent ?? 0m,

                    LenderCredit = opportunity?.LenderCredit ?? 0m,

                    TotalEstimatedClosingCost = (opportunity?.ClosingCost ?? 0m) +
                                                ((opportunity?.AmountDue ?? 0m) * (opportunity?.OriginationPercent ?? 0m) / 100) +
                                                (opportunity?.LenderCredit ?? 0m),

                    TotalAboveEstimatedDisbursmentTax = 0m,

                    TotalAboveClosingCost = 0m,

                    PrepayPenalty = 0m,

                    EstimatedTaxLoanAmount = 0m,
                };
            }

        private IQueryable<Opportunity> BuildQuery()
        {
            this.includes.Add(c => c.User);
            this.includes.Add(c => c.Lead);
            this.includes.Add(c => c.OpportunityBorrowers);
            this.includes.Add(c => c.OpportunityCommercialBorrowers);
            IQueryable<Opportunity> query = this._entity
                .IncludeMultiple(this.includes.ToArray())
                .Where(this.GetPredicate())
                .OrderBy(this._sortSelector, this._isSortAsc)
                .ApplyPaging(this._skip, this._take);

            return query;
        }

        private IQueryable<Property> GetProperties(List<Guid> ids)
        {
            IQueryable<Property> query = this._context.Property
                .Include(x => x.PropertyValuations)
                .Include(x => x.OpportunityProperties)
                .Include(x => x.County)
                .Where(x => x.OpportunityProperties.Any(op => ids.Contains(op.OpportunityId)) &&
                            x.PropertyValuations.Any(p => p.IsActive));

            return query;
        }

        private void SetSortSelector(OpportunitySortField sortField)
        {
            switch (sortField)
            {
                case OpportunitySortField.AccountName:
                    this._sortSelector = e => e.Lead.AccountName;
                    break;
                case OpportunitySortField.AmountDue:
                    this._sortSelector = e => e.AmountDue;
                    break;
                case OpportunitySortField.CloseDate:
                    this._sortSelector = e => e.CloseDate;
                    break;
                case OpportunitySortField.FirstBorrowerFirstName:
                    this._sortSelector = e => e.OpportunityBorrowers.OrderBy(o => o.Order).FirstOrDefault().FirstName;
                    break;
                case OpportunitySortField.FirstBorrowerLastName:
                    this._sortSelector = e => e.OpportunityBorrowers.OrderBy(o => o.Order).FirstOrDefault().LastName;
                    break;
                case OpportunitySortField.LoanType:
                    this._sortSelector = e => e.LoanType;
                    break;
                case OpportunitySortField.OpportunityStage:
                    this._sortSelector = e => e.OpportunityStage;
                    break;
                case OpportunitySortField.SecondBorrowerFirstName:
                    this._sortSelector = e =>
                        e.OpportunityBorrowers.OrderBy(o => o.Order).Skip(1).FirstOrDefault().FirstName;
                    break;
                case OpportunitySortField.SecondBorrowerLastName:
                    this._sortSelector = e =>
                        e.OpportunityBorrowers.OrderBy(o => o.Order).Skip(1).FirstOrDefault().LastName;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(sortField), "No sorting exist for such field");
            }
        }
    }
}