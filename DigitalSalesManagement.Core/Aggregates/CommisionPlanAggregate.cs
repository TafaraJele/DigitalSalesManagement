using DigitalSalesManagement.Abstractions.Entities;
using DigitalSalesManagement.Abstractions.Models;
using Veneka.Platform.Common;
using Veneka.Platform.Common.Enums;

namespace DigitalSalesManagement.Core.Aggregates
{
    public class CommisionPlanAggregate : BaseAggregate<CommisionPlanEntity>
    {
        ValidationResult validationResult = new ValidationResult();
        public CommisionPlanAggregate(CommisionPlanEntity entity) : base(entity)
        {

        }

        public void DeleteRecord()
        {
            Entity.Delete();
        }


        public ValidationResult Save(CommisionPlan plan)
        {
            if (ValidateCommisionPlan(plan).IsValid)
            {
                SetDetails(plan);
            }
            return validationResult;
        }


        private void SetDetails(CommisionPlan plan)
        {
            Entity.Description = plan.Description;
            Entity.FixedValue = plan.FixedValue;
            Entity.Name = plan.Name;
            Entity.PercentageValue = plan.PercentageValue;
            Entity.ProductId = plan.ProductId.Value;
        }


        private ValidationResult ValidateCommisionPlan(CommisionPlan plan)
        {
           

            if(string.IsNullOrEmpty(plan.Name))
            {
                validationResult.AddValidationMessage(ResultMessageType.Error, "plan name", "Plan name required");
            }

            if (!plan.ProductId.HasValue)
            {
                validationResult.AddValidationMessage(ResultMessageType.Error, "product", "ProductId is required");
            }

            return validationResult;
        } 



    }
}
