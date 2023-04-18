using DigitalSalesManagement.Abstractions.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalSalesManagement.Abstractions.Repositories
{
    public interface IAgentCommissionApprovalRepository : IRepository<AgentCommissionApprovalEntity, Guid>
    {
      Task<IEnumerable<AgentCommissionApprovalEntity>> GetMonthlyApprovals(string year, string month, Guid? agentId);
        Task<AgentCommissionApprovalEntity> GetAgentApproval(Guid agentId);
    }
}
