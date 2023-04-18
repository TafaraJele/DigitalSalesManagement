using DigitalSalesManagement.Abstractions.Entities;
using DigitalSalesManagement.Abstractions.Models;
using Veneka.Platform.Common;

namespace DigitalSalesManagement.Core.Aggregates
{
    public class AgentCommissionAggregate : BaseAggregate<AgentCommissionEntity>
    {
        ValidationResult validationResult = new ValidationResult();
        public AgentCommissionAggregate(AgentCommissionEntity entity) : base(entity)
        {

        }


        public ValidationResult Save(AgentCommission commission)
        {
            if (ValidateAgentCommission(commission).IsValid)
            {
                SetDetails(commission);
            }
            return validationResult;
        }

        public void DeleteRecord()
        {
            Entity.Delete();
        }

        private void SetDetails(AgentCommission commission)
        {
            
            Entity.AgentCommissionPlanId = commission.AgentCommissionPlanId;
            Entity.AgentId = commission.AgentId;
        }

        private ValidationResult ValidateAgentCommission(AgentCommission commission)
        {
            return validationResult;
        }

    }
}
