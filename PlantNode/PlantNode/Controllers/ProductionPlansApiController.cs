using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlantNode.Application.Interfaces;
using PlantNode.Application.DTOs.ProductionPlans;
using PlantNode.Domain.Enums;

namespace PlantNode.Controllers
{
    [ApiController]
    [Route("api/production-plans")]
    public class ProductionPlansApiController : ControllerBase
    {
        private readonly IProductionPlanService _productionPlanService;
        private readonly IPlanEventService _planEventService;

        public ProductionPlansApiController(IProductionPlanService productionPlanService, IPlanEventService planEventService)
        {
            _productionPlanService = productionPlanService;
            _planEventService = planEventService;
        }

        /// <summary>
        /// Gets all production plans (excluding demands).
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductionPlanDto>>> GetAllPlans()
        {
            var plans = await _productionPlanService.GetAllProductionPlansAsync();
            var dtos = plans
                .Where(p => p.Status != ProductionPlanStatus.Demand)
                .Select(p => p.ToDto())
                .ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Gets production plans filtered by status.
        /// </summary>
        [HttpGet("status")]
        public async Task<ActionResult<IEnumerable<ProductionPlanDto>>> GetPlansByStatus([FromQuery] ProductionPlanStatus status)
        {
            var plans = await _productionPlanService.GetAllProductionPlansAsync();
            var dtos = plans
                .Where(p => p.Status == status)
                .Select(p => p.ToDto())
                .ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Gets a specific production plan by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductionPlanDto>> GetPlanById(int id)
        {
            var plan = await _productionPlanService.GetProductionPlanByIdAsync(id);
            if (plan == null || plan.Status == ProductionPlanStatus.Demand)
            {
                return NotFound($"ID'si {id} olan plan bulunamadı.");
            }

            return Ok(plan.ToDto());
        }

        /// <summary>
        /// Updates the status of a specific production plan.
        /// </summary>
        [HttpPut("{id}/status")]
        public async Task<ActionResult<ProductionPlanDto>> UpdatePlanStatus(int id, [FromBody] UpdateProductionPlanStatusDto request)
        {
            if (request == null)
            {
                return BadRequest("Geçersiz istek gövdesi.");
            }

            var plan = await _productionPlanService.GetProductionPlanByIdAsync(id);
            if (plan == null || plan.Status == ProductionPlanStatus.Demand)
            {
                return NotFound($"ID'si {id} olan plan bulunamadı.");
            }

            // Perform status transition update
            plan.Status = request.Status;
            
            try
            {
                // Save changes which also executes business logic calculations and updates machine logs
                await _productionPlanService.SaveProductionPlanAsync(plan);

                // Notify all active Blazor circuits about the status change
                _planEventService.NotifyPlanStatusChanged(id);
                
                // Fetch the updated plan to return fresh mapped values
                var updatedPlan = await _productionPlanService.GetProductionPlanByIdAsync(id);
                return Ok(updatedPlan!.ToDto());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Plan durumu güncellenirken hata oluştu: {ex.Message}");
            }
        }
    }
}
