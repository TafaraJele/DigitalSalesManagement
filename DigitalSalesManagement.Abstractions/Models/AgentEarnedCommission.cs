using System;
using System.Runtime.Serialization;

namespace DigitalSalesManagement.Abstractions.Models
{
    [DataContract]
    public class AgentEarnedCommission
    {
        [DataMember]
        public Guid AgentId { get; set; }

        [DataMember]
        public double TotalAmount { get; set; }
    }
}
