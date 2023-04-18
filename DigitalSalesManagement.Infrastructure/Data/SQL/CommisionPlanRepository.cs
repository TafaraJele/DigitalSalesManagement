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
    public class CommisionPlanRepository : ICommisionPlanRepository
    {
        private SQLDBContext _context;
        public CommisionPlanRepository(SQLDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CommisionPlanEntity>> FindAggregatesAsync(List<SearchParameter> searchParameters)
        {
            var result = (await _context.CommissionPlans.ToListAsync()).Where(s => !s.IsDeleted).Select(s => s);
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
                    case SearchOptions.PRODUCTID:
                        {
                            result = result.Where(s => s.ProductId == int.Parse(parameter.Value)).Select(s => s);
                        }
                    break;

                }
            }
            return result;

        }

        public async Task<CommisionPlanEntity> GetCommisionPlan(Guid id)
        {
            var result = await _context.CommissionPlans.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
            return result;
        }

        public async Task<CommisionPlanEntity> LoadAggregateAsync(Guid id)
        {
            var result = await _context.CommissionPlans.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);

            return result ?? EntityFactory.CreateCommisionPlan();
        }

        public async Task<Guid> SaveAggregateAsync(CommisionPlanEntity aggregate)
        {
            var result = await _context.CommissionPlans.FindAsync(aggregate.Id);

            if (result == null)
            {
                await _context.CommissionPlans.AddAsync(aggregate);
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
