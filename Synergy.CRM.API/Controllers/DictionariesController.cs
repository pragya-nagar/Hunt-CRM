using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Synergy.Common.Domain.Models.Common;
using Synergy.CRM.Domain.Abstracts;
using Synergy.CRM.Models;

namespace Synergy.CRM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DictionariesController : Controller
    {
        private readonly IDictionaryService _dictionaryService;

        public DictionariesController(IDictionaryService dictionaryService)
        {
            this._dictionaryService = dictionaryService;
        }

        [Route("CampaignTypes")]
        [HttpGet]
        [ProducesResponseType(typeof(SearchResultModel<CampaignTypeModel>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetCampaignTypes(CancellationToken cancellationToken = default)
        {
            var res = await _dictionaryService.GetCampaignTypesAsync(cancellationToken).ConfigureAwait(false);
            return this.Ok(res);
        }

        [Route("ContactTypes")]
        [HttpGet]
        [ProducesResponseType(typeof(SearchResultModel<FastEntityModel<int>>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetContactTypes(CancellationToken cancellationToken = default)
        {
            var res = await _dictionaryService.GetEnumDictionaryAsync<DataAccess.Enum.ContactType>(cancellationToken).ConfigureAwait(false);
            return this.Ok(res);
        }

        [Route("GeneralLandUseCodes")]
        [HttpGet]
        [ProducesResponseType(typeof(SearchResultModel<FastEntityModel<int>>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetGeneralLandUseCodes(CancellationToken cancellationToken = default)
        {
            var res = await _dictionaryService.GetEnumDictionaryAsync<DataAccess.Enum.GeneralLandUseCode>(cancellationToken).ConfigureAwait(false);
            return this.Ok(res);
        }

        [Route("LoanTypes")]
        [HttpGet]
        [ProducesResponseType(typeof(SearchResultModel<FastEntityModel<int>>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetLoanTypes(CancellationToken cancellationToken = default)
        {
            var res = await _dictionaryService.GetEnumDictionaryAsync<DataAccess.Enum.LoanType>(cancellationToken).ConfigureAwait(false);
            return this.Ok(res);
        }

        [Route("OpportunityStages")]
        [HttpGet]
        [ProducesResponseType(typeof(SearchResultModel<FastEntityModel<int>>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetOpportunityStages(CancellationToken cancellationToken = default)
        {
            var res = await _dictionaryService.GetEnumDictionaryAsync<DataAccess.Enum.OpportunityStage>(cancellationToken).ConfigureAwait(false);
            return this.Ok(res);
        }

        [Route("RuleFields")]
        [HttpGet]
        [ProducesResponseType(typeof(SearchResultModel<CampaignRuleFieldModel>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetRuleFields(CancellationToken cancellationToken = default)
        {
            var res = await _dictionaryService.GetRuleFieldsAsync(cancellationToken).ConfigureAwait(false);
            return this.Ok(res);
        }

        [Route("Counties")]
        [HttpGet]
        [ProducesResponseType(typeof(SearchResultModel<FastEntityModel<int>>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetCounties([FromQuery]SearchArgsModel<CountySearchArgs, CountySortField> args, CancellationToken cancellationToken = default)
        {
            var res = await _dictionaryService.GetCountiesAsync(args, cancellationToken).ConfigureAwait(false);
            return this.Ok(res);
        }

        [Route("PropertyTypes")]
        [HttpGet]
        [ProducesResponseType(typeof(SearchResultModel<FastEntityModel<int>>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetPropertyTypes(CancellationToken cancellationToken = default)
        {
            var res = await _dictionaryService.GetPropertyTypesAsync(cancellationToken).ConfigureAwait(false);
            return this.Ok(res);
        }

        [Route("CollectingEntityTypes")]
        [HttpGet]
        [ProducesResponseType(typeof(SearchResultModel<FastEntityModel<int>>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetCollectingEntityTypes(CancellationToken cancellationToken = default)
        {
            var res = await _dictionaryService.GetCollectingEntityTypesAsync(cancellationToken).ConfigureAwait(false);
            return this.Ok(res);
        }

        [Route("MonthlyPrepayFields")]
        [HttpGet]
        [ProducesResponseType(typeof(SearchResultModel<FastEntityModel<int>>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetMonthlyPrepayFields(CancellationToken cancellationToken = default)
        {
            var res = await _dictionaryService.GetMonthlyPrepayFieldsAsync(cancellationToken).ConfigureAwait(false);
            return this.Ok(res);
        }

        [Route("PercentagePrepayFields")]
        [HttpGet]
        [ProducesResponseType(typeof(SearchResultModel<FastEntityModel<int>>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetPercentagePrepayFields(CancellationToken cancellationToken = default)
        {
            var res = await _dictionaryService.GetPercentagePrepayFieldsAsync(cancellationToken).ConfigureAwait(false);
            return this.Ok(res);
        }
    }
}