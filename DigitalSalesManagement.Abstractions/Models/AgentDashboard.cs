using System.Runtime.Serialization;

namespace DigitalSalesManagement.Abstractions.Models
{
    [DataContract]
    public class AgentDashboard : Agent
    {
        public AgentDashboard(Agent agent)
        {
            this.AccountNumber = agent.AccountNumber;
            this.AgentCode = agent.AgentCode;
            this.Name = agent.Name;
            this.RegisteredDate = agent.RegisteredDate;
            this.Surname = agent.Surname;
            this.Id = agent.Id;
        }

        [DataMember]
        public decimal TotalSales { get; set; }
        [DataMember]
        public decimal TotalCommissionsEarned { get; set; }
        [DataMember]
        public decimal TotalCommissionsPaid { get; set; }
        [DataMember]
        public decimal TotalCommissionUnpaid { get; set; }
    }
}
