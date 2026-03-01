using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using InsureX.ModernAPI.Data;
using InsureX.ModernAPI.Models;
using System.Text.Json;

namespace InsureX.ModernAPI.Controllers.v1;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class AssetsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AssetsController> _logger;

    public AssetsController(ApplicationDbContext context, ILogger<AssetsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult> GetAssets(
        [FromQuery] int? policyId = null,
        [FromQuery] string? assetType = null,
        [FromQuery] string? status = null)
    {
        try
        {
            var query = _context.Assets
                .Include(a => a.Policy)
                .AsQueryable();

            if (policyId.HasValue)
                query = query.Where(a => a.PolicyId == policyId.Value);

            if (!string.IsNullOrEmpty(assetType))
                query = query.Where(a => a.AssetType == assetType);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(a => a.Status == status);

            var assets = await query
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            var result = assets.Select(a => new
            {
                a.Id,
                a.AssetType,
                a.Description,
                PolicyId = a.PolicyId,
                PolicyNumber = a.Policy?.PolicyNumber ?? "",
                a.FinanceValue,
                a.InsuredValue,
                a.Status,
                Details = JsonSerializer.Deserialize<Dictionary<string, object>>(a.JsonData) ?? new Dictionary<string, object>(),
                a.CreatedAt
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting assets");
            return StatusCode(500, new { message = "Erro interno ao buscar ativos", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetAsset(int id)
    {
        try
        {
            var asset = await _context.Assets
                .Include(a => a.Policy)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (asset == null)
                return NotFound(new { message = "Ativo não encontrado" });

            var result = new
            {
                asset.Id,
                asset.AssetType,
                asset.Description,
                PolicyId = asset.PolicyId,
                PolicyNumber = asset.Policy?.PolicyNumber ?? "",
                asset.FinanceValue,
                asset.InsuredValue,
                asset.Status,
                Details = JsonSerializer.Deserialize<Dictionary<string, object>>(asset.JsonData) ?? new Dictionary<string, object>(),
                asset.CreatedAt
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting asset {Id}", id);
            return StatusCode(500, new { message = "Erro interno ao buscar ativo" });
        }
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsset([FromBody] CreateAssetRequest request)
    {
        try
        {
            var policy = await _context.Policies.FindAsync(request.PolicyId);
            if (policy == null)
                return BadRequest(new { message = "Apólice não encontrada" });

            var asset = new Asset
            {
                AssetType = request.AssetType,
                Description = request.Description,
                PolicyId = request.PolicyId,
                FinanceValue = request.FinanceValue,
                InsuredValue = request.InsuredValue,
                Status = "Active",
                JsonData = JsonSerializer.Serialize(request.Details ?? new Dictionary<string, object>()),
                CreatedAt = DateTime.UtcNow
            };

            _context.Assets.Add(asset);
            await _context.SaveChangesAsync();

            var result = new
            {
                asset.Id,
                asset.AssetType,
                asset.Description,
                PolicyId = asset.PolicyId,
                PolicyNumber = policy.PolicyNumber,
                asset.FinanceValue,
                asset.InsuredValue,
                asset.Status,
                Details = request.Details ?? new Dictionary<string, object>(),
                asset.CreatedAt
            };

            return CreatedAtAction(nameof(GetAsset), new { id = asset.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating asset");
            return StatusCode(500, new { message = "Erro interno ao criar ativo" });
        }
    }

    public class CreateAssetRequest
    {
        public string AssetType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int PolicyId { get; set; }
        public decimal FinanceValue { get; set; }
        public decimal InsuredValue { get; set; }
        public Dictionary<string, object>? Details { get; set; }
    }
}
