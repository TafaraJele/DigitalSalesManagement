using DigitalSalesManagement.Abstractions.Entities;
using DigitalSalesManagement.Abstractions.Models;
using Veneka.Platform.Common;

namespace DigitalSalesManagement.Core.Aggregates
{
    public class AgentCommissionApprovalAggregate : BaseAggregate<AgentCommissionApprovalEntity>
    {
        ValidationResult validationResult;
        public AgentCommissionApprovalAggregate(AgentCommissionApprovalEntity entity): base(entity) 
        {
            validationResult = new ValidationResult();
        }


        private ValidationResult ValidateApproval(AgentCommissionApproval approval)
        {

            return validationResult;
        }
        

        public ValidationResult Save(AgentCommissionApproval approval)
        {
            if (ValidateApproval(approval).IsValid)
            {
               SaveDetails(approval);
            }
            return validationResult;
        }

        public void Delete()
        {
            Entity.Delete();
        }

        private void SaveDetails(AgentCommissionApproval aprroval)
        {
            Entity.AgentId = aprroval.AgentId;
            Entity.Status = aprroval.Status;
            Entity.StatusReason = aprroval.StatusReason;
            Entity.Month = aprroval.Month;
            Entity.Year = aprroval.Year;
            Entity.TotalAmount = aprroval.TotalAmount;
            
            
        }
    }
}
