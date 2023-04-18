using System;
using System.Runtime.Serialization;

namespace DigitalSalesManagement.Abstractions.Models
{
    [DataContract]
   public class AgentCommissionRead : CommisionPlan
    {
        [DataMember]
        public Guid AgentId { get; set; }
      
    }
}
