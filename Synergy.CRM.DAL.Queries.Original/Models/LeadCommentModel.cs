using System;
using Synergy.DataAccess.Abstractions.Interfaces;
using Synergy.DataAccess.Abstractions.Models;

namespace Synergy.CRM.DAL.Queries.Original.Models
{
    public class LeadCommentModel : IModel
    {
        public Guid Id { get; set; }

        public Guid LeadId { get; set; }

        public FastEntityModel<Guid> Author { get; set; }

        public string Comment { get; set; }

        public DateTime CommentDate { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}
