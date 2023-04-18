using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DigitalSalesManagement.Abstractions.Models
{

    [DataContract]
    public class CommissionPlanResponse
    {
        public CommissionPlanResponse()
        {
            this.CommissionPlans = new List<CommisionPlan>();
        }
        [DataMember]
        public int ProductId { get; set; }

        [DataMember]
        public List<CommisionPlan> CommissionPlans { get; set; }

    }
}
