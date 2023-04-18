using AutoMapper;
using DigitalSalesManagement.Abstractions.Entities;

namespace DigitalSalesManagement.Abstractions.Models
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AgentEntity, Agent>();
            CreateMap<AgentCommissionEntity, AgentCommission>();
            CreateMap<CommisionPlanEntity, CommisionPlan>();
            CreateMap<AgentCalculatedCommissionEntity, AgentCalculatedCommission>();
            CreateMap<AgentCommissionApprovalEntity, AgentCommissionApproval>();
        }
    }
}
