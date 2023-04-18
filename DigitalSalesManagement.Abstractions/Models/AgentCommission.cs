using System;
using System.Runtime.Serialization;
using Veneka.Platform.Common;

namespace DigitalSalesManagement.Abstractions.Models
{
    [DataContract]
   public class AgentCommission : BaseQueryModel
    {
        [DataMember]
        public Guid AgentId { get; set; }
        [DataMember]
        public Guid AgentCommissionPlanId { get; set; }
    }
}
