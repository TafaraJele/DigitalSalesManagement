using DigitalSalesManagement.Abstractions.Entities;
using DigitalSalesManagement.Abstractions.Enums;
using DigitalSalesManagement.Abstractions.Repositories;
using DigitalSalesManagement.Core.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Veneka.Platform.Common;

namespace DigitalSalesManagement.Infrastructure.Data.SQL
{
    public class AgentCommissionApprovalRepository : IAgentCommissionApprovalRepository
    {

        private SQLDBContext _context;
        public AgentCommissionApprovalRepository(SQLDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AgentCommissionApprovalEntity>> FindAggregatesAsync(List<SearchParameter> searchParameters)
        {
            var result = (await _context.AgentCommissionApprovals.ToListAsync()).Where(s => !s.IsDeleted).Select(s => s);
            foreach (var parameter in searchParameters.Where(
                   parameter => !string.IsNullOrEmpty(parameter.Name) && !string.IsNullOrEmpty(parameter.Value)))
            {
                var validParameter = Enum.TryParse(parameter.Name.ToUpper(), out SearchOptions option);
                if (!validParameter)
                {
                    continue;
                }
                switch (option)
                {
                    case SearchOptions.AGENTID:
                        {
                            result = result.Where(s => s.AgentId == Guid.Parse(parameter.Value)).Select(s => s);
                        }
                        break;

                    case SearchOptions.CALENDARMONTH:
                        {
                           result = result.Where(s => s.Month == parameter.Value).Select(s => s);
                        }
                        break;
                    case SearchOptions.CALENDARYEAR:
                        {
                            result = result.Where(s => s.Year == parameter.Value).Select(s => s);
                        }
                        break;


                }
            }
            return result;
        }

        public async Task<AgentCommissionApprovalEntity> GetAgentApproval(Guid agentId)
        {
            var result = await _context.AgentCommissionApprovals.FirstOrDefaultAsync(a => a.AgentId == agentId && !a.IsDeleted);
            return result;
        }

        public async Task<IEnumerable<AgentCommissionApprovalEntity>> GetMonthlyApprovals(string year, string month, Guid? agentId )
        {
            var result = await _context.AgentCommissionApprovals.Where(a => a.Year == year && a.Month == month && !a.IsDeleted).Select(a=>a).ToListAsync();

            if(agentId.HasValue)
            {
                result = await _context.AgentCommissionApprovals.Where(a => a.Year == year && a.Month == month && a.AgentId == agentId.Value && !a.IsDeleted).Select(a => a).ToListAsync();
            }

            return result;
        }

        public  async Task<AgentCommissionApprovalEntity> LoadAggregateAsync(Guid id)
        {
            var result = await _context.AgentCommissionApprovals.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);

            return result ?? EntityFactory.CreateAgentCommissionApproval();
        }

        public async Task<Guid> SaveAggregateAsync(AgentCommissionApprovalEntity aggregate)
        {
            var result = await _context.AgentCommissionApprovals.FindAsync(aggregate.Id);

            if (result == null)
            {
                await _context.AgentCommissionApprovals.AddAsync(aggregate);
                await _context.SaveChangesAsync();

            }
            else
            {
                _context.Entry(aggregate).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            return aggregate.Id;

        }
    }
}
