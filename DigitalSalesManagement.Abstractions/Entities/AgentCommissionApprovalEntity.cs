using System;

namespace DigitalSalesManagement.Abstractions.Entities
{
    public class AgentCommissionApprovalEntity : BaseEntity
    {

        public string Year { get; set; }
        public Guid AgentId { get; set; }
        
        public string Status { get; set; }
        public double TotalAmount { get; set; }
        public string StatusReason { get; set; }
        public string Month { get; set; }
    }
}
