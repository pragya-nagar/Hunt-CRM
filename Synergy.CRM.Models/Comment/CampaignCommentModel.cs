using System;
using Synergy.Common.Domain.Models.Abstracts;
using Synergy.Common.Domain.Models.Common;

namespace Synergy.CRM.Models
{
    public class CampaignCommentModel : IResultModel
    {
        public Guid Id { get; set; }

        public Guid CampaignId { get; set; }

        public FastEntityModel<Guid> Author { get; set; }

        public string Comment { get; set; }

        public DateTime CommentDate { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}
