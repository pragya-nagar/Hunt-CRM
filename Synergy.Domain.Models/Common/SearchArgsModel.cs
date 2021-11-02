using Synergy.Domain.Models.Abstracts;

namespace Synergy.Domain.Models.Common
{
    public class SearchArgsModel<T> : SearchArgsModel
    {
        public T Filter { get; set; }
    }

    public class SearchArgsModel : IArgumentModel
    {
        public string FullSearch { get; set; }
        public string SortField { get; set; }
        public string SortOrder { get; set; }
        public int? Limit { get; set; }
        public int? Offset { get; set; }
    }
}
