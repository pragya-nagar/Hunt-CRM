using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Synergy.Common.DAL.Abstract;
using Synergy.Common.Exceptions;
using Synergy.CRM.DAL.Queries.Entities;
using Synergy.CRM.Domain.Abstracts;
using Synergy.CRM.Models.Opportunity;

namespace Synergy.CRM.Domain
{
    public class HistoryService : IHistoryService
    {
        private readonly IMapper _mapper;
        private readonly IQueryProvider<OpportunityAudit> _opportunityAuditQuery;
        private readonly IQueryProvider<Opportunity> _opportunityQuery;
        private readonly IQueryProvider<OpportunityBorrowerBaseAudit> _opportunityBorrowerAuditQuery;
        private readonly IQueryProvider<OpportunityBorrower> _opportunityBorrowerQuery;
        private readonly IQueryProvider<OpportunityCommercialBorrower> _opportunityCommercialBorrowerQuery;
        private readonly IQueryProvider<ContactAudit> _contactAuditQuery;
        private readonly IQueryProvider<Contact> _contactQuery;
        private readonly IQueryProvider<OpportunityPropertyAudit> _opportunityPropertyAuditQuery;
        private readonly IQueryProvider<OpportunityProperty> _opportunityPropertyQuery;
        private readonly IQueryProvider<PropertyAudit> _propertyAuditQuery;
        private readonly IQueryProvider<Property> _propertyQuery;

        public HistoryService(IMapper mapper,
                                 IQueryProvider<OpportunityAudit> opportunityAuditQuery,
                                 IQueryProvider<Opportunity> opportunityQuery,
                                 IQueryProvider<OpportunityBorrowerBaseAudit> opportunityBorrowerAuditQuery,
                                 IQueryProvider<OpportunityBorrower> opportunityBorrowerQuery,
                                 IQueryProvider<OpportunityCommercialBorrower> opportunityCommercialBorrowerQuery,
                                 IQueryProvider<ContactAudit> contactAuditQuery,
                                 IQueryProvider<Contact> contactQuery,
                                 IQueryProvider<OpportunityPropertyAudit> opportunityPropertyAuditQuery,
                                 IQueryProvider<OpportunityProperty> opportunityPropertyQuery,
                                 IQueryProvider<PropertyAudit> propertyAuditQuery,
                                 IQueryProvider<Property> propertyQuery)
        {
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._opportunityAuditQuery = opportunityAuditQuery ?? throw new ArgumentNullException(nameof(opportunityAuditQuery));
            this._opportunityQuery = opportunityQuery ?? throw new ArgumentNullException(nameof(opportunityQuery));
            this._opportunityBorrowerAuditQuery = opportunityBorrowerAuditQuery ?? throw new ArgumentNullException(nameof(opportunityBorrowerAuditQuery));
            this._opportunityBorrowerQuery = opportunityBorrowerQuery ?? throw new ArgumentNullException(nameof(opportunityBorrowerQuery));
            this._opportunityCommercialBorrowerQuery = opportunityCommercialBorrowerQuery ?? throw new ArgumentNullException(nameof(opportunityCommercialBorrowerQuery));
            this._contactAuditQuery = contactAuditQuery ?? throw new ArgumentNullException(nameof(contactAuditQuery));
            this._contactQuery = contactQuery ?? throw new ArgumentNullException(nameof(contactQuery));
            this._opportunityPropertyAuditQuery = opportunityPropertyAuditQuery ?? throw new ArgumentNullException(nameof(opportunityPropertyAuditQuery));
            this._opportunityPropertyQuery = opportunityPropertyQuery ?? throw new ArgumentNullException(nameof(opportunityPropertyQuery));
            this._propertyAuditQuery = propertyAuditQuery ?? throw new ArgumentNullException(nameof(propertyAuditQuery));
            this._propertyQuery = propertyQuery ?? throw new ArgumentNullException(nameof(propertyQuery));
        }

        public async Task<List<OpportunityHistoryModel>> GetOpportunityHistoryAsync(Guid id, OpportunityHistoryFilterArgs filterArgs, CancellationToken cancellationToken = default(CancellationToken))
        {
            var opportunityHistories = await GenerateOpportunityHistories(id, filterArgs, cancellationToken).ConfigureAwait(false);
            return opportunityHistories.OrderByDescending(x => x.LastUpdate).ToList();
        }

        private static List<OpportunityHistoryModel> CompareOpportunityRecords(OpportunityAudit current, OpportunityAudit previous)
        {
            var histories = new List<OpportunityHistoryModel>();

            if (previous.CloseDate != current.CloseDate)
            {
                histories.Add(new OpportunityHistoryModel
                {
                    LastUpdate = current.ModifiedOn,
                    UpdatedBy = current.ModifiedById,
                    Field = OpportunityHistoryFieldName.CloseDate,
                    PreviousValue = previous.CloseDate.HasValue ? previous.CloseDate.Value.ToString("d", CultureInfo.InvariantCulture) : string.Empty,
                    NewValue = current.CloseDate.HasValue ? current.CloseDate.Value.ToString("d", CultureInfo.InvariantCulture) : string.Empty,
                });
            }

            if (previous.CloseProbabilityPercent != current.CloseProbabilityPercent)
            {
                histories.Add(new OpportunityHistoryModel
                {
                    LastUpdate = current.ModifiedOn,
                    UpdatedBy = current.ModifiedById,
                    Field = OpportunityHistoryFieldName.CloseProbabilityPercent,
                    PreviousValue = previous.CloseProbabilityPercent.HasValue ? previous.CloseProbabilityPercent.ToString() : string.Empty,
                    NewValue = current.CloseProbabilityPercent.HasValue ? current.CloseProbabilityPercent.ToString() : string.Empty,
                });
            }

            if (previous.OpportunityStageId != current.OpportunityStageId)
            {
                histories.Add(new OpportunityHistoryModel
                {
                    LastUpdate = current.ModifiedOn,
                    UpdatedBy = current.ModifiedById,
                    Field = OpportunityHistoryFieldName.Stage,
                    PreviousValue = previous.OpportunityStageId.ToString(CultureInfo.InvariantCulture),
                    NewValue = current.OpportunityStageId.ToString(CultureInfo.InvariantCulture),
                });
            }

            if (previous.LoanTypeId != current.LoanTypeId)
            {
                histories.Add(new OpportunityHistoryModel
                {
                    LastUpdate = current.ModifiedOn,
                    UpdatedBy = current.ModifiedById,
                    Field = OpportunityHistoryFieldName.LoanType,
                    PreviousValue = previous.LoanTypeId.HasValue ? previous.LoanTypeId.ToString() : string.Empty,
                    NewValue = current.LoanTypeId.HasValue ? current.LoanTypeId.ToString() : string.Empty,
                });
            }

            if (previous.OriginationPercent != current.OriginationPercent)
            {
                histories.Add(new OpportunityHistoryModel
                {
                    LastUpdate = current.ModifiedOn,
                    UpdatedBy = current.ModifiedById,
                    Field = OpportunityHistoryFieldName.OriginationPercent,
                    PreviousValue = previous.OriginationPercent.HasValue ? previous.OriginationPercent.ToString() : string.Empty,
                    NewValue = current.OriginationPercent.HasValue ? current.OriginationPercent.ToString() : string.Empty,
                });
            }

            if (previous.ClosingCost != current.ClosingCost)
            {
                histories.Add(new OpportunityHistoryModel
                {
                    LastUpdate = current.ModifiedOn,
                    UpdatedBy = current.ModifiedById,
                    Field = OpportunityHistoryFieldName.ClosingCost,
                    PreviousValue = previous.ClosingCost.HasValue ? previous.ClosingCost.ToString() : string.Empty,
                    NewValue = current.ClosingCost.HasValue ? current.ClosingCost.ToString() : string.Empty,
                });
            }

            if (previous.LenderCredit != current.LenderCredit)
            {
                histories.Add(new OpportunityHistoryModel
                {
                    LastUpdate = current.ModifiedOn,
                    UpdatedBy = current.ModifiedById,
                    Field = OpportunityHistoryFieldName.LenderCredit,
                    PreviousValue = previous.LenderCredit.HasValue ? previous.LenderCredit.ToString() : string.Empty,
                    NewValue = current.LenderCredit.HasValue ? current.LenderCredit.ToString() : string.Empty,
                });
            }

            if (previous.AmountDue != current.AmountDue)
            {
                histories.Add(new OpportunityHistoryModel
                {
                    LastUpdate = current.ModifiedOn,
                    UpdatedBy = current.ModifiedById,
                    Field = OpportunityHistoryFieldName.AmountDue,
                    PreviousValue = previous.AmountDue.HasValue ? previous.AmountDue.ToString() : string.Empty,
                    NewValue = current.AmountDue.HasValue ? current.AmountDue.ToString() : string.Empty,
                });
            }

            if (previous.InterestRate != current.InterestRate)
            {
                histories.Add(new OpportunityHistoryModel
                {
                    LastUpdate = current.ModifiedOn,
                    UpdatedBy = current.ModifiedById,
                    Field = OpportunityHistoryFieldName.InterestRate,
                    PreviousValue = previous.InterestRate.HasValue ? previous.InterestRate.ToString() : string.Empty,
                    NewValue = current.InterestRate.HasValue ? current.InterestRate.ToString() : string.Empty,
                });
            }

            if (previous.Term != current.Term)
            {
                histories.Add(new OpportunityHistoryModel
                {
                    LastUpdate = current.ModifiedOn,
                    UpdatedBy = current.ModifiedById,
                    Field = OpportunityHistoryFieldName.Term,
                    PreviousValue = previous.Term.HasValue ? previous.Term.ToString() : string.Empty,
                    NewValue = current.Term.HasValue ? current.Term.ToString() : string.Empty,
                });
            }

            if (previous.PrePay != current.PrePay)
            {
                histories.Add(new OpportunityHistoryModel
                {
                    LastUpdate = current.ModifiedOn,
                    UpdatedBy = current.ModifiedById,
                    Field = OpportunityHistoryFieldName.PrePay,
                    PreviousValue = previous.PrePay.HasValue ? previous.PrePay.ToString() : string.Empty,
                    NewValue = current.PrePay.HasValue ? current.PrePay.ToString() : string.Empty,
                });
            }

            if (previous.MonthlyPrepay != current.MonthlyPrepay)
            {
                histories.Add(new OpportunityHistoryModel
                {
                    LastUpdate = current.ModifiedOn,
                    UpdatedBy = current.ModifiedById,
                    Field = OpportunityHistoryFieldName.MonthlyPrepay,
                    PreviousValue = previous.MonthlyPrepay.HasValue ? previous.MonthlyPrepay.ToString() : string.Empty,
                    NewValue = current.MonthlyPrepay.HasValue ? current.MonthlyPrepay.ToString() : string.Empty,
                });
            }

            if (previous.PercentagePrepay != current.PercentagePrepay)
            {
                histories.Add(new OpportunityHistoryModel
                {
                    LastUpdate = current.ModifiedOn,
                    UpdatedBy = current.ModifiedById,
                    Field = OpportunityHistoryFieldName.PercentagePrepay,
                    PreviousValue = previous.PercentagePrepay.HasValue ? previous.PercentagePrepay.ToString() : string.Empty,
                    NewValue = current.PercentagePrepay.HasValue ? current.PercentagePrepay.ToString() : string.Empty,
                });
            }

            if (previous.CurrentLoanBalance != current.CurrentLoanBalance)
            {
                histories.Add(new OpportunityHistoryModel
                {
                    LastUpdate = current.ModifiedOn,
                    UpdatedBy = current.ModifiedById,
                    Field = OpportunityHistoryFieldName.CurrentLoanBalance,
                    PreviousValue = previous.CurrentLoanBalance.HasValue ? previous.CurrentLoanBalance.ToString() : string.Empty,
                    NewValue = current.CurrentLoanBalance.HasValue ? current.CurrentLoanBalance.ToString() : string.Empty,
                });
            }

            if (previous.ThirdPartyLoanBalance != current.ThirdPartyLoanBalance)
            {
                histories.Add(new OpportunityHistoryModel
                {
                    LastUpdate = current.ModifiedOn,
                    UpdatedBy = current.ModifiedById,
                    Field = OpportunityHistoryFieldName.ThirdPartyLoanBalance,
                    PreviousValue = previous.ThirdPartyLoanBalance.HasValue ? previous.ThirdPartyLoanBalance.ToString() : string.Empty,
                    NewValue = current.ThirdPartyLoanBalance.HasValue ? current.ThirdPartyLoanBalance.ToString() : string.Empty,
                });
            }

            if (previous.OpportunityPropertyTypeId != current.OpportunityPropertyTypeId)
            {
                histories.Add(new OpportunityHistoryModel
                {
                    LastUpdate = current.ModifiedOn,
                    UpdatedBy = current.ModifiedById,
                    Field = OpportunityHistoryFieldName.PropertyType,
                    PreviousValue = previous.OpportunityPropertyTypeId.ToString(CultureInfo.InvariantCulture),
                    NewValue = current.OpportunityPropertyTypeId.ToString(CultureInfo.InvariantCulture),
                });
            }

            return histories;
        }

        private static List<OpportunityHistoryModel> GenerateBorrowersHistory(OpportunityBorrowerBaseAudit topOpportunityBorrowersRecord, List<OpportunityBorrowerBaseAudit> borrowersAudit, bool borrowerTypeChanged)
        {
            List<OpportunityHistoryModel> histories = new List<OpportunityHistoryModel>();
            var borrowersStartIndex = topOpportunityBorrowersRecord != null ? 0 : 1;

            // to show that borrower had no records if PropertyType was changed before
            if (!borrowersAudit.Any() && topOpportunityBorrowersRecord != null && borrowerTypeChanged)
            {
                borrowersAudit.Add(new OpportunityBorrowerBaseAudit());
            }

            for (int i = borrowersStartIndex; i < borrowersAudit.Count; i++)
            {
                histories.AddRange(i == 0 ? CompareBorrowerRecords(topOpportunityBorrowersRecord, borrowersAudit[i]) : CompareBorrowerRecords(borrowersAudit[i - 1], borrowersAudit[i]));
            }

            return histories;
        }

        private static List<OpportunityHistoryModel> CompareBorrowerRecords(OpportunityBorrowerBaseAudit current, OpportunityBorrowerBaseAudit previous)
        {
            var histories = new List<OpportunityHistoryModel>();

            if ($"{previous.FirstName} {previous.MiddleName} {previous.LastName}" != $"{current.FirstName} {current.MiddleName} {current.LastName}")
            {
                histories.Add(new OpportunityHistoryModel
                {
                    LastUpdate = current.ModifiedOn,
                    UpdatedBy = current.ModifiedById,
                    Field = OpportunityHistoryFieldName.BorrowerName,
                    PreviousValue = $"{previous.FirstName} {previous.MiddleName} {previous.LastName}",
                    NewValue = $"{current.FirstName} {current.MiddleName} {current.LastName}",
                    Borrower = new BorrowerHistoryModel
                    {
                        BorrowerOrder = current.Order,
                        BorrowerType = current.Discriminator == nameof(OpportunityBorrower) ? BorrowerType.OpportunityBorrower : BorrowerType.OpportunityCommercialBorrower,
                    },
                });
            }

            if (previous.Email != current.Email)
            {
                histories.Add(new OpportunityHistoryModel
                {
                    LastUpdate = current.ModifiedOn,
                    UpdatedBy = current.ModifiedById,
                    Field = OpportunityHistoryFieldName.BorrowerEmail,
                    PreviousValue = previous.Email,
                    NewValue = current.Email,
                    Borrower = new BorrowerHistoryModel
                    {
                        BorrowerOrder = current.Order,
                        BorrowerType = current.Discriminator == nameof(OpportunityBorrower) ? BorrowerType.OpportunityBorrower : BorrowerType.OpportunityCommercialBorrower,
                    },
                });
            }

            if (previous.CellPhone != current.CellPhone)
            {
                histories.Add(new OpportunityHistoryModel
                {
                    LastUpdate = current.ModifiedOn,
                    UpdatedBy = current.ModifiedById,
                    Field = OpportunityHistoryFieldName.BorrowerCellPhone,
                    PreviousValue = previous.CellPhone,
                    NewValue = current.CellPhone,
                    Borrower = new BorrowerHistoryModel
                    {
                        BorrowerOrder = current.Order,
                        BorrowerType = current.Discriminator == nameof(OpportunityBorrower) ? BorrowerType.OpportunityBorrower : BorrowerType.OpportunityCommercialBorrower,
                    },
                });
            }

            if (previous.WorkPhone != current.WorkPhone)
            {
                histories.Add(new OpportunityHistoryModel
                {
                    LastUpdate = current.ModifiedOn,
                    UpdatedBy = current.ModifiedById,
                    Field = OpportunityHistoryFieldName.BorrowerWorkPhone,
                    PreviousValue = previous.WorkPhone,
                    NewValue = current.WorkPhone,
                    Borrower = new BorrowerHistoryModel
                    {
                        BorrowerOrder = current.Order,
                        BorrowerType = current.Discriminator == nameof(OpportunityBorrower) ? BorrowerType.OpportunityBorrower : BorrowerType.OpportunityCommercialBorrower,
                    },
                });
            }

            if (previous.Fax != current.Fax)
            {
                histories.Add(new OpportunityHistoryModel
                {
                    LastUpdate = current.ModifiedOn,
                    UpdatedBy = current.ModifiedById,
                    Field = OpportunityHistoryFieldName.BorrowerFax,
                    PreviousValue = previous.Fax,
                    NewValue = current.Fax,
                    Borrower = new BorrowerHistoryModel
                    {
                        BorrowerOrder = current.Order,
                        BorrowerType = current.Discriminator == nameof(OpportunityBorrower) ? BorrowerType.OpportunityBorrower : BorrowerType.OpportunityCommercialBorrower,
                    },
                });
            }

            if (previous.IsMarried != current.IsMarried)
            {
                histories.Add(new OpportunityHistoryModel
                {
                    LastUpdate = current.ModifiedOn,
                    UpdatedBy = current.ModifiedById,
                    Field = OpportunityHistoryFieldName.BorrowerMarialStatus,
                    PreviousValue = previous.IsMarried.HasValue ? (previous.IsMarried.Value ? "Married" : "Not Married") : "Unknown",
                    NewValue = current.IsMarried.HasValue ? (current.IsMarried.Value ? "Married" : "Not Married") : "Unknown",
                    Borrower = new BorrowerHistoryModel
                    {
                        BorrowerOrder = current.Order,
                        BorrowerType = BorrowerType.OpportunityBorrower,
                    },
                });
            }

            if (previous.EntityName != current.EntityName)
            {
                histories.Add(new OpportunityHistoryModel
                {
                    LastUpdate = current.ModifiedOn,
                    UpdatedBy = current.ModifiedById,
                    Field = OpportunityHistoryFieldName.BorrowerEntityName,
                    PreviousValue = previous.EntityName,
                    NewValue = current.EntityName,
                    Borrower = new BorrowerHistoryModel
                    {
                        BorrowerOrder = current.Order,
                        BorrowerType = BorrowerType.OpportunityCommercialBorrower,
                    },
                });
            }

            if (previous.Title != current.Title)
            {
                histories.Add(new OpportunityHistoryModel
                {
                    LastUpdate = current.ModifiedOn,
                    UpdatedBy = current.ModifiedById,
                    Field = OpportunityHistoryFieldName.BorrowerTitle,
                    PreviousValue = previous.Title,
                    NewValue = current.Title,
                    Borrower = new BorrowerHistoryModel
                    {
                        BorrowerOrder = current.Order,
                        BorrowerType = BorrowerType.OpportunityCommercialBorrower,
                    },
                });
            }

            return histories;
        }

        private async Task<List<OpportunityHistoryModel>> GenerateOpportunityHistories(Guid opportunityId, OpportunityHistoryFilterArgs filterArgs, CancellationToken cancellationToken = default(CancellationToken))
        {
            List<OpportunityHistoryModel> histories = new List<OpportunityHistoryModel>();
            List<PrimaryContactHistoryModel> primaryContactHistory = new List<PrimaryContactHistoryModel>();

            var currentOpportunityQuery = this._opportunityQuery.Query.Where(x => x.Id == opportunityId);

            var opportunityAuditQuery = this._opportunityAuditQuery.Query.Where(x => x.Id == opportunityId);

            if (filterArgs.DateFrom.HasValue && filterArgs.DateTo.HasValue)
            {
                opportunityAuditQuery = opportunityAuditQuery.Where(x => x.InsertedOn >= filterArgs.DateFrom.Value && x.InsertedOn <= filterArgs.DateTo.Value);
                currentOpportunityQuery = currentOpportunityQuery.Where(x => x.ModifiedOn >= filterArgs.DateFrom.Value && x.ModifiedOn <= filterArgs.DateTo.Value);
            }

            var currentOpportunity = await currentOpportunityQuery.SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            var opportunityAudit = await opportunityAuditQuery.OrderByDescending(x => x.ModifiedOn)
                                         .ToListAsync(cancellationToken)
                                         .ConfigureAwait(false);

            if (currentOpportunity == null && !opportunityAudit.Any())
            {
                return new List<OpportunityHistoryModel>();
            }

            var topOpportunityRecord = this._mapper.Map<OpportunityAudit>(currentOpportunity);
            var startIndex = topOpportunityRecord != null ? 0 : 1;

            for (int i = startIndex; i < opportunityAudit.Count; i++)
            {
                if (i == 0 && topOpportunityRecord.ContactId != opportunityAudit[i].ContactId)
                {
                    primaryContactHistory.Add(new PrimaryContactHistoryModel
                    {
                        CurrentId = topOpportunityRecord.ContactId,
                        PreviouseId = opportunityAudit[i].ContactId,
                        LastUpdate = topOpportunityRecord.ModifiedOn,
                        UpdatedBy = topOpportunityRecord.ModifiedById,
                    });
                }
                else if (i != 0 && opportunityAudit[i - 1].ContactId != opportunityAudit[i].ContactId)
                {
                    primaryContactHistory.Add(new PrimaryContactHistoryModel
                    {
                        CurrentId = opportunityAudit[i - 1].ContactId,
                        PreviouseId = opportunityAudit[i].ContactId,
                        LastUpdate = opportunityAudit[i - 1].ModifiedOn,
                        UpdatedBy = opportunityAudit[i - 1].ModifiedById,
                    });
                }

                histories.AddRange(i == 0 ? CompareOpportunityRecords(topOpportunityRecord, opportunityAudit[i]) : CompareOpportunityRecords(opportunityAudit[i - 1], opportunityAudit[i]));
            }

            if (primaryContactHistory.Any())
            {
                histories.AddRange(await GenerateOpportunityPrimaryContactHistories(primaryContactHistory, cancellationToken).ConfigureAwait(false));
            }

            var mergedOpportunityProperties = await MergeOpportunityProperties(opportunityId, filterArgs, cancellationToken).ConfigureAwait(false);
            histories.AddRange(await GenerateOpportunityPropertyHistories(mergedOpportunityProperties, cancellationToken).ConfigureAwait(false));

            var borrowerTypeChanged = histories.Where(x => x.Field == OpportunityHistoryFieldName.PropertyType).Any();
            var opportunityPropertyTypeId = topOpportunityRecord == null ? opportunityAudit.First().OpportunityPropertyTypeId : currentOpportunity.OpportunityPropertyTypeId;
            histories.AddRange(await GenerateOpportunityBorrowerHistories(opportunityId, filterArgs, borrowerTypeChanged, opportunityPropertyTypeId, cancellationToken).ConfigureAwait(false));
            return histories;
        }

        private async Task<List<OpportunityHistoryModel>> GenerateOpportunityBorrowerHistories(Guid opportunityId,
                                                                                               OpportunityHistoryFilterArgs filterArgs,
                                                                                               bool borrowerTypeChanged,
                                                                                               int opportunityPropertyTypeId,
                                                                                               CancellationToken cancellationToken)
        {
            List<OpportunityHistoryModel> histories = new List<OpportunityHistoryModel>();

            List<OpportunityBorrowerBaseAudit> topBorrowerRecords;

            var currentOpportunityCommercialBorrowersQuery = this._opportunityCommercialBorrowerQuery.Query.Where(x => x.OpportunityId == opportunityId);
            var currentOpportunityBorrowersQuery = this._opportunityBorrowerQuery.Query.Where(x => x.OpportunityId == opportunityId);
            var opportunityBorrowersAuditQuery = this._opportunityBorrowerAuditQuery.Query.Where(x => x.OpportunityId == opportunityId);

            if (filterArgs.DateFrom.HasValue && filterArgs.DateTo.HasValue)
            {
                opportunityBorrowersAuditQuery = opportunityBorrowersAuditQuery.Where(x => x.InsertedOn >= filterArgs.DateFrom.Value && x.InsertedOn <= filterArgs.DateTo.Value);
                currentOpportunityBorrowersQuery = currentOpportunityBorrowersQuery.Where(x => x.ModifiedOn >= filterArgs.DateFrom.Value && x.ModifiedOn <= filterArgs.DateTo.Value);
                currentOpportunityCommercialBorrowersQuery = currentOpportunityCommercialBorrowersQuery.Where(x => x.ModifiedOn >= filterArgs.DateFrom.Value && x.ModifiedOn <= filterArgs.DateTo.Value);
            }

            if (opportunityPropertyTypeId == (int)Models.Opportunity.OpportunityPropertyType.CommercialEntityOwned)
            {
                var currentOpportunityCommercialBorrowers = await currentOpportunityCommercialBorrowersQuery.ToListAsync(cancellationToken).ConfigureAwait(false);
                topBorrowerRecords = this._mapper.Map<List<OpportunityBorrowerBaseAudit>>(currentOpportunityCommercialBorrowers);

                opportunityBorrowersAuditQuery = opportunityBorrowersAuditQuery.Where(x => x.Discriminator == nameof(OpportunityCommercialBorrower));
            }
            else
            {
                var currentOpportunityBorrowers = await currentOpportunityBorrowersQuery.ToListAsync(cancellationToken).ConfigureAwait(false);
                topBorrowerRecords = this._mapper.Map<List<OpportunityBorrowerBaseAudit>>(currentOpportunityBorrowers);

                opportunityBorrowersAuditQuery = opportunityBorrowersAuditQuery.Where(x => x.Discriminator == nameof(OpportunityBorrower));
            }

            var opportunityBorrowersAudit = await opportunityBorrowersAuditQuery.OrderByDescending(x => x.ModifiedOn)
                                                 .ToListAsync(cancellationToken)
                                                 .ConfigureAwait(false);

            var borrowersNumbers = topBorrowerRecords.Select(x => x.Order)
                                 .Union(opportunityBorrowersAudit.Select(x => x.Order)).OrderBy(x => x);

            foreach (var order in borrowersNumbers)
            {
                var borrowerAudit = opportunityBorrowersAudit.Where(x => x.Order == order).ToList();

                histories.AddRange(GenerateBorrowersHistory(topBorrowerRecords.Where(x => x.Order == order).FirstOrDefault(), borrowerAudit, borrowerTypeChanged).ToList());
            }

            return histories;
        }

        private async Task<List<OpportunityHistoryModel>> GenerateOpportunityPrimaryContactHistories(List<PrimaryContactHistoryModel> primaryContactHistory, CancellationToken cancellationToken)
        {
            List<OpportunityHistoryModel> histories = new List<OpportunityHistoryModel>();

            var primaryContacts = await this._contactQuery.Query.Where(x => primaryContactHistory.Any(c => c.CurrentId == x.Id || c.PreviouseId == x.Id))
                                                              .ToListAsync(cancellationToken).ConfigureAwait(false);

            var deletedIds = primaryContactHistory.Select(x => x.CurrentId)
                                                  .Union(primaryContactHistory.Select(x => x.PreviouseId))
                                                  .Where(x => !primaryContacts.Any(c => c.Id == x)).ToList();

            var deletedContactsQuery = await this._contactAuditQuery.Query.Where(x => deletedIds.Any(c => c == x.Id) && x.DeletedOn.HasValue)
                                                              .ToListAsync(cancellationToken).ConfigureAwait(false);

            deletedContactsQuery.AddRange(this._mapper.Map<List<ContactAudit>>(primaryContacts));

            var contacts = deletedContactsQuery;

            foreach (var primaryContact in primaryContactHistory)
            {
                var previouseContact = contacts.Where(x => x.Id == primaryContact.PreviouseId).FirstOrDefault();
                var newContact = contacts.Where(x => x.Id == primaryContact.CurrentId).FirstOrDefault();

                histories.Add(new OpportunityHistoryModel
                {
                    LastUpdate = primaryContact.LastUpdate,
                    UpdatedBy = primaryContact.UpdatedBy,
                    Field = OpportunityHistoryFieldName.PrimaryContactName,
                    PreviousValue = previouseContact != null ? $"{previouseContact.FirstName} {previouseContact.LastName}" : string.Empty,
                    NewValue = newContact != null ? $"{newContact.FirstName} {newContact.LastName}" : string.Empty,
                });
            }

            return histories;
        }

        private async Task<List<OpportunityHistoryModel>> GenerateOpportunityPropertyHistories(List<OpportunityPropertyHistoryModel> mergedOpportunityProperties, CancellationToken cancellationToken)
        {
            List<OpportunityHistoryModel> histories = new List<OpportunityHistoryModel>();

            var grouppedOpportunityProperties = mergedOpportunityProperties.GroupBy(x => x.ModifiedOn.Value.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture))
                                                   .OrderBy(x => x.Key).ToList();

            var propertyIds = mergedOpportunityProperties.Select(x => x.PropertyId).ToList();

            var propertyAddresses = await this._propertyQuery.Query
                                              .Where(x => propertyIds.Contains(x.Id))
                                              .ToListAsync(cancellationToken)
                                              .ConfigureAwait(false);

            var deletedIds = mergedOpportunityProperties.Select(x => x.PropertyId)
                                                .Union(propertyAddresses.Select(x => x.Id))
                                                .Where(x => !propertyAddresses.Any(c => c.Id == x)).ToList();

            var deletedPropertiesQuery = await this._propertyAuditQuery.Query.Where(x => deletedIds.Any(c => c == x.Id) && x.DeletedOn.HasValue)
                                                   .ToListAsync(cancellationToken).ConfigureAwait(false);

            deletedPropertiesQuery.AddRange(this._mapper.Map<List<PropertyAudit>>(propertyAddresses));

            var properties = deletedPropertiesQuery;

            foreach (var grouppedItem in grouppedOpportunityProperties.Skip(1))
            {
                var previouseAddresses = histories.Any() ?
                    histories.Last().NewValues :
                    properties.Where(p => grouppedOpportunityProperties.First().Where(x => x.DeletedOn == null).Select(x => x.PropertyId).Any(g => g == p.Id)).Select(x => x.Address);

                var deleted = properties.Where(p => grouppedItem.Where(x => x.DeletedOn != null).Select(x => x.PropertyId).Any(g => g == p.Id)).Select(x => x.Address);
                var @new = properties.Where(p => grouppedItem.Where(x => x.DeletedOn == null).Select(x => x.PropertyId).Any(g => g == p.Id)).Select(x => x.Address);

                var newAddresses = deleted.Any() ?
                                   new List<string>(previouseAddresses).Except(deleted).Concat(@new) :
                                   properties.Where(p => grouppedItem.Where(x => x.DeletedOn == null).Select(x => x.PropertyId).Any(g => g == p.Id)).Select(x => x.Address)
                                   .Concat(previouseAddresses);

                histories.Add(new OpportunityHistoryModel
                {
                    LastUpdate = grouppedItem.First().ModifiedOn.Value,
                    UpdatedBy = grouppedItem.First().UpdatedBy,
                    Field = OpportunityHistoryFieldName.PropertyAddress,
                    PreviousValues = previouseAddresses,
                    NewValues = newAddresses,
                });
            }

            return histories;
        }

        private async Task<List<OpportunityPropertyHistoryModel>> MergeOpportunityProperties(Guid opportunityId, OpportunityHistoryFilterArgs filterArgs, CancellationToken cancellationToken)
        {
            var opportunityPropertiesQuery = this._opportunityPropertyQuery.Query.Where(x => x.OpportunityId == opportunityId);
            var opportunityPropertiesAuditQuery = this._opportunityPropertyAuditQuery.Query.Where(x => x.OpportunityId == opportunityId);

            if (filterArgs.DateFrom.HasValue && filterArgs.DateTo.HasValue)
            {
                opportunityPropertiesQuery = opportunityPropertiesQuery.Where(x => x.ModifiedOn >= filterArgs.DateFrom.Value && x.ModifiedOn <= filterArgs.DateTo.Value);
                opportunityPropertiesAuditQuery = opportunityPropertiesAuditQuery.Where(x => x.InsertedOn >= filterArgs.DateFrom.Value && x.InsertedOn <= filterArgs.DateTo.Value);
            }

            var opportunityPropertiesAudit = await opportunityPropertiesAuditQuery
                                                      .ToListAsync(cancellationToken)
                                                      .ConfigureAwait(false);

            var currentOpportunityProperties = this._mapper.Map<List<OpportunityPropertyHistoryModel>>(await opportunityPropertiesQuery.ToListAsync(cancellationToken).ConfigureAwait(false));

            var modifiedOpportunityPropertiesAudit = opportunityPropertiesAudit.Select(x => new OpportunityPropertyHistoryModel
            {
                ModifiedOn = x.ModifiedOn,
                PropertyId = x.PropertyId,
                UpdatedBy = x.ModifiedById,
            });

            var deletedOpportunityPropertiesAudit = opportunityPropertiesAudit.Select(x => new OpportunityPropertyHistoryModel
            {
                DeletedOn = x.DeletedOn,
                ModifiedOn = x.InsertedOn,
                PropertyId = x.PropertyId,
                UpdatedBy = x.InsertedBy,
            });

            return modifiedOpportunityPropertiesAudit.Concat(deletedOpportunityPropertiesAudit).Concat(currentOpportunityProperties).ToList();
        }
    }
}
