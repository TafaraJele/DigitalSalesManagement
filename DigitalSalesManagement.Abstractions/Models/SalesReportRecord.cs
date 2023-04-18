using System;
using System.Runtime.Serialization;
using Veneka.Platform.Common;

namespace DigitalSalesManagement.Abstractions.Models
{
    [DataContract]
    public class SalesReportRecord :  BaseQueryModel
    {

        [DataMember]
        public decimal? Amount { get; set; }
        [DataMember]
        public decimal Fee { get; set; }
       
        [DataMember]
        public decimal? FixedValue { get; set; }

        [DataMember]
        public decimal? PercentageValue { get; set; }

        [DataMember]
        public int? ProductId { get; set; }

        [DataMember]
        public string CommissionPlan { get; set; }

        [DataMember]
        public string  AgentName { get; set; }

        [DataMember]
        public bool? IsPaid { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public string CardReferenceNumber { get; set; }

    }
}
