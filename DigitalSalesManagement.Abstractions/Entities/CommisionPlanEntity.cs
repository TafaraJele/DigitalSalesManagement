namespace DigitalSalesManagement.Abstractions.Entities
{
    public class CommisionPlanEntity : BaseEntity
    {
        public string CommissionValueType { get; set; }
       
        public string Name { get; set; }
       
        public string Description { get; set; }

        
        public decimal FixedValue { get; set; }
        
        public decimal PercentageValue { get; set; }
       
        public int ProductId { get; set; }

    }
}
