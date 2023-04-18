using System;
using System.Runtime.Serialization;
using Veneka.Platform.Common;

namespace DigitalSalesManagement.Abstractions.Models
{
    [DataContract]
   public class AgentCalculatedCommission : BaseQueryModel
    {
        [DataMember]
        public decimal Amount { get; set; }
        [DataMember]
        public decimal Fee { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public decimal FixedValue { get; set; }

        [DataMember]
        public decimal PercentageValue { get; set; }

        [DataMember]
        public int? ProductId { get; set; }

        [DataMember]
        public Guid CommissionPlanId { get; set; }

        [DataMember]
        public Guid AgentId { get; set; }

        [DataMember]
        public bool IsPaid { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public string CardReferenceNumber { get; set; }

        [DataMember]
        public bool HasError { get; set; }

        [DataMember]
        public string ErrorReason { get; set; }

    }
}
