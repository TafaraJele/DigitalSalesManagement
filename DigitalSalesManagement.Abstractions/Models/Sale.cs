using System;
using System.Runtime.Serialization;

namespace DigitalSalesManagement.Abstractions.Models
{
    [DataContract]
    public class Sale 
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public DateTime Date { get; set; }
        [DataMember]
        public string Product { get; set; }
        [DataMember]
        public string CardReferenceNumber { get; set; }
        [DataMember]
        public string AgentName { get; set; }
        [DataMember]
        public decimal CardFee { get; set; }


    }
}
