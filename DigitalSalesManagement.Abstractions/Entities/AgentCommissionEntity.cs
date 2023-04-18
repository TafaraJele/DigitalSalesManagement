using System;

namespace DigitalSalesManagement.Abstractions.Entities
{
    public class AgentCommissionEntity : BaseEntity
    {
        public Guid AgentId { get; set; }
        public Guid AgentCommissionPlanId { get; set; }

    }
}
