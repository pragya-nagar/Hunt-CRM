using Synergy.ServiceBus.Abstracts;
using System;

namespace Synergy.Domain.Models.Common
{
    public abstract class FastCommand<TK, T> : ICommand
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime CreatedOn { get; } = DateTime.UtcNow;
        public Guid CorrelationId { get; set; } = Guid.NewGuid();

        public TK EntityId { get; }
        public T Entity { get; }

        protected FastCommand(TK entityId, T entity)
        {
            this.EntityId = entityId;
            this.Entity = entity;
        }
    }
}
