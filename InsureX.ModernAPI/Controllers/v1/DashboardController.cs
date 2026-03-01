using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using InsureX.ModernAPI.Data;

namespace InsureX.ModernAPI.Controllers.v1;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(ApplicationDbContext context, ILogger<DashboardController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("stats")]
    public async Task<ActionResult> GetStats()
    {
        try
        {
            var totalPolicies = await _context.Policies.CountAsync();
            var activePolicies = await _context.Policies.CountAsync(p => p.Status == "Active");
            var totalAssets = await _context.Assets.CountAsync(a => !a.IsDeleted);
            var totalInsuredValue = await _context.Assets.Where(a => !a.IsDeleted).SumAsync(a => a.InsuredValue);
            
            var expiringSoon = await _context.Policies
                .CountAsync(p => p.Status == "Active" && p.EndDate <= DateTime.UtcNow.AddDays(30));

            var recentPolicies = await _context.Policies
                .OrderByDescending(p => p.CreatedAt)
                .Take(5)
                .Select(p => new
                {
                    p.Id,
                    p.PolicyNumber,
                    p.PolicyHolder,
                    p.Status,
                    p.CreatedAt
                })
                .ToListAsync();

            var stats = new
            {
                totalPolicies,
                activePolicies,
                totalAssets,
                totalInsuredValue,
                expiringSoon,
                recentPolicies
            };

            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard stats");
            return StatusCode(500, new { message = "Erro interno ao buscar estatísticas" });
        }
    }

    [HttpGet("policies-by-status")]
    public async Task<ActionResult> GetPoliciesByStatus()
    {
        try
        {
            var stats = await _context.Policies
                .GroupBy(p => p.Status)
                .Select(g => new { status = g.Key, count = g.Count() })
                .ToListAsync();

            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting policies by status");
            return StatusCode(500, new { message = "Erro interno ao buscar estatísticas" });
        }
    }

    [HttpGet("assets-by-type")]
    public async Task<ActionResult> GetAssetsByType()
    {
        try
        {
            var stats = await _context.Assets
                .Where(a => !a.IsDeleted)
                .GroupBy(a => a.AssetType)
                .Select(g => new { type = g.Key, count = g.Count() })
                .ToListAsync();

            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting assets by type");
            return StatusCode(500, new { message = "Erro interno ao buscar estatísticas" });
        }
    }
}
