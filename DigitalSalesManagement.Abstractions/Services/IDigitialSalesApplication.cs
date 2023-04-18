using DigitalSalesManagement.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Veneka.Platform.Common;

namespace DigitalSalesManagement.Abstractions.Services
{
    public interface IDigitialSalesApplication
    {

        #region Agents
        Task<CommandResult<Agent>> CreateAgent(AgentWrite agent);
        Task<CommandResult> UpdateAgent(AgentWrite agent);
        Task<CommandResult> DeleteAgent(Guid agentId);
        Task<Agent> GetAgentDetails(Guid agentId);
        Task<IEnumerable<Agent>> GetAgents();
        Task<Agent> GetAgentByCode(string code);
        Task<CommandResult> CalculateAgentCommission(AgentCommissionFee commissionFee);
        Task<IEnumerable<AgentSale>> GetAgentSales(Guid agentId, DateTime? startdate, DateTime? endDate);
        Task<IEnumerable<AgentDashboard>> GetAgentDashboards( DateTime? startdate, DateTime? endDate);

        Task<IEnumerable<AgentEarnedCommission>> GetTotalAgentEarnedCommissions(DateTime? startdate, DateTime? endDate); 
        #endregion

        Task<SalesDashboard> GetSalesDashboard(DateTime? startdate, DateTime? endDate);

        #region AgentCommissions
        Task<CommandResult<AgentCommission>> CreateAgentCommission(AgentCommission commission);
        Task<CommandResult> DeleteAgentCommision(Guid agentCommissionId);
        Task<IEnumerable<AgentCommissionRead>> GetAgentCommissions(Guid agentId);

        

        #endregion


        #region CalculatedCommissions
        Task<CommandResult<AgentCalculatedCommission>> AddCalculatedCommission(AgentCalculatedCommission commission);
        Task<CommandResult> UpdateCalculatedCommission(AgentCalculatedCommission commission);

        Task<CommandResult> PayAgent(Agent agent, string year, string month);

        Task<CommandResult> RecalculateAgentCommissions(Guid agentId, DateTime startDate, DateTime endDate);
        Task<CommandResult> RescheduleAgentCommissions(List<Guid> agents,  DateTime startDate, DateTime endDate);
        Task<IEnumerable<AgentCalculatedCommission>> GetAgentPaidCommissions(Guid agentId, DateTime? startDate, DateTime? endDate);
        Task<IEnumerable<AgentCalculatedCommission>> GetAgentUnpaidCommissions(Guid agentId, DateTime? startDate, DateTime? endDate);
        Task<IEnumerable<AgentCalculatedCommission>> GetAgentEarnedCommissions(Guid agentId, DateTime? startDate, DateTime? endDate);
        Task<IEnumerable<AgentCalculatedCommission>> GetAgentUnProcessedCommissions(Guid agentId, DateTime? startDate, DateTime? endDate);



        #endregion

        #region CommissionPlans
        Task<CommandResult<CommisionPlan>> CreateCommissionPlan(CommisionPlan plan);
        Task<CommandResult> UpdateCommissionPlan(CommisionPlan plan);
        Task<CommandResult> DeleteCommissionPlan(Guid commissionPlanId);
        Task<IEnumerable<CommisionPlan>> GetProductCommissionPlans(int productId);
        Task<IEnumerable<CommissionPlanResponse>> GetCommisionPlans();
        #endregion


        #region Reports
        Task<IEnumerable<SalesReportRecord>> GetSalesReport(DateTime? startDate, DateTime? endDate, int? productId, Guid? agentId);
        Task<IEnumerable<CommissionReportRecord>> GetEarnedCommissions(DateTime? startDate, DateTime? endDate);
        Task<IEnumerable<CommissionReportRecord>> GetPaidCommissions(DateTime? startDate, DateTime? endDate);
        Task<IEnumerable<CommissionReportRecord>> GetUnpaidCommissions(DateTime? startDate, DateTime? endDate);
        Task<IEnumerable<CommissionReportRecord>> GetUnprocessedCommissions(DateTime? startDate, DateTime? endDate);

        Task<IEnumerable<AgentCommissionApproval>> GetAgentCommissionApprovals(string month, string year);
        #endregion


        #region Commission Approvals

        Task<CommandResult> CreateApproval(AgentCommissionApproval approval);
        Task<CommandResult> ApproveAllCommissions(List<AgentCommissionApproval> approvals);
        Task<CommandResult> UpdateApproval(AgentCommissionApproval approval);
        Task<CommandResult> RemoveApproval(Guid id);
         Task<AgentCommissionApproval> GetAgentCommissionApprovals(Guid agentId, string month, string year);



        #endregion

    }

}