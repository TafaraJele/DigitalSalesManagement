using System.Runtime.Serialization;

namespace DigitalSalesManagement.Abstractions.Models
{
    [DataContract]
    public class Product
    {
        [DataMember]
        public string  Product_id { get; set; }
        [DataMember]
        public string Product_code { get; set; }
        [DataMember]
        public string Product_name { get; set; }
    }
}
