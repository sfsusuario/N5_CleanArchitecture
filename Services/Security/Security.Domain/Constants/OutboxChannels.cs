namespace Security.Domain.Constants
{
    /// <summary>
    /// Valid values for <see cref="Entities.OutboxMessage.Channel"/>.
    /// </summary>
    public static class OutboxChannels
    {
        public static readonly string Kafka = "Kafka";

        public static readonly string Elasticsearch = "Elasticsearch";
    }
}
