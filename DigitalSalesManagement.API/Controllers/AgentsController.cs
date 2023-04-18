using DigitalSalesManagement.Abstractions.Models;
using DigitalSalesManagement.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Veneka.Platform.Common;

namespace DigitalSalesManagement.API.Controllers
{
    [Route("api/agents")]
    [ApiController]
    public class AgentsController : ControllerBase
    {
        private ILogger<AgentsController> _logger;
        private IDigitialSalesApplication _application;

        public AgentsController(ILogger<AgentsController> logger, IDigitialSalesApplication application)
        {
            _logger = logger;
            _application = application;
        }

        /// <summary>
        /// creates agent's record
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<object>> Create([FromBody]AgentWrite agent)
        {
            var data = JsonConvert.SerializeObject(agent);
            _logger.LogInformation(data);

            var result = await _application.CreateAgent(agent);
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
        /// updates agents details
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        public async Task<ActionResult<object>> UpdateAgent([FromBody] AgentWrite agent)
        {
            var data = JsonConvert.SerializeObject(agent);
            _logger.LogInformation(data);

            var result = await _application.UpdateAgent (agent);
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
        /// 
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("calculate-agent-commission")]
        public async Task<ActionResult<CommandResult>> CalculateAgentCommission([FromBody] AgentCommissionFee agent)
        {
            var data = JsonConvert.SerializeObject(agent);
            _logger.LogInformation(data);

            var result = await _application.CalculateAgentCommission(agent);
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
        /// removes agent record
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{agentId}")]
        public async Task<ActionResult<CommandResult>> DeleteAgent(Guid agentId)
        {
            
            var result = await _application.DeleteAgent(agentId);
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
        /// verify agent's existence by agent code
        /// </summary>
        /// <param name="agentCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("verify/{agentCode}")]
        public async Task<ActionResult<CommandResult>> VerifyAgent(string agentCode)
        {
            var agent = await _application.GetAgentByCode(agentCode);
            var data = JsonConvert.SerializeObject(agent);
            _logger.LogInformation($"agent details :  {data}");
            if (agent != null)
            {
                return Ok(agent);
            }
            else
            {
                return NoContent();
            }
            
        }

        /// <summary>
        /// get all agent records
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<IEnumerable<Agent>>> GetAgents()
        {
            var result = await _application.GetAgents();
            return Ok(result);
        }

        /// <summary>
        /// get all agent sales
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("sales/{agentId}")]
        public async Task<ActionResult<IEnumerable<Agent>>> GetAgentSales(Guid agentId, DateTime? startDate, DateTime? endDate)
        {
            if (endDate.HasValue)
            {
                endDate = endDate.Value.ToLocalTime().AddHours(20);
            }
            var result = await _application.GetAgentSales(agentId, startDate, endDate);
            return Ok(result);
        }

        /// <summary>
        /// get agent dashboard
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("dashboards")]
        public async Task<ActionResult<IEnumerable<AgentDashboard>>> GetAgentDashboards(DateTime? startDate, DateTime? endDate)
        {
            var result = await _application.GetAgentDashboards(startDate, endDate);
            return Ok(result);
        }

        /// <summary>
        /// agent commission approval
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("agent-commission-approvals")]
        public async Task<ActionResult<CommandResult>> CreateAgentCommissionApproval([FromBody] AgentCommissionApproval agent)
        {
            var data = JsonConvert.SerializeObject(agent);
            _logger.LogInformation(data);

            var result = await _application.CreateApproval(agent);
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
        /// approve a list of commission approvals
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("agent-commission-approvals/approve-all")]
        public async Task<ActionResult<CommandResult>> ApproveAllAgentCommissionApproval([FromBody] List<AgentCommissionApproval> dto)
        {
            var data = JsonConvert.SerializeObject(dto);
            _logger.LogInformation(data);

            var result = await _application.ApproveAllCommissions(dto);
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
        /// agent commission approval
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("agent-commission-approvals")]
        public async Task<ActionResult<CommandResult>> UpdateAgentCommissionApproval([FromBody] AgentCommissionApproval agent)
        {
            var data = JsonConvert.SerializeObject(agent);
            _logger.LogInformation(data);

            var result = await _application.UpdateApproval(agent);
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
        /// check agent approval status
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("agent-commission-approval")]
        public async Task<ActionResult<IEnumerable<AgentCommissionApproval>>> GetAgentAprroval(string month, string year)
        {
            var result = await _application.GetAgentCommissionApprovals(month, year);
            return Ok(result);
        }


        /// <summary>
        /// get total earned commsions per agent for approval
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("agent-total-earned-commission")]
        public async Task<ActionResult<IEnumerable<AgentEarnedCommission>>> GetTotalAgentEarnedCommission(DateTime? startDate, DateTime?  endDate)
        {
            var result = await _application.GetTotalAgentEarnedCommissions(startDate, endDate);
            return Ok(result);
        }



    }
}
