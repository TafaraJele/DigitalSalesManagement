using DigitalSalesManagement.Abstractions.Entities;
using System;
using System.Runtime.Serialization;
using Veneka.Platform.Common;

namespace DigitalSalesManagement.Abstractions.Models
{
    [DataContract]
    public class CommissionReportRecord : BaseQueryModel
    {

        public CommissionReportRecord(AgentCalculatedCommissionEntity commission)
        {
            this.Amount = commission.Amount;
            this.CardReferenceNumber = commission.CardReferenceNumber;
            this.Name = commission.Name;
            this.Id = commission.Id;
            this.FixedValue = commission.FixedValue;
            this.PercentageValue = commission.PercentageValue;
            this.ProductId = commission.ProductId;
            this.IsPaid = commission.IsPaid;
            this.Date = commission.Date;
            this.CardReferenceNumber = commission.CardReferenceNumber;
            this.HasError = commission.HasError;
            this.ErrorReason = commission.ErrorReason;
            this.Fee = commission.Fee;
            this.AgentId = commission.AgentId;
        }

        [DataMember]
        public decimal Fee { get; set; }

        [DataMember]
        public decimal Amount { get; set; }
       
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public decimal FixedValue { get; set; }

        [DataMember]
        public decimal PercentageValue { get; set; }

        [DataMember]
        public int? ProductId { get; set; }

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
        
        [DataMember]
        public string AgentName { get; set; }

        [DataMember]
        public Guid AgentId { get; set; }
    }
}
