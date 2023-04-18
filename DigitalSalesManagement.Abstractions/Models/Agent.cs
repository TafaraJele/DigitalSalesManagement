using System;
using System.Runtime.Serialization;
using Veneka.Platform.Common;

namespace DigitalSalesManagement.Abstractions.Models
{
    [DataContract]
    public class Agent : BaseQueryModel
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Surname { get; set; }
        [DataMember]
        public string AgentCode { get; set; }
        [DataMember]
        public DateTime? RegisteredDate { get; set; }
        [DataMember]
        public string AccountNumber { get; set; }
    }
}
