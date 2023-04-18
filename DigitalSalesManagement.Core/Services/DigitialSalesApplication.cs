using AutoMapper;
using DigitalSalesManagement.Abstractions;
using DigitalSalesManagement.Abstractions.Entities;
using DigitalSalesManagement.Abstractions.Enums;
using DigitalSalesManagement.Abstractions.Models;
using DigitalSalesManagement.Abstractions.Repositories;
using DigitalSalesManagement.Abstractions.Services;
using DigitalSalesManagement.Core.Aggregates;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Veneka.Platform.Common;
using Veneka.Platform.Common.Enums;

namespace DigitalSalesManagement.Core.Services
{
    public class DigitialSalesApplication : IDigitialSalesApplication
    {
        private readonly IAgentRepository _agentRepository;
        private readonly ICommisionPlanRepository _commisionPlanRepository;
        private readonly IAgentCommissionRepository _agentCommissionRepository;
        private readonly ILogger<DigitialSalesApplication> _logger;
        private readonly IAgentCalculatedCommissionRepository _agentCalculatedCommissionRepository;
        private readonly IAgentCommissionApprovalRepository _agentCommissionApprovalRepository;
        private readonly IMapper _mapper;
        private readonly IOptions<ApplicationSettings> _config;
        private readonly IHttpClientFactory _clientFactory;

        public DigitialSalesApplication(IAgentRepository agentRepository, IAgentCommissionApprovalRepository agentCommissionApprovalRepository, ICommisionPlanRepository commisionPlanRepository, IAgentCommissionRepository agentCommissionRepository, ILogger<DigitialSalesApplication> logger, IAgentCalculatedCommissionRepository agentCalculatedCommissionRepository, IMapper mapper, IOptions<ApplicationSettings> config)
        {
            _agentCommissionApprovalRepository = agentCommissionApprovalRepository;
            _agentRepository = agentRepository;
            _commisionPlanRepository = commisionPlanRepository;
            _agentCommissionRepository = agentCommissionRepository;
            _logger = logger;
            _agentCalculatedCommissionRepository = agentCalculatedCommissionRepository;
            _mapper = mapper;
            _config = config;
        }

        public async Task<CommandResult<AgentCalculatedCommission>> AddCalculatedCommission(AgentCalculatedCommission commission)
        {
            //initiliase commission record
            _logger.LogInformation("Initialise agent calculated commission record.......");
            var entity = await _agentCalculatedCommissionRepository.LoadAggregateAsync(Guid.Empty);

            //init
            var aggregate = new AgentCalculatedCommissionAggregate(entity);
            commission.Id = aggregate.Entity.Id;
            var commandResult = new CommandResult<AgentCalculatedCommission>(aggregate.Id, commission, true);
            var result = aggregate.Save(commission);

            //validate
            if (result.IsValid)
            {
                //save agent calculated commission record in db
                _logger.LogInformation("Saving agent calculated commission details in the db...........");
                await _agentCalculatedCommissionRepository.SaveAggregateAsync(aggregate.Entity);

            }
            else
            {
                commandResult = new CommandResult<AgentCalculatedCommission>(Guid.Empty, null, false);
                foreach (var msg in result.ValidationMessages)
                {
                    commandResult.AddResultMessage(msg.MessageType, msg.Code, msg.Message);
                }
            }

            return commandResult;
        }

        public async Task<CommandResult<Agent>> CreateAgent(AgentWrite agent)
        {
            //initiliase agent record
            _logger.LogInformation("Initialise agent record.......");
            var entity = await _agentRepository.LoadAggregateAsync(Guid.Empty);

            //init
            var aggregate = new AgentAggregate(entity);
            agent.RegisteredDate = DateTime.Now.Date;
            agent.Id = aggregate.Entity.Id;
            var commandResult = new CommandResult<Agent>(aggregate.Id, agent, true);
            var result = aggregate.Save(agent);


            //check if agent code is unique
            _logger.LogInformation("Checking if agent code is unique..............");
            var check = await GetAgentByCode(agent.AgentCode);
            if (check != null)
            {
                commandResult = new CommandResult<Agent>(Guid.Empty, check, false);
                commandResult.AddResultMessage(ResultMessageType.Error, "Agent code", "agent code already exists for another agent");
                _logger.LogInformation("agent code already exists for another agent");
                return commandResult;
            }


            //validate
            if (result.IsValid)
            {
                //save agent record in db
                _logger.LogInformation("Saving agent details in the db...........");
                await _agentRepository.SaveAggregateAsync(aggregate.Entity);

                //add agent commissions
                await AddAgentCommissions(agent.CommissionPlanIds, aggregate.Entity.Id);
            }
            else
            {
                commandResult = new CommandResult<Agent>(Guid.Empty, null, false);
                foreach (var msg in result.ValidationMessages)
                {
                    commandResult.AddResultMessage(msg.MessageType, msg.Code, msg.Message);
                }
            }

            return commandResult;
        }




        public async Task<CommandResult<AgentCommission>> CreateAgentCommission(AgentCommission commission)
        {
            //initiliase agent commission record
            _logger.LogInformation("Initialise agent  record.......");
            var entity = await _agentCommissionRepository.LoadAggregateAsync(Guid.Empty);

            //init
            var aggregate = new AgentCommissionAggregate(entity);
            commission.Id = aggregate.Entity.Id;
            var commandResult = new CommandResult<AgentCommission>(aggregate.Id, commission, true);
            var result = aggregate.Save(commission);

            //validate
            if (result.IsValid)
            {
                //save agent commission record in db
                _logger.LogInformation("Saving agent commission details in the db...........");
                await _agentCommissionRepository.SaveAggregateAsync(aggregate.Entity);
            }
            else
            {
                commandResult = new CommandResult<AgentCommission>(Guid.Empty, null, false);
                foreach (var msg in result.ValidationMessages)
                {
                    commandResult.AddResultMessage(msg.MessageType, msg.Code, msg.Message);
                }
            }

            return commandResult;
        }

        public async Task<CommandResult<CommisionPlan>> CreateCommissionPlan(CommisionPlan plan)
        {
            //initiliase commission plan record
            _logger.LogInformation("Initialise commission plan record.......");
            var entity = await _commisionPlanRepository.LoadAggregateAsync(Guid.Empty);

            //init
            var aggregate = new CommisionPlanAggregate(entity);
            plan.Id = aggregate.Entity.Id;
            var commandResult = new CommandResult<CommisionPlan>(aggregate.Id, plan, true);
            var result = aggregate.Save(plan);

            //validate
            if (result.IsValid)
            {
                //save commission plan record in db
                _logger.LogInformation("Saving agent commission details in the db...........");
                await _commisionPlanRepository.SaveAggregateAsync(aggregate.Entity);
            }
            else
            {
                commandResult = new CommandResult<CommisionPlan>(Guid.Empty, null, false);
                foreach (var msg in result.ValidationMessages)
                {
                    commandResult.AddResultMessage(msg.MessageType, msg.Code, msg.Message);
                }
            }

            return commandResult;
        }

        public async Task<CommandResult> DeleteAgent(Guid agentId)
        {
            //initiliase agent record
            _logger.LogInformation("Searching agent record.......");
            var entity = await _agentRepository.LoadAggregateAsync(agentId);

            //init
            var aggregate = new AgentAggregate(entity);
            var commandResult = new CommandResult(aggregate.Id, true);

            //check if agent exists
            if (entity.Id != agentId)
            {
                commandResult = new CommandResult(Guid.Empty, false);
                commandResult.AddResultMessage(ResultMessageType.Error, "404", "agent not found");
                _logger.LogInformation("agent not found");
                return commandResult;
            }

            //remove agent commissions
            _logger.LogInformation("Removing agent commissions");
            var search = new List<SearchParameter>() { new SearchParameter { Name = "AGENTID", Value = agentId.ToString() } };
            var agentCommissions = await _agentCommissionRepository.FindAggregatesAsync(search);
            foreach (var item in agentCommissions)
            {
                await DeleteAgentCommision(item.Id);
            }


            //delete agent
            _logger.LogInformation("Removing agent details");
            aggregate.DeleteRecord();
            await _agentRepository.SaveAggregateAsync(aggregate.Entity);

            return commandResult;

        }

        public async Task<CommandResult> DeleteAgentCommision(Guid agentCommissionId)
        {
            //initiliase agent commission record
            _logger.LogInformation("Searching agent commission record.......");
            var entity = await _agentCommissionRepository.LoadAggregateAsync(agentCommissionId);

            //init
            var aggregate = new AgentCommissionAggregate(entity);
            var commandResult = new CommandResult(aggregate.Id, true);

            //check if agent commission exists
            if (entity.Id != agentCommissionId)
            {
                commandResult = new CommandResult(Guid.Empty, false);
                commandResult.AddResultMessage(ResultMessageType.Error, "404", "agent commission not found");
                _logger.LogInformation("agent commission not found");
                return commandResult;
            }

            //delete agent commission
            _logger.LogInformation("Removing agent commission details");
            aggregate.DeleteRecord();
            await _agentCommissionRepository.SaveAggregateAsync(aggregate.Entity);

            return commandResult;
        }

        public async Task<CommandResult> DeleteCommissionPlan(Guid commissionPlanId)
        {
            //initiliase commission plan record
            _logger.LogInformation("Searching commission plan record.......");
            var entity = await _commisionPlanRepository.LoadAggregateAsync(commissionPlanId);

            //init
            var aggregate = new CommisionPlanAggregate(entity);
            var commandResult = new CommandResult(aggregate.Id, true);

            //check if commission plan exists
            if (entity.Id != commissionPlanId)
            {
                commandResult = new CommandResult(Guid.Empty, false);
                commandResult.AddResultMessage(ResultMessageType.Error, "404", " commission plan not found");
                _logger.LogInformation(" commission plan not found");
                return commandResult;
            }

            //delete commission plan
            _logger.LogInformation("Removing commission plan details");
            aggregate.DeleteRecord();
            await _commisionPlanRepository.SaveAggregateAsync(aggregate.Entity);

            //remove assinged commissions
            _logger.LogInformation("Removing assigned commissions to agents......");
            var search = new List<SearchParameter> { new SearchParameter { Name = "COMMISSIONPLANID", Value = aggregate.Entity.Id.ToString() } };
            var agentCommissionPlans = await _agentCommissionRepository.FindAggregatesAsync(search);
            foreach (var plan in agentCommissionPlans)
            {
                await DeleteAgentCommision(plan.Id);
            }


            return commandResult;
        }
        /// <summary>
        /// get agent details by code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<Agent> GetAgentByCode(string code)
        {
            //load agent details
            _logger.LogInformation("Loading agent details by agent code........");
            var search = new List<SearchParameter>() { new SearchParameter { Name = "AGENTCODE", Value = code } };
            var agentEntity = (await _agentRepository.FindAggregatesAsync(search)).FirstOrDefault();
            var agent = _mapper.Map<AgentEntity, Agent>(agentEntity);
            return agent;
        }

        public async Task<IEnumerable<AgentCommissionRead>> GetAgentCommissions(Guid agentId)
        {
            //loading agent commission records
            _logger.LogInformation("Loading agent commission records..............");
            var search = new List<SearchParameter>()
            {
                new SearchParameter{ Name = "AGENTID", Value=agentId.ToString() }
            };

            var result = new List<AgentCommissionRead>();

            var agentCommissionEntities = await _agentCommissionRepository.FindAggregatesAsync(search);
            var agentCommissions = _mapper.Map<IEnumerable<AgentCommissionEntity>, IEnumerable<AgentCommission>>(agentCommissionEntities);

            foreach (var commission in agentCommissions)
            {
                var commissionDetails = new AgentCommissionRead { AgentId = commission.AgentId };

                var plan = await _commisionPlanRepository.LoadAggregateAsync(commission.AgentCommissionPlanId);
                commissionDetails.Description = plan.Description;
                commissionDetails.FixedValue = plan.FixedValue;
                commissionDetails.Name = plan.Name;
                commissionDetails.PercentageValue = plan.PercentageValue;
                commissionDetails.ProductId = plan.ProductId;
                commissionDetails.Id = commission.AgentCommissionPlanId;

                result.Add(commissionDetails);
            }

            return result;
        }

        public async Task<Agent> GetAgentDetails(Guid agentId)
        {
            //load agentDetails
            _logger.LogInformation("Loading agent details..........");
            var agentEntity = await _agentRepository.LoadAggregateAsync(agentId);
            var agent = _mapper.Map<Agent>(agentEntity);
            return agent;
        }

        public async Task<IEnumerable<AgentCalculatedCommission>> GetAgentPaidCommissions(Guid agentId, DateTime? startDate, DateTime? endDate)
        {
            //load agent commissions 
            _logger.LogInformation("Loading agent calculated commissions details........");
            var calculatedCommissions = await GetAgentCalculatedCommissions(agentId);

            //filter agent commissions date range
            if (startDate.HasValue && endDate.HasValue)
            {

                calculatedCommissions = calculatedCommissions.Where(s => s.Date > startDate.Value && s.Date <= endDate.Value);
            }

            var paidCommissions = calculatedCommissions.Where(s => s.IsPaid).OrderByDescending(s => s.Date).Select(s => s);
            return paidCommissions;
        }

        private async Task<IEnumerable<AgentCalculatedCommission>> GetAgentCalculatedCommissions(Guid agentId)
        {
            //load  commissions 
            _logger.LogInformation("Loading agent commssions............. ");
            var search = new List<SearchParameter> { new SearchParameter { Name = "AGENTID", Value = agentId.ToString() } };
            var agentCommissionEntities = await _agentCalculatedCommissionRepository.FindAggregatesAsync(search);
            var paidCommissions = _mapper.Map<IEnumerable<AgentCalculatedCommissionEntity>, IEnumerable<AgentCalculatedCommission>>(agentCommissionEntities);
            return paidCommissions;
        }

        public async Task<IEnumerable<Agent>> GetAgents()
        {
            //loading agent records
            _logger.LogInformation("Loading agent records..............");
            var search = new List<SearchParameter>();
            var agentEntities = await _agentRepository.FindAggregatesAsync(search);
            var agents = _mapper.Map<IEnumerable<AgentEntity>, IEnumerable<Agent>>(agentEntities);
            return agents;
        }

        public async Task<IEnumerable<CommissionPlanResponse>> GetCommisionPlans()
        {
            //loading  commission plan records
            _logger.LogInformation("Loading  commission plans records..............");
            var search = new List<SearchParameter>();
            var commissionPlanEntities = await _commisionPlanRepository.FindAggregatesAsync(search);
            var commissionPlans = _mapper.Map<IEnumerable<CommisionPlanEntity>, IEnumerable<CommisionPlan>>(commissionPlanEntities);

            var responseList = new List<CommissionPlanResponse>();
            var productCommissionPlans = commissionPlans.GroupBy(s => s.ProductId);
            foreach (var group in productCommissionPlans)
            {
                var response = new CommissionPlanResponse
                {
                    ProductId = group.Key.Value,
                };

                foreach (var plan in group)
                {
                    response.CommissionPlans.Add(plan);
                }
                responseList.Add(response);
            }

            return responseList;

        }

        public async Task<IEnumerable<CommisionPlan>> GetProductCommissionPlans(int productId)
        {
            //loading  commission plan records
            _logger.LogInformation("Loading  commission plans records..............");
            var search = new List<SearchParameter>()
            {
                new SearchParameter{ Name = "PRODUCTID", Value = productId.ToString() }
            };
            var commissionPlanEntities = await _commisionPlanRepository.FindAggregatesAsync(search);
            var commissionPlans = _mapper.Map<IEnumerable<CommisionPlanEntity>, IEnumerable<CommisionPlan>>(commissionPlanEntities);
            return commissionPlans;
        }

        public async Task<IEnumerable<AgentCalculatedCommission>> GetAgentUnpaidCommissions(Guid agentId, DateTime? startDate, DateTime? endDate)
        {
            //load calculated commissions
            _logger.LogInformation("Loading agents unpaid commissions........");
            var calculatedCommissions = await GetAgentCalculatedCommissions(agentId);

            if (startDate.HasValue && endDate.HasValue)
            {
                endDate = endDate.Value.AddDays(1);
                calculatedCommissions = calculatedCommissions.ToList().Where(s => s.Date > startDate.Value && s.Date <= endDate.Value).OrderByDescending(s => s.Date).Select(s => s);
            }

            var paidCommissions = calculatedCommissions.Where(s => !s.IsPaid).OrderByDescending(s => s.Date).Select(s => s);
            return paidCommissions;
        }


        public async Task<IEnumerable<AgentCalculatedCommission>> GetAgentEarnedCommissions(Guid agentId, DateTime? startDate, DateTime? endDate)
        {
            //load earned commissions
            _logger.LogInformation("Loading agents earned commissions........");
            var calculatedCommissions = await GetAgentCalculatedCommissions(agentId);

            if (startDate.HasValue && endDate.HasValue)
            {
                calculatedCommissions = calculatedCommissions.ToList().Where(s => s.Date.Date > startDate.Value.Date && s.Date <= endDate.Value.Date).Select(s => s);
            }

            var earnedCommissions = calculatedCommissions.Where(s => !s.HasError).OrderByDescending(s => s.Date).Select(s => s);
            return earnedCommissions;
        }

        public async Task<IEnumerable<AgentCalculatedCommission>> GetAgentUnProcessedCommissions(Guid agentId, DateTime? startDate, DateTime? endDate)
        {
            //load earned commissions
            _logger.LogInformation("Loading agents earned commissions........");
            var calculatedCommissions = await GetAgentCalculatedCommissions(agentId);

            if (startDate.HasValue && endDate.HasValue)
            {
                calculatedCommissions = calculatedCommissions.ToList().Where(s => s.Date.Date > startDate.Value.Date && s.Date <= endDate.Value.Date).Select(s => s);
            }

            var unprocessedCommissions = calculatedCommissions.Where(s => s.HasError).OrderByDescending(s => s.Date).Select(s => s);
            return unprocessedCommissions;
        }

        public async Task<CommandResult> UpdateAgent(AgentWrite agent)
        {
            //initiliase agent record
            _logger.LogInformation("Searching agent record.......");
            var entity = await _agentRepository.LoadAggregateAsync(agent.Id);

            //init
            var aggregate = new AgentAggregate(entity);
            var commandResult = new CommandResult(aggregate.Id, true);
            var result = aggregate.Save(agent);

            //check if agent exists
            if (entity.Id != agent.Id)
            {
                commandResult = new CommandResult(Guid.Empty, false);
                commandResult.AddResultMessage(ResultMessageType.Error, "404", "agent not found");
                _logger.LogInformation("agent not found");
                return commandResult;
            }

            //validate
            if (result.IsValid)
            {
                //save agent record in db
                _logger.LogInformation("Saving agent details in the db...........");
                await _agentRepository.SaveAggregateAsync(aggregate.Entity);

                //update commission plans 
                _logger.LogInformation("Updating agent's assigned commissions.........");
                if (agent.CommissionPlanIds.Count > 0)
                {
                    //loading agent commission records
                    _logger.LogInformation("Loading agent commission records..............");
                    var search = new List<SearchParameter>()
                    {
                        new SearchParameter{ Name = "AGENTID", Value=agent.Id.ToString() }
                    };
                    var agentCommissionEntities = await _agentCommissionRepository.FindAggregatesAsync(search);


                    foreach (var commission in agentCommissionEntities)
                    {
                        await DeleteAgentCommision(commission.Id);
                    }

                    //add new commission plans
                    await AddAgentCommissions(agent.CommissionPlanIds, entity.Id);

                }

            }
            else
            {
                commandResult = new CommandResult(Guid.Empty, false);
                foreach (var msg in result.ValidationMessages)
                {
                    commandResult.AddResultMessage(msg.MessageType, msg.Code, msg.Message);
                }
            }

            return commandResult;
        }

        public async Task<CommandResult> UpdateCalculatedCommission(AgentCalculatedCommission commission)
        {
            //initiliase commission record
            _logger.LogInformation("Searching calculated commission record.......");
            var entity = await _agentCalculatedCommissionRepository.LoadAggregateAsync(commission.Id);

            //init
            var aggregate = new AgentCalculatedCommissionAggregate(entity);
            var commandResult = new CommandResult(aggregate.Id, true);
            var result = aggregate.Save(commission);

            //check if plan exists
            if (entity.Id != commission.Id)
            {
                commandResult = new CommandResult(Guid.Empty, false);
                commandResult.AddResultMessage(ResultMessageType.Error, "404", "calculated commission not found");
                _logger.LogInformation("calculated commission not found");
                return commandResult;
            }

            //validate
            if (result.IsValid)
            {
                //save calculated commission record in db
                _logger.LogInformation("Saving calculated commission details in the db...........");
                await _agentCalculatedCommissionRepository.SaveAggregateAsync(aggregate.Entity);
            }
            else
            {
                commandResult = new CommandResult(Guid.Empty, false);
                foreach (var msg in result.ValidationMessages)
                {
                    commandResult.AddResultMessage(msg.MessageType, msg.Code, msg.Message);
                }
            }

            return commandResult;
        }

        public async Task<CommandResult> UpdateCommissionPlan(CommisionPlan plan)
        {
            //initiliase plan record
            _logger.LogInformation("Searching plan record.......");
            var entity = await _commisionPlanRepository.LoadAggregateAsync(plan.Id);

            //init
            var aggregate = new CommisionPlanAggregate(entity);
            var commandResult = new CommandResult(aggregate.Id, true);
            var result = aggregate.Save(plan);

            //check if plan exists
            if (entity.Id != plan.Id)
            {
                commandResult = new CommandResult(Guid.Empty, false);
                commandResult.AddResultMessage(ResultMessageType.Error, "404", "plan not found");
                _logger.LogInformation("plan not found");
                return commandResult;
            }

            //validate
            if (result.IsValid)
            {
                //save plan record in db
                _logger.LogInformation("Saving plan details in the db...........");
                await _commisionPlanRepository.SaveAggregateAsync(aggregate.Entity);
            }
            else
            {
                commandResult = new CommandResult(Guid.Empty, false);
                foreach (var msg in result.ValidationMessages)
                {
                    commandResult.AddResultMessage(msg.MessageType, msg.Code, msg.Message);
                }
            }

            return commandResult;
        }

        public async Task<CommandResult> CalculateAgentCommission(AgentCommissionFee commissionFee)
        {
            //load agent details 
            _logger.LogInformation($"Loading agent {commissionFee.AgentCode} details..........");
            var commandResult = new CommandResult(Guid.Empty, false);

            var agent = await GetAgentByCode(commissionFee.AgentCode);


            if (agent != null)
            {
                //load agent's assigned commissions
                var agentAssignedCommissions = await GetAgentCommissions(agent.Id);
                var commissionPlanForProduct = agentAssignedCommissions.Where(s => s.ProductId == commissionFee.ProductId).Select(s => s).FirstOrDefault();
                if (commissionPlanForProduct != null)
                {
                    _logger.LogInformation("Calculating agent's commission amount.....");
                    var calculatedCommission = new AgentCalculatedCommission
                    {
                        Fee = commissionFee.Fee,
                        AgentId = agent.Id,
                        CommissionPlanId = commissionPlanForProduct.Id,
                        FixedValue = commissionPlanForProduct.FixedValue,
                        PercentageValue = commissionPlanForProduct.PercentageValue,
                        Name = commissionPlanForProduct.Name,
                        ProductId = commissionFee.ProductId,
                        Amount = 0,
                        Date = DateTime.Now,
                        CardReferenceNumber = commissionFee.CardRequestRef
                    };

                    //calculate commission
                    var percentageAmount = (commissionPlanForProduct.PercentageValue) / 100 * commissionFee.Fee;
                    calculatedCommission.Amount = percentageAmount + commissionPlanForProduct.FixedValue;


                    var agentCalculatedCommissionResponse = await AddCalculatedCommission(calculatedCommission);
                    var response = JsonConvert.SerializeObject(agentCalculatedCommissionResponse);
                    _logger.LogInformation($"Added agent's calculated commission with response : {response}");

                    commandResult = new CommandResult(agent.Id, true);


                }
                else
                {
                    _logger.LogInformation($"Commission plan for product: {commissionFee.ProductId} and agent: {commissionFee.AgentCode} was not found..");
                    var calculatedCommission = new AgentCalculatedCommission
                    {
                        Fee = commissionFee.Fee,
                        AgentId = agent.Id,
                        ProductId = commissionFee.ProductId,
                        Amount = 0,
                        Date = DateTime.Now,
                        CardReferenceNumber = commissionFee.CardRequestRef,
                        HasError = true,
                        ErrorReason = $"Commission plan for product was not found..",
                        FixedValue = 0,
                        PercentageValue = 0,
                        IsPaid = false
                    };

                    var agentCalculatedCommissionResponse = await AddCalculatedCommission(calculatedCommission);
                    var response = JsonConvert.SerializeObject(agentCalculatedCommissionResponse);
                    _logger.LogInformation($"Added agent's calculated commission with response : {response}");

                    commandResult = new CommandResult(agent.Id, true);

                }
            }
            else
            {
                _logger.LogInformation("Agent not found..........");
            }

            return commandResult;
        }

        public async Task<IEnumerable<AgentSale>> GetAgentSales(Guid agentId, DateTime? startdate, DateTime? endDate)
        {
            //load sales
            _logger.LogInformation("Loading agent sales..............");
            var calculatedCommissions = await GetAgentCalculatedCommissions(agentId);

            //filter date range
            if (startdate.HasValue && endDate.HasValue)
            {

                calculatedCommissions = calculatedCommissions.ToList().Where(s => s.Date > startdate.Value && s.Date <= endDate.Value).OrderByDescending(s => s.Date).Select(s => s);
            }

            var sales = new List<AgentSale>();

            foreach (var item in calculatedCommissions)
            {
                var sale = new AgentSale
                {
                    AgentId = item.AgentId,
                    CardReferenceNumber = item.CardReferenceNumber,
                    Date = item.Date,
                    Fee = item.Fee,
                    Id = item.Id,
                    ProductId = item.ProductId
                };

                sales.Add(sale);
            }

            return sales;
        }


        private async Task AddAgentCommissions(List<Guid> commissionPlanIds, Guid agentId)
        {
            //add the corresponding agent commission plans
            foreach (var planId in commissionPlanIds)
            {
                _logger.LogInformation($"Adding commission plan {planId} to agent {agentId}.....");
                var agentCommission = new AgentCommission
                {
                    AgentId = agentId,
                    AgentCommissionPlanId = planId
                };

                var commissionResult = await CreateAgentCommission(agentCommission);
            }
        }

        public async Task<IEnumerable<SalesReportRecord>> GetSalesReport(DateTime? startDate, DateTime? endDate, int? productId, Guid? agentId)
        {
            //load  commissions 
            _logger.LogInformation("Loading agent commssions............. ");
            var search = new List<SearchParameter> { new SearchParameter { } };
            var agentCommissionEntities = await _agentCalculatedCommissionRepository.FindAggregatesAsync(search);

            if (startDate.HasValue && endDate.HasValue)
            {
                agentCommissionEntities = agentCommissionEntities.ToList().Where(s => s.Date > startDate.Value && s.Date <= endDate.Value).OrderByDescending(s => s.Date).Select(s => s);
            }

            if (productId.HasValue)
            {
                agentCommissionEntities = agentCommissionEntities.ToList().Where(s => s.ProductId == productId.Value).Select(s => s);
            }

            if (agentId.HasValue)
            {
                agentCommissionEntities = agentCommissionEntities.ToList().Where(s => s.AgentId == agentId.Value).Select(s => s);
            }


            var salesReport = new List<SalesReportRecord>();

            foreach (var item in agentCommissionEntities)
            {
                var salesRecord = new SalesReportRecord
                {

                    CardReferenceNumber = item.CardReferenceNumber,
                    Date = item.Date,
                    Fee = item.Fee,
                    CommissionPlan = item.Name,
                    ProductId = item.ProductId,
                    Id = item.Id
                };

                var agent = await _agentRepository.LoadAggregateAsync(item.AgentId);

                //<----------Agent exsitence check workaround ------------->
                if (agent.AgentCode != null)
                {
                    salesRecord.AgentName = $"{agent.Name} {agent.Surname}";
                    salesReport.Add(salesRecord);
                }
            }

            return salesReport;

        }

        public async Task<IEnumerable<AgentDashboard>> GetAgentDashboards(DateTime? startdate, DateTime? endDate)
        {
            //initialise
            var agentDashboards = new List<AgentDashboard>();

            //load all agents
            var agents = await GetAgents();

            foreach (var agent in agents)
            {
                var agentDashboard = new AgentDashboard(agent);

                //get agent sales
                var agentSales = await GetAgentSales(agent.Id, startdate, endDate);
                decimal totalSales = 0;
                foreach (var sale in agentSales)
                {
                    totalSales = sale.Fee + totalSales;
                }
                agentDashboard.TotalSales = totalSales;


                //get paid commissions
                var paidCommissions = await GetAgentPaidCommissions(agent.Id, startdate, endDate);
                decimal totalPaidCommissions = 0;
                foreach (var commission in paidCommissions)
                {
                    totalPaidCommissions = commission.Amount + totalPaidCommissions;
                }
                agentDashboard.TotalCommissionsPaid = totalPaidCommissions;


                //get unpaid commissions
                var unpaidCommissions = await GetAgentUnpaidCommissions(agent.Id, startdate, endDate);
                decimal totalUnpaidCommissions = 0;
                foreach (var commission in unpaidCommissions)
                {
                    totalUnpaidCommissions = commission.Amount + totalUnpaidCommissions;
                }
                agentDashboard.TotalCommissionUnpaid = totalUnpaidCommissions;


                //get earned commissions
                var earnedCommissions = await GetAgentEarnedCommissions(agent.Id, startdate, endDate);
                decimal totalEarnedCommissions = 0;
                foreach (var commission in earnedCommissions)
                {
                    totalEarnedCommissions = commission.Amount + totalEarnedCommissions;
                }
                agentDashboard.TotalCommissionsEarned = totalEarnedCommissions;

                agentDashboards.Add(agentDashboard);

            }
            return agentDashboards;

        }

        public async Task<SalesDashboard> GetSalesDashboard(DateTime? startdate, DateTime? endDate)
        {
            //load  commissions 
            _logger.LogInformation("Loading agent commssions............. ");
            var search = new List<SearchParameter> { new SearchParameter { } };
            var agentCommissionEntities = await _agentCalculatedCommissionRepository.FindAggregatesAsync(search);

            if (startdate.HasValue && endDate.HasValue)
            {
                agentCommissionEntities = agentCommissionEntities.ToList().Where(s => s.Date > startdate.Value && s.Date <= endDate.Value).Select(s => s);
            }

            var salesDashboard = new SalesDashboard();
            salesDashboard.Id = Guid.NewGuid();

            decimal totalSales = 0;
            decimal totalEarnedCommissions = 0;
            decimal totalPaidCommissions = 0;
            decimal totalUnpaidCommissions = 0;
            foreach (var sale in agentCommissionEntities)
            {
                totalSales = sale.Fee + totalSales;
                totalEarnedCommissions = sale.Amount + totalEarnedCommissions;
            }
            salesDashboard.TotalSales = totalSales;
            salesDashboard.SalesCount = agentCommissionEntities.Count();


            salesDashboard.TotalEarnedCommissions = totalEarnedCommissions;
            salesDashboard.EarnedCommissionsCount = agentCommissionEntities.Count();


            //get unpaid commissions
            var unpaidCommissions = agentCommissionEntities.Where(s => !s.IsPaid).Select(s => s);
            foreach (var sale in unpaidCommissions)
            {
                totalUnpaidCommissions = sale.Amount + totalUnpaidCommissions;
            }
            salesDashboard.TotalUnpaidCommissions = totalUnpaidCommissions;
            salesDashboard.UnpaidCommissionsCount = unpaidCommissions.Count();

            //get paid commssions
            var paidCommissions = agentCommissionEntities.Where(s => s.IsPaid).Select(s => s);
            foreach (var sale in paidCommissions)
            {
                totalPaidCommissions = sale.Amount + totalPaidCommissions;
            }
            salesDashboard.TotalUnpaidCommissions = totalPaidCommissions;
            salesDashboard.PaidCommissionsCount = paidCommissions.Count();

            return salesDashboard;


        }

        private async Task<IEnumerable<AgentCalculatedCommissionEntity>> GetAgentCalculatedCommissions(DateTime? startDate, DateTime? endDate)
        {
            //load  commissions 
            _logger.LogInformation("Loading agent commssions............. ");
            var search = new List<SearchParameter> { new SearchParameter { } };
            var agentCommissionEntities = await _agentCalculatedCommissionRepository.FindAggregatesAsync(search);

            if (startDate.HasValue && endDate.HasValue)
            {
                agentCommissionEntities = agentCommissionEntities.ToList().Where(s => s.Date > startDate.Value && s.Date <= endDate.Value).OrderByDescending(s => s.Date).Select(s => s);
            }

            return agentCommissionEntities;
        }

        public async Task<IEnumerable<CommissionReportRecord>> GetEarnedCommissions(DateTime? startDate, DateTime? endDate)
        {
            //load calculated commissions
            var commissions = await GetAgentCalculatedCommissions(startDate, endDate);
            commissions = commissions.Where(s => !s.HasError).OrderByDescending(s => s.Date).Select(s => s);
            var reports = new List<CommissionReportRecord>();

            foreach (var commission in commissions)
            {
                var reportRecord = new CommissionReportRecord(commission);
                var agent = await _agentRepository.LoadAggregateAsync(commission.AgentId);
                reportRecord.AgentName = $"{agent.Name}  {agent.Surname}";
                reports.Add(reportRecord);
            }

            return reports;
        }

        public async Task<IEnumerable<CommissionReportRecord>> GetPaidCommissions(DateTime? startDate, DateTime? endDate)
        {
            var commissions = await GetAgentCalculatedCommissions(startDate, endDate);
            commissions = commissions.Where(s => s.IsPaid).OrderByDescending(s => s.Date).Select(s => s);
            var reports = new List<CommissionReportRecord>();

            foreach (var commission in commissions)
            {
                var reportRecord = new CommissionReportRecord(commission);
                var agent = await _agentRepository.LoadAggregateAsync(commission.AgentId);
                reportRecord.AgentName = $"{agent.Name}  {agent.Surname}";
                reports.Add(reportRecord);
            }

            return reports;
        }

        public async Task<IEnumerable<CommissionReportRecord>> GetUnpaidCommissions(DateTime? startDate, DateTime? endDate)
        {
            var commissions = await GetAgentCalculatedCommissions(startDate, endDate);
            commissions = commissions.Where(s => !s.IsPaid).OrderByDescending(s => s.Date).Select(s => s);
            var reports = new List<CommissionReportRecord>();

            foreach (var commission in commissions)
            {
                var reportRecord = new CommissionReportRecord(commission);
                var agent = await _agentRepository.LoadAggregateAsync(commission.AgentId);
                reportRecord.AgentName = $"{agent.Name}  {agent.Surname}";
                reports.Add(reportRecord);
            }

            return reports;

        }

        public async Task<IEnumerable<CommissionReportRecord>> GetUnprocessedCommissions(DateTime? startDate, DateTime? endDate)
        {
            var commissions = await GetAgentCalculatedCommissions(startDate, endDate);
            commissions = commissions.Where(s => s.HasError).OrderByDescending(s => s.Date).Select(s => s);
            var reports = new List<CommissionReportRecord>();

            foreach (var commission in commissions)
            {
                var reportRecord = new CommissionReportRecord(commission);
                var agent = await _agentRepository.LoadAggregateAsync(commission.AgentId);
                reportRecord.AgentName = $"{agent.Name}  {agent.Surname}";
                reports.Add(reportRecord);
            }

            return reports;

        }

        private async Task<List<Product>> GetProductsAsync()
        {
            string result = null;
            //initialise client
            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Clear();

            // url = $"{_config.Value.FileProcessingUrl}";


            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, ""))
            {
                using (var response = await client
                     .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                     .ConfigureAwait(false))
                {
                    // response.EnsureSuccessStatusCode();
                    result = await response.Content.ReadAsStringAsync();
                }

            }
            return new List<Product>();
        }

        public async Task<CommandResult> CreateApproval(AgentCommissionApproval approval)
        {
            //initiliase commission approval record
            _logger.LogInformation("Initialise approval record.......");
            var entity = await _agentCommissionApprovalRepository.LoadAggregateAsync(Guid.Empty);

            //check if there is an exisiting record for the month
            _logger.LogInformation("Checking for an exisiting entity......");
            var exisitingEntity = (await _agentCommissionApprovalRepository.GetMonthlyApprovals(approval.Year, approval.Month, approval.AgentId)).FirstOrDefault();

            if (exisitingEntity != null)
            {
                entity = exisitingEntity;
            }


            //init
            var aggregate = new AgentCommissionApprovalAggregate(entity);
            approval.Id = aggregate.Entity.Id;


            var commandResult = new CommandResult(aggregate.Id, true);
            var result = aggregate.Save(approval);

            //validate
            if (result.IsValid)
            {
                //save commission approval in db
                _logger.LogInformation("Saving agent commission approval details in the db...........");
                await _agentCommissionApprovalRepository.SaveAggregateAsync(aggregate.Entity);
            }
            else
            {
                commandResult = new CommandResult<CommisionPlan>(Guid.Empty, null, false);
                foreach (var msg in result.ValidationMessages)
                {
                    commandResult.AddResultMessage(msg.MessageType, msg.Code, msg.Message);
                }
            }

            return commandResult;
        }

        public async Task<CommandResult> UpdateApproval(AgentCommissionApproval approval)
        {
            //initiliase approval record
            _logger.LogInformation("Searching approval record.......");
            var entity = await _agentCommissionApprovalRepository.LoadAggregateAsync(approval.Id);

            //init
            var aggregate = new AgentCommissionApprovalAggregate(entity);
            var commandResult = new CommandResult(aggregate.Id, true);
            var result = aggregate.Save(approval);

            //check if plan exists
            if (entity.Id != approval.Id)
            {
                commandResult = new CommandResult(Guid.Empty, false);
                commandResult.AddResultMessage(ResultMessageType.Error, "404", "approval not found");
                _logger.LogInformation("approval not found");
                return commandResult;
            }

            //validate
            if (result.IsValid)
            {
                //save plan record in db
                _logger.LogInformation("Saving plan details in the db...........");
                await _agentCommissionApprovalRepository.SaveAggregateAsync(aggregate.Entity);
            }
            else
            {
                commandResult = new CommandResult(Guid.Empty, false);
                foreach (var msg in result.ValidationMessages)
                {
                    commandResult.AddResultMessage(msg.MessageType, msg.Code, msg.Message);
                }
            }

            return commandResult;
        }

        public async Task<AgentCommissionApproval> GetAgentCommissionApprovals(Guid agentId, string month, string year)
        {
            var search = new List<SearchParameter>()
            {
                new SearchParameter{Name="AGENTID", Value = agentId.ToString()},
                new SearchParameter{Name="CALENDARMONTH", Value = month},
                new SearchParameter{Name="CALENDARYEAR", Value = year},

            };

            var agentApprovalEntity = (await _agentCommissionApprovalRepository.FindAggregatesAsync(search)).FirstOrDefault();
            var agentApproval = _mapper.Map<AgentCommissionApprovalEntity, AgentCommissionApproval>(agentApprovalEntity);



            return agentApproval;
        }

        public async Task<IEnumerable<AgentEarnedCommission>> GetTotalAgentEarnedCommissions(DateTime? startdate, DateTime? endDate)
        {
            var earnedCommissions = await GetEarnedCommissions(startdate, endDate);
            var result = new List<AgentEarnedCommission>();

            //group commission by agent
            var agentCommissionGroups = earnedCommissions.GroupBy(s => s.AgentId);

            foreach (var group in agentCommissionGroups)
            {
                //calculate totals for each agent
                double total = 0;
                foreach (var agentEarnedCommission in group)
                {
                    total = (double)agentEarnedCommission.Amount + total;
                }
                result.Add(new AgentEarnedCommission { AgentId = group.Key, TotalAmount = total });
            }

            return result;
        }

        public async Task<IEnumerable<AgentCommissionApproval>> GetAgentCommissionApprovals(string month, string year)
        {
            var approvalEntities = await _agentCommissionApprovalRepository.GetMonthlyApprovals(year, month, null);
            var result = _mapper.Map<IEnumerable<AgentCommissionApprovalEntity>, IEnumerable<AgentCommissionApproval>>(approvalEntities);
            return result;
        }

        public async Task<CommandResult> ApproveAllCommissions(List<AgentCommissionApproval> approvals)
        {
            var result = new CommandResult(Guid.NewGuid(), true);

            //approve a list of approvals
            _logger.LogInformation("Aprroving commissions........");
            foreach (var approval in approvals)
            {
                approval.Status = Enum.GetName(typeof(CommisionApprovalStatus), CommisionApprovalStatus.AWAIT_PAYMENT);
                var response = await CreateApproval(approval);
                if (!response.Accepted)
                {
                    result = new CommandResult(Guid.NewGuid(), false);
                    foreach (var msg in response.Messages)
                    {
                        result.AddResultMessage(msg.MessageType, msg.Code, msg.Message);
                    }
                    break;
                }
            }


            return result;
        }

        public async Task<CommandResult> RecalculateAgentCommissions(Guid agentId, DateTime startDate, DateTime endDate)
        {
            _logger.LogInformation("Re-running commision schedule........");
            var commandResult = new CommandResult(Guid.NewGuid(), true);
            var commissions = await GetAgentUnpaidCommissions(agentId, startDate, endDate);

            var exsitingApproval = await _agentCommissionApprovalRepository.GetAgentApproval(agentId);
            if (exsitingApproval != null)
            {
                await RemoveApproval(exsitingApproval.Id);
            }


            foreach (var commission in commissions)
            {
                var commisionPlan = await _commisionPlanRepository.GetCommisionPlan(commission.CommissionPlanId);
                if (commisionPlan != null)
                {
                    //calculate commission
                    var percentageAmount = (commisionPlan.PercentageValue) / 100 * commission.Fee;
                    commission.Amount = percentageAmount + commisionPlan.FixedValue;
                    commission.FixedValue = commisionPlan.FixedValue;
                    commission.PercentageValue = commisionPlan.PercentageValue;

                }
                else
                {
                    commission.Amount = 0;
                    commission.FixedValue = 0;
                    commission.PercentageValue = 0;

                }

                //update records
                var updateResponse = await UpdateCalculatedCommission(commission);
                if (!updateResponse.Accepted)
                {
                    // do something
                }
            }
            return commandResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="agents"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<CommandResult> RescheduleAgentCommissions(List<Guid> agents, DateTime startDate, DateTime endDate)
        {
            var commandResult = new CommandResult(Guid.NewGuid(), true);

            foreach (var agentId in agents)
            {
                var response = await RecalculateAgentCommissions(agentId, startDate, endDate);
                if (!response.Accepted)
                {
                    commandResult = new CommandResult(Guid.NewGuid(), false);
                    break;
                }

            }
            return commandResult;
        }

        public async Task<CommandResult> RemoveApproval(Guid id)
        {
            var entity = await _agentCommissionApprovalRepository.LoadAggregateAsync(id);
            var commandResult = new CommandResult(id, true);
            var aggregate = new AgentCommissionApprovalAggregate(entity);
            aggregate.Delete();
            await _agentCommissionApprovalRepository.SaveAggregateAsync(aggregate.Entity);
            return commandResult;

        }

        private async Task<CommandResult> CreatePaymentFile()
        {
            var commandResult = new CommandResult(Guid.NewGuid(), true);
            return commandResult;
        }

        public async Task<CommandResult> PayAgent(Agent agent, string year, string month)
        {

            var commandResult = new CommandResult(Guid.NewGuid(), true);

            //get agent approval
            _logger.LogInformation("Load agent approval details........");
            var approvalEntity = (await _agentCommissionApprovalRepository.GetMonthlyApprovals(year, month, agent.Id)).FirstOrDefault();
            if(approvalEntity != null)
            {
                if(approvalEntity.Status == Enum.GetName(typeof(CommisionApprovalStatus), CommisionApprovalStatus.AWAIT_PAYMENT)){

                   var response = await CreatePaymentFile();
                    if (response.Accepted)
                    {
                        var approval = _mapper.Map<AgentCommissionApprovalEntity, AgentCommissionApproval>(approvalEntity);
                        await UpdateApproval(approval);
                    }
                    else
                    {
                         commandResult = new CommandResult(Guid.NewGuid(), false);
                    }
                }
            }

            return commandResult;
        }
    }
}
