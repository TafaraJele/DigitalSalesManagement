using System.Runtime.Serialization;
using Veneka.Platform.Common;

namespace DigitalSalesManagement.Abstractions.Models
{
    [DataContract]
   public class CommisionPlan : BaseQueryModel
    {
     
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public decimal FixedValue { get; set; }
        [DataMember]
        public decimal PercentageValue { get; set; }
        [DataMember]
        public int?  ProductId { get; set; }
       
    }
}
