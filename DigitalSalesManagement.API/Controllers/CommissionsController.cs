using DigitalSalesManagement.Abstractions.Models;
using DigitalSalesManagement.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Veneka.Platform.Common;

namespace DigitalSalesManagement.API.Controllers
{
    [Route("api/commissions/plans")]
    [ApiController]
    public class CommissionsController : ControllerBase
    {
        private ILogger<CommissionsController> _logger;
        private IDigitialSalesApplication _application;

        public CommissionsController(ILogger<CommissionsController> logger, IDigitialSalesApplication application)
        {
            _logger = logger;
            _application = application;
        }


        /// <summary>
        /// creates a commission plan
        /// </summary>
        /// <param name="plan"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<CommandResult<CommisionPlan>>> Create([FromBody] CommisionPlan plan)
        {
            var data = JsonConvert.SerializeObject(plan);
            _logger.LogInformation(data);

            var result = await _application.CreateCommissionPlan(plan);
            if (result.Accepted)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        /// <summary>
        /// updates a commision plan
        /// </summary>
        /// <param name="plan"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        public async Task<ActionResult<CommandResult>> UpdateCommissionPlan([FromBody] CommisionPlan plan)
        {
            var data = JsonConvert.SerializeObject(plan);
            _logger.LogInformation(data);

            var result = await _application.UpdateCommissionPlan(plan);
            if (result.Accepted)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }


        /// <summary>
        /// removes commision plan details
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{planId}")]
        public async Task<ActionResult<CommandResult>> DeleteCommissionPlan(Guid planId)
        {

            var result = await _application.DeleteCommissionPlan(planId);
            if (result.Accepted)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }


        /// <summary>
        /// gets commission plans for a product
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{productId}")]
        public async Task<ActionResult<IEnumerable<CommisionPlan>>> GetProductCommisionPlans(int productId)
        {
            var result = await _application.GetProductCommissionPlans(productId);
            if (result.ToList().Count < 1)
            {
                return NoContent();
            }
            return Ok(result);
        }

        /// <summary>
        /// gets all commission plans
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<IEnumerable<CommissionPlanResponse>>> GetCommisionPlans()
        {
            var result = await _application.GetCommisionPlans();
            return Ok(result);
        }


        /// <summary>
        /// assign commission plan to agent
        /// </summary>
        /// <param name="commission"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("agent-commissions")]
        public async Task<ActionResult<CommandResult<CommisionPlan>>> CreateAgentCommission([FromBody] AgentCommission commission)
        {
            var data = JsonConvert.SerializeObject(commission);
            _logger.LogInformation(data);

            var result = await _application.CreateAgentCommission(commission);
            if (result.Accepted)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        /// <summary>
        /// remove commission plan assignment from agent
        /// </summary>
        /// <param name="agentCommissionId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("agent-commissions/{agentCommissionId}")]
        public async Task<ActionResult<CommandResult>> AgentCommission(Guid agentCommissionId)
        {

            var result = await _application.DeleteAgentCommision(agentCommissionId);
            if (result.Accepted)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        /// <summary>
        /// get agents assigned commission plans
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("agent-commissions/{agentId}")]
        public async Task<ActionResult<IEnumerable<AgentCommissionRead>>> GetAgentCommissions(Guid agentId)
        {
            var result = await _application.GetAgentCommissions(agentId);
            if (result.ToList().Count < 1)
            {
                return NoContent();
            }
            return Ok(result);
        }

        /// <summary>
        /// get an agents paid commissions 
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("agent-paid-commissions/{agentId}")]
        public async Task<ActionResult<IEnumerable<AgentCalculatedCommission>>> GetAgentPaidCommissions(Guid agentId, DateTime? startDate, DateTime? endDate)
        {
            if (endDate.HasValue)
            {
                endDate = endDate.Value.ToLocalTime().AddHours(20);
            }
            var result = await _application.GetAgentPaidCommissions(agentId, startDate, endDate);
            if (result.ToList().Count < 1)
            {
                return NoContent();
            }
            return Ok(result);
        }

        /// <summary>
        /// get an agents unpaid commissions 
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("agent-unpaid-commissions/{agentId}")]
        public async Task<ActionResult<IEnumerable<AgentCalculatedCommission>>> GetAgentUnpaidCommissions(Guid agentId, DateTime? startDate, DateTime? endDate)
        {
            if (endDate.HasValue)
            {
                endDate = endDate.Value.ToLocalTime().AddHours(20);
            }
            var result = await _application.GetAgentUnpaidCommissions(agentId, startDate, endDate);
            if (result.ToList().Count < 1)
            {
                return NoContent();
            }
            return Ok(result);
        }


        /// <summary>
        /// rerun an agents schedule
        /// </summary>
        /// <param name="commissions"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("rerun-agent-commissions")]
        public async Task<ActionResult<CommandResult>> RecalculateAgentCommissions(Guid agentId, DateTime startDate, DateTime endDate)
        {
            var result = await _application.RecalculateAgentCommissions(agentId, startDate, endDate);
            if (result.Accepted)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
      
        }


        /// <summary>
        /// redo commission shedules
        /// </summary>
        /// <param name="commissions"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("rerun-commission-schedule")]
        public async Task<ActionResult<CommandResult>> RerunCommissionSchedule([FromBody] List<Guid> agents, DateTime startDate, DateTime endDate)
        {
            var result = await _application.RescheduleAgentCommissions(agents, startDate, endDate);
            if (result.Accepted)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }

        }




    }
}
