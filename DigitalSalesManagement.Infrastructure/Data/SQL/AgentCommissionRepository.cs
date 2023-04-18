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
    public class AgentCommissionRepository : IAgentCommissionRepository
    {
        private SQLDBContext _context;
        public AgentCommissionRepository(SQLDBContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<AgentCommissionEntity>> FindAggregatesAsync(List<SearchParameter> searchParameters)
        {
            var result = await (_context.AgentCommissions.Where(s => !s.IsDeleted).Select(s => s)).ToListAsync();
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
                            result = result.Where(s => s.AgentId == Guid.Parse(parameter.Value)).Select(s => s).ToList();
                        }
                        break;
                    case SearchOptions.COMMISSIONPLANID:
                        {
                            result = result.Where(s => s.AgentCommissionPlanId == Guid.Parse(parameter.Value)).Select(s => s).ToList();
                        }
                        break;


                }
            }
            return result;
        }

        public async Task<AgentCommissionEntity> LoadAggregateAsync(Guid id)
        {
            var result = await _context.AgentCommissions.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);

            return result ?? EntityFactory.CreateAgentCommission();
        }

        public async Task<Guid> SaveAggregateAsync(AgentCommissionEntity aggregate)
        {
            var result = await _context.AgentCommissions.FindAsync(aggregate.Id);

            if (result == null)
            {
                await _context.AgentCommissions.AddAsync(aggregate);
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
