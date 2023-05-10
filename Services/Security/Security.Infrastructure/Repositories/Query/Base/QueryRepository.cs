using Microsoft.Extensions.Configuration;
using Security.Domain.Repositories.Query.Base;
using Security.Infrastructure.Data;

namespace Security.Infrastructure.Repository.Query.Base
{
    /// <summary>
    /// QueryRepository base class
    /// </summary>
    public class QueryRepository<T> : DbConnector,  IQueryRepository<T> where T : class
    {
        /// <summary>
        /// Security context instance
        /// </summary>
        protected readonly SecurityContext _context;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration">Configuration</param>
        /// <param name="context">Security context instance</param>
        public QueryRepository(IConfiguration configuration, SecurityContext context)
            : base(configuration)
        {
            _context = context;
        }
    }
}