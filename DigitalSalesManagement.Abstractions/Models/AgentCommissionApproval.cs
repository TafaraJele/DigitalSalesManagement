using System;
using System.Runtime.Serialization;
using Veneka.Platform.Common;

namespace DigitalSalesManagement.Abstractions.Models

{
    [DataContract]
    public class AgentCommissionApproval : BaseQueryModel
    {
        [DataMember]
        public Guid AgentId { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string StatusReason { get; set; }

        [DataMember]
        public string Month { get; set; }

        [DataMember]
        public string Year { get; set; }
        [DataMember]
        public double TotalAmount { get; set; }



    }
}
