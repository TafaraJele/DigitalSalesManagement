using DigitalSalesManagement.Abstractions.Entities;
using System;

namespace DigitalSalesManagement.Core.Services
{
    public static class EntityFactory
    {
        
        public static AgentCalculatedCommissionEntity CreateAgentCalculatedCommission()
        {
            return new AgentCalculatedCommissionEntity
            {
                Id = Guid.NewGuid()
            };
        }

        public static AgentCommissionApprovalEntity CreateAgentCommissionApproval()
        {
            return new AgentCommissionApprovalEntity
            {
                Id = Guid.NewGuid()
            };
        }

        public static AgentCommissionEntity CreateAgentCommission()
        {
            return new AgentCommissionEntity
            {
                Id = Guid.NewGuid()
            };
        }

        
        public static CommisionPlanEntity CreateCommisionPlan()
        {
            return new CommisionPlanEntity
            {
                Id = Guid.NewGuid()
            };
        }
        public static AgentEntity CreateAgent()
        {
            return new AgentEntity
            {
                Id = Guid.NewGuid()
            };
        }
    }
}
