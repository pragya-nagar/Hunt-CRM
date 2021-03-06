using System;
using Synergy.Common.DAL.Abstract;

namespace Synergy.CRM.DAL.Queries.Entities
{
    public class LeadComment : IAuditEntity<Guid>
    {
        public Guid Id { get; set; }

        public Guid LeadId { get; set; }

        public Guid AuthorId { get; set; }

        public string Comment { get; set; }

        public DateTime CommentDate { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid CreatedById { get; set; }

        public DateTime ModifiedOn { get; set; }

        public Guid ModifiedById { get; set; }

        public DateTime? DeletedOn { get; set; }

        public Lead Lead { get; set; }

        public User Author { get; set; }

        public User CreatedBy { get; set; }

        public User ModifiedBy { get; set; }
    }
}