using System;
using System.Runtime.Serialization;
using Veneka.Platform.Common;

namespace DigitalSalesManagement.Abstractions.Models
{
    [DataContract]
   public class AgentSale : BaseQueryModel
    {
        [DataMember]
        public decimal Fee { get; set; }

        [DataMember]
        public int? ProductId { get; set; }

        [DataMember]
        public Guid AgentId { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public string CardReferenceNumber { get; set; }
    }
}
