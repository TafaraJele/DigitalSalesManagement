using DigitalSalesManagement.Abstractions.Entities;
using DigitalSalesManagement.Abstractions.Models;
using Veneka.Platform.Common;

namespace DigitalSalesManagement.Core.Aggregates
{
    public class AgentAggregate : BaseAggregate<AgentEntity>
    {
        ValidationResult validationResult = new ValidationResult();

        public AgentAggregate(AgentEntity entity) : base(entity)
        {

        }


        public ValidationResult Save(Agent agent)
        {
            if (ValidateAgent(agent).IsValid)
            {
                SetDetails(agent);
            }
            return validationResult;
        }


        public void DeleteRecord()
        {
            Entity.Delete();
        }

        private void SetDetails(Agent agent)
        {
            Entity.Name = agent.Name;
            Entity.Surname = agent.Surname;
            Entity.AgentCode = agent.AgentCode;
            Entity.RegisteredDate = agent.RegisteredDate;
            Entity.AccountNumber = agent.AccountNumber;
        }

        private ValidationResult ValidateAgent(Agent agent)
        {
            return validationResult;
        }

    }
}
