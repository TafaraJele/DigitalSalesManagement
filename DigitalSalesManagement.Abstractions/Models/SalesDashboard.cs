using System.Runtime.Serialization;
using Veneka.Platform.Common;

namespace DigitalSalesManagement.Abstractions.Models
{
    [DataContract]
    public class SalesDashboard : BaseQueryModel
    {
        [DataMember]
        public decimal TotalSales { get; set; }

        [DataMember]
        public int SalesCount { get; set; }

        [DataMember]
        public decimal TotalUnpaidCommissions { get; set; }

        [DataMember]
        public int UnpaidCommissionsCount { get; set; }

        [DataMember]
        public decimal TotalPaidCommissions { get; set; }

        [DataMember]
        public int PaidCommissionsCount { get; set; }

        [DataMember]
        public decimal TotalEarnedCommissions { get; set; }

        [DataMember]
        public int EarnedCommissionsCount { get; set; }
    }
}
