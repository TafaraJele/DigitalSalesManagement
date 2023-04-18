using System.Collections.Generic;
using System.Threading.Tasks;
using Veneka.Platform.Common;

namespace DigitalSalesManagement.Abstractions.Repositories
{
    public interface IRepository<T, TId> where T : IAggregateRoot
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchParameters"></param>
        /// <param name="filterType"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> FindAggregatesAsync(List<SearchParameter> searchParameters);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> LoadAggregateAsync(TId id);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aggregate"></param>
        /// <returns></returns>
        Task<TId> SaveAggregateAsync(T aggregate);
    }
}
