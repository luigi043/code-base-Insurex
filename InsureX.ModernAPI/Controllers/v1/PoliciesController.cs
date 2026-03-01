using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using InsureX.ModernAPI.Data;
using InsureX.ModernAPI.Models;

namespace InsureX.ModernAPI.Controllers.v1;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class PoliciesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PoliciesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult> GetPolicies()
    {
        try
        {
            var policies = await _context.Policies.ToListAsync();
            
            var result = policies.Select(p => new
            {
                p.Id,
                p.PolicyNumber,
                p.PolicyHolder,
                p.Email,
                p.StartDate,
                p.EndDate,
                p.Status,
                p.Premium,
                p.PolicyType,
                p.CreatedAt
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetPolicy(int id)
    {
        try
        {
            var policy = await _context.Policies.FindAsync(id);
            if (policy == null)
                return NotFound(new { message = "Apólice não encontrada" });

            return Ok(new
            {
                policy.Id,
                policy.PolicyNumber,
                policy.PolicyHolder,
                policy.Email,
                policy.StartDate,
                policy.EndDate,
                policy.Status,
                policy.Premium,
                policy.PolicyType,
                policy.CreatedAt
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult> CreatePolicy([FromBody] CreatePolicyRequest request)
    {
        try
        {
            var policy = new Policy
            {
                PolicyNumber = $"POL-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}",
                PolicyHolder = request.PolicyHolder,
                Email = request.Email,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Premium = request.Premium,
                PolicyType = request.PolicyType,
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            };

            _context.Policies.Add(policy);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPolicy), new { id = policy.Id }, new
            {
                policy.Id,
                policy.PolicyNumber,
                policy.PolicyHolder,
                policy.Email,
                policy.StartDate,
                policy.EndDate,
                policy.Status,
                policy.Premium,
                policy.PolicyType,
                policy.CreatedAt
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    public class CreatePolicyRequest
    {
        public string PolicyHolder { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Premium { get; set; }
        public string PolicyType { get; set; } = string.Empty;
    }
}
