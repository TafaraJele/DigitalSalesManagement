using System;

namespace DigitalSalesManagement.Abstractions.Entities
{

    public class AgentEntity : BaseEntity
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string AgentCode { get; set; }
        public DateTime? RegisteredDate { get; set; }
        public string AccountNumber { get; set; }

    }
}
