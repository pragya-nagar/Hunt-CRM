using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace Synergy.CRM.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using Synergy.Common.DAL.Abstract;
    using Synergy.Common.Domain.Models.Common;
    using Synergy.Common.Exceptions;
    using Synergy.CRM.DAL.Queries.Entities;
    using Synergy.CRM.DAL.Queries.Original.Interfaces;
    using Synergy.CRM.Domain.Abstracts;
    using Synergy.CRM.Models;
    using Synergy.CRM.Models.Opportunity;
    using Synergy.DataAccess.Enum;

    public class OpportunityService : IOpportunityService
    {
        private readonly IGetOpportunitiesQuery _opportunityQuery;
        private readonly IMapper _mapper;
        private readonly IGetBorrowerSensitiveDataQuery _borrowerSensitiveDataQuery;
        private readonly IGetCommercialBorrowerSensitiveDataQuery _commercialBorrowerSensitiveDataQuery;
        private readonly IQueryProvider<OpportunityAudit> _opportunityAuditQuery;

        public OpportunityService(IGetOpportunitiesQuery opportunityQuery,
            IMapper mapper,
            IGetBorrowerSensitiveDataQuery borrowerSensitiveDataQuery,
            IGetCommercialBorrowerSensitiveDataQuery commercialBorrowerSensitiveDataQuery,
            IQueryProvider<OpportunityAudit> opportunityAuditQuery)
        {
            this._opportunityQuery = opportunityQuery ?? throw new ArgumentNullException(nameof(opportunityQuery));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._commercialBorrowerSensitiveDataQuery = commercialBorrowerSensitiveDataQuery ?? throw new ArgumentNullException(nameof(commercialBorrowerSensitiveDataQuery));
            this._borrowerSensitiveDataQuery = borrowerSensitiveDataQuery ?? throw new ArgumentNullException(nameof(borrowerSensitiveDataQuery));
            this._opportunityAuditQuery = opportunityAuditQuery ?? throw new ArgumentNullException(nameof(opportunityAuditQuery));
        }

        public async Task<SearchResultModel<OpportunityModel>> GetListAsync(SearchArgsModel<OpportunityFilterArgs, OpportunitySortField> args, CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = this._opportunityQuery
                .IncludeContact();

            int userCount = 0;
            if (args?.Filter?.UserIds != null)
            {
                userCount = args.Filter.UserIds.Count();
            }

            if (string.IsNullOrWhiteSpace(args?.FullSearch) == false)
            {
                var val = args.FullSearch.Trim();
                query.Search(val);
            }

            if (args?.Filter?.UserIds?.Any() == true)
            {
                var ids = args.Filter.UserIds;
                query.FindByUserIds(ids);
            }

            if (args?.Filter?.LeadIds?.Any() == true)
            {
                var ids = args.Filter.LeadIds;
                query.FilterByLeads(ids);
            }

            if (args?.Filter?.CampaignIds?.Any() == true)
            {
                var ids = args.Filter.CampaignIds;
                query.FindByCampaignIds(ids);
            }

            if (args?.SortField != null)
            {
                query = (args.SortOrder ?? SortOrder.Asc) == SortOrder.Asc
                    ? query.OrderBy(args.SortField.Value)
                    : query.OrderByDescending(args.SortField.Value);
            }

            query.Skip(args?.Offset ?? 0).Take(args?.Limit ?? 50);

            var items = await query.ExeсuteAsync(cancellationToken).ConfigureAwait(false);
            var itm = new List<OpportunityModel>();
            var itemList = new List<Synergy.CRM.DAL.Queries.Original.Models.OpportunityModel>();
            foreach (var item in items)
            {
                var opportunityAuditQuery = this._opportunityAuditQuery.Query.Where(x => x.OpportunityNumber == item.OpportunityNumber && x.OpportunityStageId == 3).FirstOrDefault();

                item.FileDateStarted = opportunityAuditQuery?.InsertedOn.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                if (userCount == 1)
                {
                    item.LoanOfficer = null;
                }

                itemList.Add(item);
            }

            var count = query.TotalCount ?? 0;

            return new SearchResultModel<OpportunityModel>
            {
                TotalCount = count,
                List = this._mapper.Map<IEnumerable<OpportunityModel>>(itemList),
            };
        }

        public async Task<OpportunityDetailsModel> FindAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var list = await _opportunityQuery
                .IncludeContact()
                .FindById(id)
                .ExeсuteAsync(cancellationToken)
                .ConfigureAwait(false);

            var item = this._mapper.Map<OpportunityDetailsModel>(list.FirstOrDefault() ?? throw new NotFoundException());

            return item;
        }

        public async Task<string> GetSensitiveData(Guid opportunityId, Guid borrowerId, OpportunitySensitiveDataField field, CancellationToken cancellationToken = default(CancellationToken))
        {
            switch (field)
            {
                case OpportunitySensitiveDataField.DayOfBirth:
                    return (await this._borrowerSensitiveDataQuery
                        .FindById(borrowerId)
                        .FilterByOpportunity(opportunityId)
                        .ExeсuteAsync(cancellationToken)
                        .ConfigureAwait(false)).DateOfBirth;

                case OpportunitySensitiveDataField.SSN:
                    return (await this._borrowerSensitiveDataQuery
                        .FindById(borrowerId)
                        .FilterByOpportunity(opportunityId)
                        .ExeсuteAsync(cancellationToken)
                        .ConfigureAwait(false)).SSN;

                case OpportunitySensitiveDataField.TaxIdNumber:
                    return (await this._commercialBorrowerSensitiveDataQuery
                        .FindById(borrowerId)
                        .FilterByOpportunity(opportunityId)
                        .ExeсuteAsync(cancellationToken)
                        .ConfigureAwait(false)).TaxIdNumber;
                default:
                    throw new ModelStateException(nameof(field), $"Invalid field value. Possible values are: {string.Join(",", Enum.GetNames(typeof(OpportunitySensitiveDataField)))}");
            }
        }

        public StringBuilder GetPdfToExport(string opportunityNo, CancellationToken cancellationToken = default)
        {
            var item = _opportunityQuery.GetPdfToExport(opportunityNo);
            var totalEstimatedTaxDisbursement = item.EstimatedCountyTaxDue ?? 0 + item.EstimatedIsdTaxDue ??
                                                0 + item.EstimatedMudTaxDue ?? 0 + item.EstimatedMudTaxDue ?? 0 +
                                                              item.AttorneyCountyFee ?? 0 + item.AttorneyIsdFee ?? 0 + item.CountyConstableFees ?? 0;

            var totalEstimatedClosingCosts = item.TotalEstimatedClosingCost;

            var term = item.TaxLoanTerm;

            string newTerm;

            if (term == null)
            {
                newTerm = "0 Years/0 Months";
            }
            else
            {
                var years = term / 12;
                var months = term % 12;
                newTerm = $"{years} Years/ {months} Months";
            }

            var result = new PdfExportModel
            {
                BorrowersName = item.BorrowersName,
                PropertyAddress = item.PropertyAddress,
                PreparationDate = $"{item.PreparationDate:MM/dd/yyyy}",
                InterestRate = Math.Round(item.InterestRate ?? 0m, 2),
                PaymentAmount = Math.Round(item.PaymentAmount ?? 0m, 2),
                EstimatedClosingDate = $"{item.EstimatedClosingDate:MM/dd/yyyy}",
                FirstPaymentDate = $"{item.FirstPaymentDate:MM/dd/yyyy}",
                EstimatedCountyTaxDue = Math.Round(item.EstimatedCountyTaxDue ?? 0m, 2),
                EstimatedIsdTaxDue = Math.Round(item.EstimatedIsdTaxDue ?? 0m, 2),
                EstimatedMudTaxDue = Math.Round(item.EstimatedMudTaxDue ?? 0m, 2),
                EstimatedOtherTaxDue = Math.Round(item.EstimatedOtherTaxDue ?? 0, 2),
                AttorneyCountyFee = Math.Round(item.AttorneyCountyFee ?? 0m, 2),
                AttorneyIsdFee = Math.Round(item.AttorneyIsdFee ?? 0m, 2),
                CountyConstableFees = Math.Round(item.CountyConstableFees ?? 0m, 2),
                BaseClosingCost = Math.Round(item.BaseClosingCost ?? 0m, 2),
                OrganisationPercent = Math.Round(item.OrganisationPercent ?? 0m, 2),
                LenderCredit = Math.Round(item.LenderCredit ?? 0m, 2),
                PrepayPenalty = Math.Round(item.PrepayPenalty ?? 0m, 2),
                TotalEstimatedClosingCost = Math.Round(totalEstimatedClosingCosts ?? 0m, 2),
                TotalAboveClosingCost = Math.Round(totalEstimatedClosingCosts ?? 0m, 2),
            };
            var totalEstimatedTaxDisbursements = result.EstimatedCountyTaxDue + result.EstimatedIsdTaxDue +
                                                 result.EstimatedMudTaxDue + result.EstimatedOtherTaxDue +
                                                 result.AttorneyCountyFee + result.AttorneyIsdFee +
                                                 result.CountyConstableFees;
            result.TotalEstimatedTaxDisbursements = Math.Round(totalEstimatedTaxDisbursements ?? 0m, 2);
            result.TotalAboveEstimatedDisbursmentTax = Math.Round(totalEstimatedTaxDisbursements ?? 0m, 2);
            result.EstimatedTaxLoanAmount = result.TotalAboveEstimatedDisbursmentTax + result.TotalAboveClosingCost;
            result.TaxLoanTerm = newTerm;
            var stringResult = GetPdfStringBuilder(result);
            return stringResult;
        }

        private StringBuilder GetPdfStringBuilder(PdfExportModel pdf)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<div style='text-align:center; font-size:10px;font-weight:bold;'>PROPERTY TAX LOAN<br/>-- WORKSHEET -- <br/><br/></div>");
            sb.Append("<table width='100%' border='1' align='center' cellspacing='0'>");
            sb.Append("<tr>");
            sb.Append($"<td colspan='3' style='padding:10px; font-family:Arial, Helvetica, sans-serif; font-size:10px; text-align: left;'>Name of Borrower (s): {pdf.BorrowersName}</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append($"<td colspan='3' style='padding:10px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: left;'>Property Address: {pdf.PropertyAddress}</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td colspan='3' style='padding:10px; font-family:Arial, Helvetica, sans-serif; font-size:10px;'>&nbsp;</td>");
            sb.Append("</tr>");
            sb.Append("<tr >");
            sb.Append($"<td style='padding:5px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: left;'>Preparation Date:{pdf.PreparationDate}</td>");
            sb.Append($"<td style='padding:5px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: left;'>Interest Rate: {pdf.InterestRate} % </td>");
            sb.Append($"<td style='padding:5px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: left;'>Payment Amount: $ {pdf.PaymentAmount} </td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append($"<td style='padding:5px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: left;'>Estimated Closing Date: {pdf.EstimatedClosingDate} </td>");
            sb.Append($"<td style='padding:5px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: left;'>Tax Loan Term: {pdf.TaxLoanTerm} </td>");
            sb.Append($"<td style='padding:5px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: left;'>1st Payment Date:{pdf.FirstPaymentDate}</td>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("<br/>");
            sb.Append("<table width='100%' border='1' align='center' cellspacing='0'>");
            sb.Append("<tr>");
            sb.Append("<td colspan='2' bgcolor='#CCC' style='text-align:center; padding:3px; font-family:Arial, Helvetica, sans-serif; font-size:10px;'>Estimated Tax & Fee Amounts</td>");
            sb.Append("<td colspan='2' bgcolor='#CCC' style='text-align:center; padding:3px; font-family:Arial, Helvetica, sans-serif; font-size:10px;'>Estimated Closing Costs</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td style='padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: left;'>Estimated Taxes Due - County</td>");
            sb.Append($"<td style='padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: right;'> $ {pdf.EstimatedCountyTaxDue} </td>");
            sb.Append("<td style='padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: left;'>Base Closing Costs</td>");
            sb.Append($"<td style='padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: right;'> $ {pdf.BaseClosingCost} </td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td style='padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: left;'>Estimated Taxes Due - ISD </td>");
            sb.Append($"<td style='padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: right;'> $ {pdf.EstimatedIsdTaxDue} </td>");
            sb.Append("<td style='padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: left;'>Origination %</td>");
            sb.Append($"<td style='padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: right;'> $ {pdf.OrganisationPercent} </td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td style='padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: left;'>Estimated Taxes Due - MUD </td>");
            sb.Append($"<td style='padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: right;'> $ {pdf.EstimatedMudTaxDue} </td>");
            sb.Append("<td style='padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: left;'>LENDER CREDIT</td>");
            sb.Append($"<td style='padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: right;'> $ {pdf.LenderCredit} </td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td style='padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: left;'>Estimated Taxes Due - OTHER </td>");
            sb.Append($"<td style='padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: right;'> $ {pdf.EstimatedOtherTaxDue} </td>");
            sb.Append("<td style='padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: left;'>&nbsp;</td>");
            sb.Append("<td style='padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: right;'> $ </td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td style='padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: left;'>Collection Attorney fees - County</td>");
            sb.Append($"<td style='padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: right;'> $ {pdf.AttorneyCountyFee} </td>");
            sb.Append("<td style='padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: left;'>&nbsp;</td>");
            sb.Append("<td style='padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: right;'> $ </td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td style='padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: left;'>Collection Attorney fees - ISD</td>");
            sb.Append($"<td style='padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: right;'> $ {pdf.AttorneyIsdFee} </td>");
            sb.Append("<td style='padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: left;'>&nbsp;</td>");
            sb.Append("<td style='padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: right;'> $ </td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td style='padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: left;'>Sheriff / County Constable Fees</td>");
            sb.Append($"<td style='padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: right;'> $ {pdf.CountyConstableFees} </td>");
            sb.Append("<td style='padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: left;'>&nbsp;</td>");
            sb.Append("<td style='padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: right;'> $ </td>");
            sb.Append("</tr>");
            sb.Append("<tr bgcolor='#CCC' style='padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px;'>");
            sb.Append("<td style='text-align:left;'>Total Estimated Tax Disbursements:</td>");
            sb.Append($"<td style='text-align:right;'> $ {pdf.TotalEstimatedTaxDisbursements} </td>");
            sb.Append("<td style='text-align:left;'>Total Estimated Closing Costs:</td>");
            sb.Append($"<td style='text-align:right;'> $ {pdf.TotalEstimatedClosingCost} </td>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("<br/>");
            sb.Append("<table width='100%' border='1' align='center' cellspacing='0'>");
            sb.Append("<tr>");
            sb.Append("<td colspan='2' bgcolor='#CCC' style='text-align:center; padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px; font-weight:bold;'>Estimated Transaction Summary</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td style='padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px;'>&nbsp;</td>");
            sb.Append("<td style='padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px;'>&nbsp;</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td style='padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: left;'>Total Estimated Tax Disbursements, from above</td>");
            sb.Append($"<td style='padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: right;'> $ {pdf.TotalAboveEstimatedDisbursmentTax} </td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td style='padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: left;'>Total Estimated Closing Costs, from above</td>");
            sb.Append($"<td style='padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: right;'> $ {pdf.TotalAboveClosingCost} </td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td style='padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: left;'>Prepayment Penalty , if applicable: [this % and time period will not affect Loan Amt Below]");
            sb.Append("</td>");
            sb.Append($"<td style='padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px;text-align: right;'> $ {pdf.PrepayPenalty} </td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td bgcolor='#CCC' style='text-align:center; padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px; font-weight:bold;'>Estimated Tax Loan Amount: </td>");
            sb.Append($"<td bgcolor='#CCC' style='text-align:right; padding:2px; font-family:Arial, Helvetica, sans-serif; font-size:10px; font-weight:bold;'> $ {pdf.EstimatedTaxLoanAmount} </td>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("<br/>");
            sb.Append("<div style='text-align:center; font-family:Arial, Helvetica, sans-serif; font-weight:bold; font-size:10px; padding:2px;'>");
            sb.Append("This worksheet does not constitute a loan application, a verified loan estimate or an extension of");
            sb.Append("credit. Upon your request, and after verification of tax amounts and other criteria, our Processing");
            sb.Append("Department will issue an official Loan Disclosure Package, including a Property Tax Pre-Closing");
            sb.Append("Disclosure [Figure: 7 TAC ยง89.506(a)(1)].");
            sb.Append("<br/><br/>");
            sb.Append("Your tax office may offer delinquent tax installment plans that may be less costly to you. You can");
            sb.Append("request information about the availability of these plans from the tax office.");
            sb.Append("</div>");
            return sb;
        }
    }
}
