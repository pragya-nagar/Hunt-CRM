using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
    public class GetPropertiesApplyRuleQuery : BaseQuery<Property>, IGetPropertiesApplyRuleQuery
    {
        private IMapper _mapper;
        private DbSet<Property> _entity;

        #region query builder
        public GetPropertiesApplyRuleQuery(ISynergyContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this._entity = context.Property;
        }

        public IGetPropertiesApplyRuleQuery FindById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IGetPropertiesApplyRuleQuery FilterByCounty(int countyId)
        {
            this.andAlsoPredicates.Add(p => p.CountyId == countyId);
            return this;
        }

        public IGetPropertiesApplyRuleQuery FilterByRules(IEnumerable<CampaignRulesModel> rules)
        {
            List<Expression<Func<Property, bool>>> orAlso = new List<Expression<Func<Property, bool>>>();

            foreach (var rule in rules)
            {
                List<Expression<Func<Property, bool>>> andAlso = new List<Expression<Func<Property, bool>>>();

                foreach (var item in rule.CampaignRuleItems)
                {
                    if (string.IsNullOrWhiteSpace(item.Value))
                    {
                        continue;
                    }

                    string value = item.Value.ToLower();
                    decimal decimalValue;
                    bool boolValue;
                    DataAccess.Enum.GeneralLandUseCode generalLandUseCodeValue;
                    switch (item.RuleField)
                    {
                        case RuleField.AccountName:
                            switch (item.LogicType)
                            {
                                case LogicType.Contains:
                                    andAlso.Add(p => p.Lead.AccountName.ToLower().Contains(value));
                                    break;
                                case LogicType.DoesNotContain:
                                    andAlso.Add(p => !p.Lead.AccountName.ToLower().Contains(value));
                                    break;
                                case LogicType.Equal:
                                    andAlso.Add(p => p.Lead.AccountName.ToLower() == value.ToLower());
                                    break;
                                case LogicType.NotEqual:
                                    andAlso.Add(p => p.Lead.AccountName.ToLower() != value.ToLower());
                                    break;
                            }

                            break;
                        case RuleField.GeneralLandUseCode:
                            if (Enum.TryParse<DataAccess.Enum.GeneralLandUseCode>(item.Value, out generalLandUseCodeValue))
                            {
                                int generalLandUseCodeId = (int)generalLandUseCodeValue;
                                andAlso.Add(p => p.GeneralLandUseCodeId == generalLandUseCodeId);
                            }

                            break;
                        case RuleField.TotalAmountDue:
                            switch (item.LogicType)
                            {
                                case LogicType.LessThan:
                                    if (decimal.TryParse(value, out decimalValue))
                                    {
                                        andAlso.Add(p => p.TotalAmountDue < decimalValue);
                                    }

                                    break;
                                case LogicType.LessThanOrEqual:
                                    if (decimal.TryParse(value, out decimalValue))
                                    {
                                        andAlso.Add(p => p.TotalAmountDue <= decimalValue);
                                    }

                                    break;
                                case LogicType.GreaterThan:
                                    if (decimal.TryParse(value, out decimalValue))
                                    {
                                        andAlso.Add(p => p.TotalAmountDue > decimalValue);
                                    }

                                    break;
                                case LogicType.GreaterThanOrEqual:
                                    if (decimal.TryParse(value, out decimalValue))
                                    {
                                        andAlso.Add(p => p.TotalAmountDue >= decimalValue);
                                    }

                                    break;
                            }

                            break;
                        case RuleField.TotalAmountDueProperties:
                            switch (item.LogicType)
                            {
                                case LogicType.LessThan:
                                    if (decimal.TryParse(value, out decimalValue))
                                    {
                                        andAlso.Add(p => p.Lead.TotalAmountDueProperties < decimalValue);
                                    }

                                    break;
                                case LogicType.LessThanOrEqual:
                                    if (decimal.TryParse(value, out decimalValue))
                                    {
                                        andAlso.Add(p => p.Lead.TotalAmountDueProperties <= decimalValue);
                                    }

                                    break;
                                case LogicType.GreaterThan:
                                    if (decimal.TryParse(value, out decimalValue))
                                    {
                                        andAlso.Add(p => p.Lead.TotalAmountDueProperties > decimalValue);
                                    }

                                    break;
                                case LogicType.GreaterThanOrEqual:
                                    if (decimal.TryParse(value, out decimalValue))
                                    {
                                        andAlso.Add(p => p.Lead.TotalAmountDueProperties >= decimalValue);
                                    }

                                    break;
                            }

                            break;
                        case RuleField.LTVPercent:
                            switch (item.LogicType)
                            {
                                case LogicType.LessThan:
                                    if (decimal.TryParse(value, out decimalValue))
                                    {
                                        andAlso.Add(p => p.LTVPercent < decimalValue);
                                    }

                                    break;
                                case LogicType.LessThanOrEqual:
                                    if (decimal.TryParse(value, out decimalValue))
                                    {
                                        andAlso.Add(p => p.LTVPercent <= decimalValue);
                                    }

                                    break;
                                case LogicType.GreaterThan:
                                    if (decimal.TryParse(value, out decimalValue))
                                    {
                                        andAlso.Add(p => p.LTVPercent > decimalValue);
                                    }

                                    break;
                                case LogicType.GreaterThanOrEqual:
                                    if (decimal.TryParse(value, out decimalValue))
                                    {
                                        andAlso.Add(p => p.LTVPercent >= decimalValue);
                                    }

                                    break;
                            }

                            break;
                        case RuleField.LTVPercentOfProperties:
                            switch (item.LogicType)
                            {
                                case LogicType.LessThan:
                                    if (decimal.TryParse(value, out decimalValue))
                                    {
                                        andAlso.Add(x =>
                                        x.Lead.AppraisedValueOfProperties > 0 && (x.Lead.TotalAmountDueProperties / x.Lead.AppraisedValueOfProperties) * 100 < decimalValue);
                                    }

                                    break;
                                case LogicType.LessThanOrEqual:
                                    if (decimal.TryParse(value, out decimalValue))
                                    {
                                        andAlso.Add(x =>
                                        x.Lead.AppraisedValueOfProperties > 0 && (x.Lead.TotalAmountDueProperties / x.Lead.AppraisedValueOfProperties) * 100 <= decimalValue);
                                    }

                                    break;
                                case LogicType.GreaterThan:
                                    if (decimal.TryParse(value, out decimalValue))
                                    {
                                        andAlso.Add(x =>
                                        x.Lead.AppraisedValueOfProperties > 0 && (x.Lead.TotalAmountDueProperties / x.Lead.AppraisedValueOfProperties) * 100 > decimalValue);
                                    }

                                    break;
                                case LogicType.GreaterThanOrEqual:
                                    if (decimal.TryParse(value, out decimalValue))
                                    {
                                        andAlso.Add(x =>
                                        x.Lead.AppraisedValueOfProperties > 0 && (x.Lead.TotalAmountDueProperties / x.Lead.AppraisedValueOfProperties) * 100 >= decimalValue);
                                    }

                                    break;
                            }

                            break;
                        case RuleField.AppraisedValue:
                            switch (item.LogicType)
                            {
                                case LogicType.LessThan:
                                    if (decimal.TryParse(value, out decimalValue))
                                    {
                                        andAlso.Add(p => p.PropertyValuations.Where(x => x.IsActive).Select(x => x.AppraisedValue).FirstOrDefault() < decimalValue);
                                    }

                                    break;
                                case LogicType.LessThanOrEqual:
                                    if (decimal.TryParse(value, out decimalValue))
                                    {
                                        andAlso.Add(p => p.PropertyValuations.Where(x => x.IsActive).Select(x => x.AppraisedValue).FirstOrDefault() <= decimalValue);
                                    }

                                    break;
                                case LogicType.GreaterThan:
                                    if (decimal.TryParse(value, out decimalValue))
                                    {
                                        andAlso.Add(p => p.PropertyValuations.Where(x => x.IsActive).Select(x => x.AppraisedValue).FirstOrDefault() > decimalValue);
                                    }

                                    break;
                                case LogicType.GreaterThanOrEqual:
                                    if (decimal.TryParse(value, out decimalValue))
                                    {
                                        andAlso.Add(p => p.PropertyValuations.Where(x => x.IsActive).Select(x => x.AppraisedValue).FirstOrDefault() >= decimalValue);
                                    }

                                    break;
                            }

                            break;
                        case RuleField.AppraisedValueOfProperties:
                            switch (item.LogicType)
                            {
                                case LogicType.LessThan:
                                    if (decimal.TryParse(value, out decimalValue))
                                    {
                                        andAlso.Add(p => p.Lead.AppraisedValueOfProperties < decimalValue);
                                    }

                                    break;
                                case LogicType.LessThanOrEqual:
                                    if (decimal.TryParse(value, out decimalValue))
                                    {
                                        andAlso.Add(p => p.Lead.AppraisedValueOfProperties <= decimalValue);
                                    }

                                    break;
                                case LogicType.GreaterThan:
                                    if (decimal.TryParse(value, out decimalValue))
                                    {
                                        andAlso.Add(p => p.Lead.AppraisedValueOfProperties > decimalValue);
                                    }

                                    break;
                                case LogicType.GreaterThanOrEqual:
                                    if (decimal.TryParse(value, out decimalValue))
                                    {
                                        andAlso.Add(p => p.Lead.AppraisedValueOfProperties >= decimalValue);
                                    }

                                    break;
                            }

                            break;
                        case RuleField.Over65SurvivingSpouse:
                            switch (item.LogicType)
                            {
                                case LogicType.EqualBool:
                                    if (bool.TryParse(value, out boolValue))
                                    {
                                        andAlso.Add(x => x.Over65SurvivingSpouse == boolValue);
                                    }

                                    break;
                                case LogicType.NotEqualBool:
                                    if (bool.TryParse(value, out boolValue))
                                    {
                                        andAlso.Add(x => x.Over65SurvivingSpouse != boolValue);
                                    }

                                    break;
                            }

                            break;
                        case RuleField.DisabilityExemption:
                            switch (item.LogicType)
                            {
                                case LogicType.EqualBool:
                                    if (bool.TryParse(value, out boolValue))
                                    {
                                        andAlso.Add(x => x.DisabilityExemption == boolValue);
                                    }

                                    break;
                                case LogicType.NotEqualBool:
                                    if (bool.TryParse(value, out boolValue))
                                    {
                                        andAlso.Add(x => x.DisabilityExemption != boolValue);
                                    }

                                    break;
                            }

                            break;
                        case RuleField.Veteran:
                            switch (item.LogicType)
                            {
                                case LogicType.EqualBool:
                                    if (bool.TryParse(value, out boolValue))
                                    {
                                        andAlso.Add(x => x.Veteran == boolValue);
                                    }

                                    break;
                                case LogicType.NotEqualBool:
                                    if (bool.TryParse(value, out boolValue))
                                    {
                                        andAlso.Add(x => x.Veteran != boolValue);
                                    }

                                    break;
                            }

                            break;
                        case RuleField.DoNotContact:
                            switch (item.LogicType)
                            {
                                case LogicType.EqualBool:
                                    if (bool.TryParse(value, out boolValue))
                                    {
                                        andAlso.Add(x => x.Lead.DoNotContact == boolValue);
                                    }

                                    break;
                                case LogicType.NotEqualBool:
                                    if (bool.TryParse(value, out boolValue))
                                    {
                                        andAlso.Add(x => x.Lead.DoNotContact != boolValue);
                                    }

                                    break;
                            }

                            break;
                    }
                }

                Expression<Func<Property, bool>> buildedAndPredicate = e => true;

                foreach (var predicate in andAlso)
                {
                    buildedAndPredicate = buildedAndPredicate.AndAlso(predicate);
                }

                orAlso.Add(buildedAndPredicate);
            }

            Expression<Func<Property, bool>> buildedORPredicate = e => false;

            foreach (var predicate in orAlso)
            {
                buildedORPredicate = buildedORPredicate.OrElse(predicate);
            }

            this.andAlsoPredicates.Add(buildedORPredicate);

            return this;
        }

        #endregion

        public IEnumerable<PropertyApplyRulesModel> Exeсute()
        {
            IQueryable<Property> data = this.BuildQuery();
            return this._mapper.Map<List<PropertyApplyRulesModel>>(data.ToList());
        }

        public async Task<IEnumerable<PropertyApplyRulesModel>> ExeсuteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            IQueryable<Property> data = this.BuildQuery();

            return this._mapper.Map<List<PropertyApplyRulesModel>>(await data.ToListAsync(cancellationToken).ConfigureAwait(false));
        }

        private IQueryable<Property> BuildQuery()
        {
            IQueryable<Property> query = this._entity
                .Include(p => p.PropertyValuations)
                .Include(p => p.Lead)
                .Where(this.GetPredicate())
                ;
            return query;
        }
    }
}
