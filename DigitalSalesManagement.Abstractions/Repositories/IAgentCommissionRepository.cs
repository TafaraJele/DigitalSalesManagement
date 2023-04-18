using DigitalSalesManagement.Abstractions.Entities;
using System;

namespace DigitalSalesManagement.Abstractions.Repositories
{
    public  interface IAgentCommissionRepository : IRepository<AgentCommissionEntity, Guid>
    {
    }
}
