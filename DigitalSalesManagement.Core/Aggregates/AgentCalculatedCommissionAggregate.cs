using DigitalSalesManagement.Abstractions.Entities;
using DigitalSalesManagement.Abstractions.Models;
using Veneka.Platform.Common;

namespace DigitalSalesManagement.Core.Aggregates
{
    public class AgentCalculatedCommissionAggregate : BaseAggregate<AgentCalculatedCommissionEntity>
    {
        ValidationResult validationResult = new ValidationResult();
        public AgentCalculatedCommissionAggregate(AgentCalculatedCommissionEntity entity) : base(entity)
        {

        }


        public ValidationResult Save(AgentCalculatedCommission commission)
        {
            if (ValidateCalculatedCommission(commission).IsValid)
            {
                SetDetails(commission);
            }
            return validationResult;
        }

        public void DeleteRecord()
        {
            Entity.Delete();
        }

        private void SetDetails(AgentCalculatedCommission commission)
        {
            Entity.Amount = commission.Amount;
            Entity.Fee = commission.Fee;
            Entity.FixedValue = commission.FixedValue;
            Entity.Name = commission.Name;
            Entity.PercentageValue = commission.PercentageValue;
            Entity.ProductId = commission.ProductId;
            Entity.CommissionPlanId = commission.CommissionPlanId;
            Entity.AgentId = commission.AgentId;
            Entity.IsPaid = commission.IsPaid;
            Entity.Date = commission.Date;
            Entity.CardReferenceNumber = commission.CardReferenceNumber;
            Entity.HasError = commission.HasError;
            Entity.ErrorReason = commission.ErrorReason;
        }

        private ValidationResult ValidateCalculatedCommission(AgentCalculatedCommission commission)
        {
            return validationResult;
        }
    }
}
