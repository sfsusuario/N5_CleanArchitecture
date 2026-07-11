using System;
using System.ComponentModel.DataAnnotations.Schema;
using Security.Domain.Entities.Base;

namespace Security.Domain.Entities
{
    /// <summary>
    /// Outbox message entity. Written in the same transaction as the business change it
    /// describes, so persisting it is atomic with that change; a background dispatcher
    /// later delivers it to the external channel (Kafka/Elasticsearch) and marks it processed.
    /// </summary>
    [Table("OutboxMessages")]
    public class OutboxMessage : BaseEntity
    {
        /// <summary>
        /// Destination channel. See <see cref="Constants.OutboxChannels"/>.
        /// </summary>
        public string Channel { get; set; } = string.Empty;

        /// <summary>
        /// JSON-serialized payload for the destination channel's command type.
        /// </summary>
        public string Payload { get; set; } = string.Empty;

        /// <summary>
        /// When the message was written (same transaction as the business change).
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Set once the dispatcher successfully delivers the message. Null means pending.
        /// </summary>
        public DateTime? ProcessedAt { get; set; }

        /// <summary>
        /// Number of failed delivery attempts so far.
        /// </summary>
        public int RetryCount { get; set; }

        /// <summary>
        /// Error message from the most recent failed delivery attempt, if any.
        /// </summary>
        public string? LastError { get; set; }
    }
}
