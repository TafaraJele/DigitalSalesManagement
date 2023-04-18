using DigitalSalesManagement.Abstractions.Models;
using DigitalSalesManagement.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalSalesManagement.API.Controllers
{/// <summary>
/// 
/// </summary>
    [Route("api/reports")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private ILogger<ReportsController> _logger;
        private IDigitialSalesApplication _application;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="application"></param>
        public ReportsController(ILogger<ReportsController> logger, IDigitialSalesApplication application)
        {
            _logger = logger;
            _application = application;
        }

       
        /// <summary>
        /// get sales reports
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="productId"></param>
        /// <param name="agentId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("sales")]
        public async Task<ActionResult<IEnumerable<SalesReportRecord>>> GetSalesReports(DateTime? startDate, DateTime? endDate, int? productId, Guid? agentId)
        {
            if (endDate.HasValue)
            {
                endDate = endDate.Value.ToLocalTime().AddHours(20);
            }
            var result = await _application.GetSalesReport(startDate, endDate,productId, agentId);
            return Ok(result);
        }

        /// <summary>
        /// get sales dashboard
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("sales-dashboard")]
        public async Task<ActionResult<IEnumerable<SalesDashboard>>> GetSalesDashboard(DateTime? startDate, DateTime? endDate)
        {
            var result = await _application.GetSalesDashboard(startDate,endDate);
            return Ok(result);
        }


        /// <summary>
        /// get earned commissions
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("earned-commissions")]
        public async Task<ActionResult<IEnumerable<CommissionReportRecord>>> GetEarnedCommissions(DateTime? startDate, DateTime? endDate)
        {
            if (endDate.HasValue)
            {
                endDate = endDate.Value.Date.ToLocalTime().AddHours(20);
            }
            var result = await _application.GetEarnedCommissions(startDate,endDate);
            return Ok(result);
        }

        /// <summary>
        /// get paid commissions
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("paid-commissions")]
        public async Task<ActionResult<IEnumerable<CommissionReportRecord>>> GetPaidCommissions(DateTime? startDate, DateTime? endDate)
        {
            if (endDate.HasValue)
            {
                endDate = endDate.Value.ToLocalTime().AddHours(20);
            }
            var result = await _application.GetPaidCommissions(startDate, endDate);
            return Ok(result);
        }

        /// <summary>
        /// get unpaid commissions
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("unpaid-commissions")]
        public async Task<ActionResult<IEnumerable<CommissionReportRecord>>> GetUnpaidCommissions(DateTime? startDate, DateTime? endDate)
        {
            if (endDate.HasValue)
            {
                endDate = endDate.Value.ToLocalTime().AddHours(20);
            }
            var result = await _application.GetUnpaidCommissions(startDate, endDate);
            return Ok(result);
        }


        /// <summary>
        /// get unprocessed commission
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("unprocessed-commissions")]
        public async Task<ActionResult<IEnumerable<CommissionReportRecord>>> GetUnprocessedCommissions(DateTime? startDate, DateTime? endDate)
        {
            if (endDate.HasValue)
            {
                endDate = endDate.Value.ToLocalTime().AddHours(20);
            }
            var result = await _application.GetUnprocessedCommissions(startDate, endDate);
            return Ok(result);
        }
    }
}
