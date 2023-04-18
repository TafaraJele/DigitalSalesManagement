using DigitalSalesManagement.Abstractions.Entities;
using System;
using System.Threading.Tasks;

namespace DigitalSalesManagement.Abstractions.Repositories
{
    public interface ICommisionPlanRepository : IRepository<CommisionPlanEntity, Guid>
    {
        Task<CommisionPlanEntity> GetCommisionPlan(Guid id);
    }
}
