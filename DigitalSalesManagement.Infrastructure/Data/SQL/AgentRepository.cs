
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
    public class AgentRepository : IAgentRepository
    {
        private SQLDBContext _context;

        public AgentRepository(SQLDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AgentEntity>> FindAggregatesAsync(List<SearchParameter> searchParameters)
        {
            var result = await (_context.Agents.Where(s => !s.IsDeleted).Select(s => s)).ToListAsync();
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
                    case SearchOptions.AGENTCODE:
                        {
                            result = result.Where(s => s.AgentCode.Trim() == parameter.Value.Trim()).Select(s => s).ToList();
                        }
                   break;
                }
            }
            return result;
        }

        public async Task<AgentEntity> LoadAggregateAsync(Guid id)
        {
            var result = await _context.Agents.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);

            return result ?? EntityFactory.CreateAgent();

        }

        public async Task<Guid> SaveAggregateAsync(AgentEntity aggregate)
        {
            var result = await _context.Agents.FindAsync(aggregate.Id);

            if (result == null)
            {
                await _context.Agents.AddAsync(aggregate);
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
