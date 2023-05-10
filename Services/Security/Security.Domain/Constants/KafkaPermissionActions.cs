using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.Domain.Constants
{
    /// <summary>
    /// Kafka permissions indentifiers
    /// </summary>
    public static class KafkaPermissionActions
    {
        /// <summary>
        /// Kafka modify event
        /// </summary>
        public static readonly string MODIFY = "MODIFY";

        /// <summary>
        /// Kafka request event
        /// </summary>
        public static readonly string REQUEST = "REQUEST";

        /// <summary>
        /// Kafka get event
        /// </summary>
        public static readonly string GET = "GET";
    }
}
