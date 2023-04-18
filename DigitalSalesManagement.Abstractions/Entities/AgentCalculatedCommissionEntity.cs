using System;

namespace DigitalSalesManagement.Abstractions.Entities
{
    public class AgentCalculatedCommissionEntity : BaseEntity
    {
       
        public decimal Amount { get; set; }
       
        public decimal Fee { get; set; }
     
        public string Name { get; set; }
        
        public decimal FixedValue { get; set; }
       
        public decimal PercentageValue { get; set; }
        
        public int? ProductId { get; set; }

        public Guid CommissionPlanId { get; set; }

        public Guid AgentId { get; set; }

        public bool IsPaid { get; set; }
        public DateTime Date { get; set; }
        public string CardReferenceNumber { get; set; }
        public bool HasError { get; set; }
        public string ErrorReason { get; set; }

    }
}
