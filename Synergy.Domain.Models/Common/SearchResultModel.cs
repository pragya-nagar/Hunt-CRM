using Synergy.Domain.Models.Abstracts;
using System.Collections.Generic;
namespace Synergy.Domain.Models.Common
{
    public class SearchResultModel<T> where T : IResultModel
    {
        public int TotalCount { get; set; }
        public IEnumerable<T> List { get; set; }
    }
}
