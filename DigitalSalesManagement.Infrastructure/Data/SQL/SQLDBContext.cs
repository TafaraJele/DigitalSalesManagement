using DigitalSalesManagement.Abstractions.Entities;
using Microsoft.EntityFrameworkCore;

namespace DigitalSalesManagement.Infrastructure.Data.SQL
{
    public class SQLDBContext : DbContext
    {
        public DbSet<AgentEntity> Agents { get; set; }
        public DbSet<AgentCommissionEntity> AgentCommissions { get; set; }
        public DbSet<CommisionPlanEntity> CommissionPlans { get; set; }
        public DbSet<AgentCalculatedCommissionEntity> AgentCalculatedCommissions { get; set; }
        public DbSet<AgentCommissionApprovalEntity> AgentCommissionApprovals { get; set; }



        public SQLDBContext(DbContextOptions<SQLDBContext> options) : base(options)
        {

        }
    }
}
