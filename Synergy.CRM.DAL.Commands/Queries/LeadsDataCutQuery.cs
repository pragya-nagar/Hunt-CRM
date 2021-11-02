using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Synergy.CRM.DAL.Commands.Models.Results;
using Synergy.DataAccess.Abstractions.Commands.Interfaces;
using Synergy.DataAccess.Context;
using Synergy.DataAccess.Entities;
using Synergy.DataAccess.Enum;

namespace Synergy.CRM.DAL.Commands.Queries
{
    public class LeadsDataCutQuery : CollectionQuery<Guid, LeadsDataCutModel>
    {
        private readonly ISynergyContext _context;
        private readonly ILogger _logger;

        public LeadsDataCutQuery(ISynergyContext context, ILogger<LeadsDataCutQuery> logger)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task<IEnumerable<LeadsDataCutModel>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            // Get campaign rules from DB
            var items = await (from rc in this._context.CampaignRuleCampaign
                               join rule in this._context.CampaignRule on rc.CampaignRuleId equals rule.Id
                               join ruleItem in this._context.CampaignRuleItem on rule.Id equals ruleItem.CampaignRuleId
                               where rc.CampaignId == id
                               select new
                               {
                                   rule.Id,
                                   Item = new
                                   {
                                       ruleItem.Value,
                                       LogicType = (LogicType)ruleItem.CampaignLogicTypeId,
                                       RuleField = (RuleField)ruleItem.CampaignRuleFieldId,
                                   },
                               }).ToListAsync(cancellationToken).ConfigureAwait(false);

            // Group rule items by rule
            var rules = items.GroupBy(x => x.Id).Select(x => new
            {
                Id = x.Key,
                Items = x.Select(y => y.Item),
            }).ToList();

            this._logger.LogInformation("Get {RulesCount} rules to calculate for Campaign {id}", rules.Count, id);

            if (rules.Any() == false)
            {
                return Enumerable.Empty<LeadsDataCutModel>();
            }

            // Get campaign with counties
            Campaign campaign = await this._context.Campaign.Include(c => c.CampaignCounty).AsNoTracking().Where(x => x.Id == id)
                .SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            List<int> countyIds = campaign.CampaignCounty.Select(cc => cc.CountyId).ToList();
            int stateId = campaign.StateId;

            IQueryable<LeadsDataCutModel> queries = null;

            foreach (var rule in rules)
            {
                this._logger.LogInformation("Start processing {RuleId} for Campaign {id}", rule.Id, id);

                // Build query to get property with Loan delinquency
                IQueryable<Property> query = null;

                if (countyIds.Any())
                {
                    query = this._context.Property.AsNoTracking()
                        .Where(p => p.DeletedOn == null && p.Delinquencies.Any(d => d.IsLoan) && countyIds.Contains(p.CountyId));
                }
                else
                {
                    query = this._context.Property.AsNoTracking()
                        .Where(p => p.DeletedOn == null && p.Delinquencies.Any(d => d.IsLoan) && p.StateId == stateId);
                }

                foreach (var item in rule.Items)
                {
                    if (string.IsNullOrWhiteSpace(item.Value))
                    {
                        continue;
                    }

                    var value = item.Value.ToLower();
                    switch (item.RuleField)
                    {
                        case RuleField.AccountName:
                            switch (item.LogicType)
                            {
                                case LogicType.Contains:
                                    query = query.Where(p => p.Lead.AccountName.ToLower().Contains(value) == true);

                                    break;
                                case LogicType.DoesNotContain:
                                    query = query.Where(p => p.Lead.AccountName.ToLower().Contains(value) == false);

                                    break;
                                case LogicType.Equal:
                                    query = query.Where(p => p.Lead.AccountName.ToLower() == value);

                                    break;
                                case LogicType.NotEqual:
                                    query = query.Where(p => p.Lead.AccountName.ToLower() != value);

                                    break;
                            }

                            break;
                        case RuleField.GeneralLandUseCode when Enum.TryParse(item.Value, out DataAccess.Enum.GeneralLandUseCode generalLandUseCodeValue):
                            var generalLandUseCodeId = (int)generalLandUseCodeValue;
                            query = query.Where(p => p.GeneralLandUseCodeId == generalLandUseCodeId);

                            break;
                        case RuleField.TotalAmountDue when decimal.TryParse(value, out var val):
                            switch (item.LogicType)
                            {
                                case LogicType.LessThan:
                                    query = query.Where(p => p.TotalAmountDue < val);

                                    break;
                                case LogicType.LessThanOrEqual:
                                    query = query.Where(p => p.TotalAmountDue <= val);

                                    break;
                                case LogicType.GreaterThan:
                                    query = query.Where(p => p.TotalAmountDue > val);

                                    break;
                                case LogicType.GreaterThanOrEqual:
                                    query = query.Where(p => p.TotalAmountDue >= val);

                                    break;
                            }

                            break;
                        case RuleField.TotalAmountDueProperties when decimal.TryParse(value, out var val):
                            switch (item.LogicType)
                            {
                                case LogicType.LessThan:
                                    query = query.Where(p => p.Lead.TotalAmountDueProperties < val);

                                    break;
                                case LogicType.LessThanOrEqual:
                                    query = query.Where(p => p.Lead.TotalAmountDueProperties <= val);

                                    break;
                                case LogicType.GreaterThan:
                                    query = query.Where(p => p.Lead.TotalAmountDueProperties > val);

                                    break;
                                case LogicType.GreaterThanOrEqual:
                                    query = query.Where(p => p.Lead.TotalAmountDueProperties >= val);

                                    break;
                            }

                            break;
                        case RuleField.LTVPercent when decimal.TryParse(value, out var val):
                            switch (item.LogicType)
                            {
                                case LogicType.LessThan:
                                    query = query.Where(p => p.LTVPercent * 100 < val);

                                    break;
                                case LogicType.LessThanOrEqual:
                                    query = query.Where(p => p.LTVPercent * 100 <= val);

                                    break;
                                case LogicType.GreaterThan:
                                    query = query.Where(p => p.LTVPercent * 100 > val);

                                    break;
                                case LogicType.GreaterThanOrEqual:
                                    query = query.Where(p => p.LTVPercent * 100 >= val);

                                    break;
                            }

                            break;
                        case RuleField.LTVPercentOfProperties when decimal.TryParse(value, out var val):
                            switch (item.LogicType)
                            {
                                case LogicType.LessThan:
                                    query = query.Where(p => p.Lead.AppraisedValueOfProperties > 0 && (p.Lead.TotalAmountDueProperties / p.Lead.AppraisedValueOfProperties) * 100 < val);

                                    break;
                                case LogicType.LessThanOrEqual:
                                    query = query.Where(p => p.Lead.AppraisedValueOfProperties > 0 && (p.Lead.TotalAmountDueProperties / p.Lead.AppraisedValueOfProperties) * 100 <= val);

                                    break;
                                case LogicType.GreaterThan:
                                    query = query.Where(p => p.Lead.AppraisedValueOfProperties > 0 && (p.Lead.TotalAmountDueProperties / p.Lead.AppraisedValueOfProperties) * 100 > val);

                                    break;
                                case LogicType.GreaterThanOrEqual:
                                    query = query.Where(p => p.Lead.AppraisedValueOfProperties > 0 && (p.Lead.TotalAmountDueProperties / p.Lead.AppraisedValueOfProperties) * 100 >= val);

                                    break;
                            }

                            break;
                        case RuleField.AppraisedValue when decimal.TryParse(value, out var val):
                            switch (item.LogicType)
                            {
                                case LogicType.LessThan:
                                    query = query.Where(p => p.PropertyValuations.Where(x => x.IsActive).Any(x => x.AppraisedValue < val));

                                    break;
                                case LogicType.LessThanOrEqual:
                                    query = query.Where(p => p.PropertyValuations.Where(x => x.IsActive).Any(x => x.AppraisedValue <= val));

                                    break;
                                case LogicType.GreaterThan:
                                    query = query.Where(p => p.PropertyValuations.Where(x => x.IsActive).Any(x => x.AppraisedValue > val));

                                    break;
                                case LogicType.GreaterThanOrEqual:
                                    query = query.Where(p => p.PropertyValuations.Where(x => x.IsActive).Any(x => x.AppraisedValue >= val));

                                    break;
                            }

                            break;
                        case RuleField.AppraisedValueOfProperties when decimal.TryParse(value, out var val):
                            switch (item.LogicType)
                            {
                                case LogicType.LessThan:
                                    query = query.Where(p => p.Lead.AppraisedValueOfProperties < val);

                                    break;
                                case LogicType.LessThanOrEqual:
                                    query = query.Where(p => p.Lead.AppraisedValueOfProperties <= val);

                                    break;
                                case LogicType.GreaterThan:
                                    query = query.Where(p => p.Lead.AppraisedValueOfProperties > val);

                                    break;
                                case LogicType.GreaterThanOrEqual:
                                    query = query.Where(p => p.Lead.AppraisedValueOfProperties >= val);

                                    break;
                            }

                            break;
                        case RuleField.Over65SurvivingSpouse when bool.TryParse(value, out var val):
                            switch (item.LogicType)
                            {
                                case LogicType.EqualBool:
                                    query = query.Where(x => x.Over65SurvivingSpouse == val);

                                    break;
                                case LogicType.NotEqualBool:
                                    query = query.Where(x => x.Over65SurvivingSpouse != val);

                                    break;
                            }

                            break;
                        case RuleField.DisabilityExemption when bool.TryParse(value, out var val):
                            switch (item.LogicType)
                            {
                                case LogicType.EqualBool:
                                    query = query.Where(x => x.DisabilityExemption == val);

                                    break;
                                case LogicType.NotEqualBool:
                                    query = query.Where(x => x.DisabilityExemption != val);

                                    break;
                            }

                            break;
                        case RuleField.Veteran when bool.TryParse(value, out var val):
                            switch (item.LogicType)
                            {
                                case LogicType.EqualBool:
                                    query = query.Where(x => x.Veteran == val);

                                    break;
                                case LogicType.NotEqualBool:
                                    query = query.Where(x => x.Veteran != val);

                                    break;
                            }

                            break;
                        case RuleField.DoNotContact when bool.TryParse(value, out var val):
                            switch (item.LogicType)
                            {
                                case LogicType.EqualBool:
                                    query = query.Where(x => x.Lead.DoNotContact == val);

                                    break;
                                case LogicType.NotEqualBool:
                                    query = query.Where(x => x.Lead.DoNotContact != val);

                                    break;
                            }

                            break;
                    }
                }

                IQueryable<LeadsDataCutModel> project = query.Select(x => new LeadsDataCutModel
                {
                    Id = x.LeadId,
                    TotalAmountDue = x.Lead.TotalAmountDueProperties,
                });

                queries = queries == null ? project : queries.Union(project);
            }

            List<LeadsDataCutModel> list = await queries.ToListAsync(cancellationToken).ConfigureAwait(false);
            return list.GroupBy(x => x.Id).SelectMany(x => x.Take(1));
        }
    }
}
