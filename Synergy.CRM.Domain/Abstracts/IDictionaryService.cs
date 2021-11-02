using System.Threading;
using System.Threading.Tasks;
using Synergy.Common.Domain.Models.Common;
using Synergy.CRM.Models;

namespace Synergy.CRM.Domain.Abstracts
{
    public interface IDictionaryService
    {
        Task<SearchResultModel<FastEntityModel<int>>> GetEnumDictionaryAsync<TEnum>(CancellationToken cancellationToken = default(CancellationToken))
            where TEnum : System.Enum;

        Task<SearchResultModel<CampaignTypeModel>> GetCampaignTypesAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<SearchResultModel<CampaignRuleFieldModel>> GetRuleFieldsAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<SearchResultModel<FastEntityModel<int>>> GetCountiesAsync(SearchArgsModel<CountySearchArgs, CountySortField> args, CancellationToken cancellationToken = default(CancellationToken));

        Task<SearchResultModel<FastEntityModel<int>>> GetPropertyTypesAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<SearchResultModel<FastEntityModel<int>>> GetMonthlyPrepayFieldsAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<SearchResultModel<FastEntityModel<int>>> GetPercentagePrepayFieldsAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<SearchResultModel<FastEntityModel<int>>> GetCollectingEntityTypesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
