using System.Runtime.Serialization;

namespace DigitalSalesManagement.Abstractions.Models
{
    [DataContract]
   public class AgentCommissionFee
   {
        [DataMember]
        public decimal Fee { get; set; }

        [DataMember]
        public string AgentCode { get; set; }
        
        [DataMember]
        public int ProductId { get; set; }

        [DataMember]
        public string CardRequestRef { get; set; }
    }
}
