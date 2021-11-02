using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Synergy.CRM.DAL.Commands.Models.Results.MailMerge;
using Synergy.DataAccess.Abstractions.Commands.Interfaces;
using Synergy.DataAccess.Context;

namespace Synergy.CRM.DAL.Commands.Queries
{
    public class GetMailMergePropertyFieldsQuery : CollectionQuery<(List<Guid> propertyIdList, Guid campaignId), MailMergePropertyModel>
    {
        private readonly ISynergyContext _synergyContext;
        private readonly ILogger<GetMailMergePropertyFieldsQuery> _logger;

        public GetMailMergePropertyFieldsQuery(ISynergyContext synergyContext, ILogger<GetMailMergePropertyFieldsQuery> logger)
        {
            this._synergyContext = synergyContext ?? throw new ArgumentNullException(nameof(synergyContext));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task<IEnumerable<MailMergePropertyModel>> ExecuteAsync((List<Guid> propertyIdList, Guid campaignId) args, CancellationToken cancellationToken = default)
        {
            this._logger.LogInformation("Start loading property data.");

            var (propertyIdList, campaignId) = args;
            var propertyList = this._synergyContext.Property.Where(x => propertyIdList.Contains(x.Id)
                                                                     && x.Lead.CampaignLeads.Any(c => c.CampaignId == campaignId)
                                                                     && x.DeletedOn == null);

            var initialData = await propertyList.Select(x => new MailMergePropertyModel
            {
                InternalPropertyId = x.Id,
                County = x.County.Name,
                PropertyAmountDue = x.TotalAmountDue,
                LandUseCode = x.LandUseCode,
                GeneralLandUseCode = x.GeneralLandUseCode.Name,
                InternalLandUseCode = x.InternalLandUseCode.Description,
                Owner = x.Lead.AccountName,
                PropertyAddress = x.Address,
                PropertyCity = x.City,
                Homestead = x.Homestead,
                PropertyState = x.State.Abbreviation,
                PropertyStateId = x.StateId,
                PropertyZipCode = x.ZipCode,
                LeadAppraisedValue = x.Lead.AppraisedValueOfProperties == null ? 0 : x.Lead.AppraisedValueOfProperties.Value,
                LeadAmountDue = x.Lead.TotalAmountDueProperties == null ? 0 : x.Lead.TotalAmountDueProperties.Value,
                MailingAddress1 = x.Lead.MailingAddress1,
                MailingAddress2 = x.Lead.MailingAddress2,
                MailingAddress3 = x.Lead.MailingAddress3,
                MailingCity = x.Lead.MailingCity,
                MailingState = x.Lead.MailingState.Abbreviation,
                MailingZipCode = x.Lead.MailingZipCode,
                DoNotContact = x.Lead.DoNotContact,
                ParcelId = x.ParcelId,
                LandAcres = x.LandAcres == null ? 0 : x.LandAcres.Value,
                BuildingSqFt = x.BuildingSqFt == null ? 0 : x.BuildingSqFt.Value,
                RUAmount = x.RUAmount == null ? 0 : x.RUAmount.Value,
                RULTV = x.RULTVPercent == null ? 0 : x.RULTVPercent.Value * 100,
                LTV = x.LTVPercent == null ? 0 : x.LTVPercent.Value * 100,
                LegalDescription = x.LegalDescription,
                YearBuilt = x.YearBuilt,
            })
            .ToListAsync(cancellationToken).ConfigureAwait(false);

            this._logger.LogInformation("Initial property data loaded.");

            var campaign = await this._synergyContext.Campaign
                .Select(val => new
                {
                    Id = val.Id,
                    CampaignName = val.CampaignName,
                    CampaignType = val.CampaignType.Description,
                    CampaignSubType = val.CampaignSubType.Description,
                    CreatedDate = val.CreateDate,
                    Description = val.Description,
                    TargetDate = val.TargetDate,
                    Note = val.Note,
                    AssignedUserFirstName = val.AssignedUser.FirstName,
                    AssignedUserLastName = val.AssignedUser.LastName,
                })
                .FirstOrDefaultAsync(x => x.Id == campaignId)
                .ConfigureAwait(false);

            this._logger.LogInformation("Campaign data loaded.");

            var propertyValuations = await propertyList
                .Join(this._synergyContext.PropertyValuation.Where(v => v.IsActive == true && v.DeletedOn == null),
                x => x.Id,
                x => x.PropertyId,
                (d, val) => new
                {
                    PropertyId = d.Id,
                    AppraisedYear = val.AppraisedYear,
                    AppraisedValue = val.AppraisedValue,
                    LandValue = val.LandValue,
                    BuildingValue = val.ImprovementValue,
                })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            this._logger.LogInformation("Property Valuations data loaded.");

            var states = initialData.Select(x => x.PropertyStateId).Distinct().ToList();
            var taxes = await this._synergyContext.StateTaxe
                .Where(x => x.DeletedOn == null && states.Contains(x.StateId))
                .Select(x => new { x.StateId, x.TaxRate })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            this._logger.LogInformation("States data loaded.");

            var mergedData = initialData
                .GroupJoin(propertyValuations,
                    x => x.InternalPropertyId,
                    x => x.PropertyId,
                    (x, val) => (Property: x, Valuation: val.OrderByDescending(v => v.AppraisedYear).FirstOrDefault()))
                .GroupJoin(taxes,
                    x => x.Property.PropertyStateId,
                    x => x.StateId,
                    (x, val) => (x.Property, x.Valuation, val.FirstOrDefault()?.TaxRate));

            this._logger.LogInformation("Data merged in memory.");

            foreach (var t in mergedData)
            {
                var (exportModel, appraisedValue, taxRate) = t;

                exportModel.Campaign = new MailMergeCampaignModel
                {
                    CampaignName = campaign.CampaignName,
                    CampaignType = campaign.CampaignType,
                    CampaignSubType = campaign.CampaignSubType,
                    CreatedDate = campaign.CreatedDate,
                    Description = campaign.Description,
                    TargetDate = campaign.TargetDate,
                    Note = campaign.Note,
                    AssignedUser = campaign.AssignedUserFirstName + " " + campaign.AssignedUserLastName,
                };

                exportModel.AppraisedValue = appraisedValue?.AppraisedValue ?? 0;
                exportModel.LandValue = appraisedValue?.LandValue ?? 0;
                exportModel.BuildingValue = appraisedValue?.BuildingValue ?? 0;

                if (taxRate != null)
                {
                    var rate = exportModel.AppraisedValue * taxRate;
                    exportModel.TaxRatio = rate > 0 ? exportModel.PropertyAmountDue / rate.Value * 100 : 0;
                }
            }

            this._logger.LogInformation("Mail merge property fields model created.");

            return mergedData.Select(x => x.Property).ToList();
        }
    }
}
