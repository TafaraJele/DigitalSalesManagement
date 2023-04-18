using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DigitalSalesManagement.Abstractions.Models
{
    [DataContract]
   public class AgentWrite : Agent
    {
        [DataMember]
        public List<Guid> CommissionPlanIds { get; set; }
    }
}
