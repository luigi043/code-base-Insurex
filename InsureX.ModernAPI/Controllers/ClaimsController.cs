using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InsureX.ModernAPI.Data;
using InsureX.ModernAPI.Models;
using System.Security.Claims;

namespace InsureX.ModernAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ClaimsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ClaimsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/claims
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InsuranceClaimDto>>> GetClaims()
    {
        var claims = await _context.Claims
            .Include(c => c.Policy)
            .Include(c => c.Asset)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new InsuranceClaimDto
            {
                Id = c.Id,
                ClaimNumber = c.ClaimNumber,
                PolicyId = c.PolicyId,
                PolicyNumber = c.Policy.PolicyNumber,
                AssetId = c.AssetId,
                AssetDescription = c.Asset != null ? c.Asset.Description : null,
                ClaimDate = c.ClaimDate,
                Description = c.Description,
                ClaimAmount = c.ClaimAmount,
                ApprovedAmount = c.ApprovedAmount,
                Status = c.Status,
                Notes = c.Notes,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();

        return Ok(claims);
    }

    // GET: api/claims/5
    [HttpGet("{id}")]
    public async Task<ActionResult<InsuranceClaimDto>> GetClaim(int id)
    {
        var claim = await _context.Claims
            .Include(c => c.Policy)
            .Include(c => c.Asset)
            .Where(c => c.Id == id)
            .Select(c => new InsuranceClaimDto
            {
                Id = c.Id,
                ClaimNumber = c.ClaimNumber,
                PolicyId = c.PolicyId,
                PolicyNumber = c.Policy.PolicyNumber,
                AssetId = c.AssetId,
                AssetDescription = c.Asset != null ? c.Asset.Description : null,
                ClaimDate = c.ClaimDate,
                Description = c.Description,
                ClaimAmount = c.ClaimAmount,
                ApprovedAmount = c.ApprovedAmount,
                Status = c.Status,
                Notes = c.Notes,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (claim == null)
        {
            return NotFound();
        }

        return Ok(claim);
    }

    // GET: api/claims/policy/5
    [HttpGet("policy/{policyId}")]
    public async Task<ActionResult<IEnumerable<InsuranceClaimDto>>> GetClaimsByPolicy(int policyId)
    {
        var claims = await _context.Claims
            .Include(c => c.Policy)
            .Include(c => c.Asset)
            .Where(c => c.PolicyId == policyId)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new InsuranceClaimDto
            {
                Id = c.Id,
                ClaimNumber = c.ClaimNumber,
                PolicyId = c.PolicyId,
                PolicyNumber = c.Policy.PolicyNumber,
                AssetId = c.AssetId,
                AssetDescription = c.Asset != null ? c.Asset.Description : null,
                ClaimDate = c.ClaimDate,
                Description = c.Description,
                ClaimAmount = c.ClaimAmount,
                ApprovedAmount = c.ApprovedAmount,
                Status = c.Status,
                Notes = c.Notes,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();

        return Ok(claims);
    }

    // POST: api/claims
    [HttpPost]
    public async Task<ActionResult<InsuranceClaim>> CreateClaim(CreateClaimDto createDto)
    {
        // Generate claim number
        var claimCount = await _context.Claims.CountAsync() + 1;
        var claimNumber = $"CLM-{DateTime.Now:yyyyMMdd}-{claimCount:D4}";

        var claim = new InsuranceClaim
        {
            ClaimNumber = claimNumber,
            PolicyId = createDto.PolicyId,
            AssetId = createDto.AssetId,
            ClaimDate = createDto.ClaimDate,
            Description = createDto.Description,
            ClaimAmount = createDto.ClaimAmount,
            Status = "Submitted",
            Notes = createDto.Notes,
            CreatedAt = DateTime.UtcNow
        };

        _context.Claims.Add(claim);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetClaim), new { id = claim.Id }, claim);
    }

    // PUT: api/claims/5/approve
    [HttpPut("{id}/approve")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> ApproveClaim(int id, ApproveClaimDto approveDto)
    {
        var claim = await _context.Claims.FindAsync(id);
        if (claim == null)
        {
            return NotFound();
        }

        claim.Status = "Approved";
        claim.ApprovedAmount = approveDto.ApprovedAmount;
        claim.Notes = approveDto.Notes;
        claim.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // PUT: api/claims/5/reject
    [HttpPut("{id}/reject")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> RejectClaim(int id, RejectClaimDto rejectDto)
    {
        var claim = await _context.Claims.FindAsync(id);
        if (claim == null)
        {
            return NotFound();
        }

        claim.Status = "Rejected";
        claim.Notes = rejectDto.Reason;
        claim.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // PUT: api/claims/5/pay
    [HttpPut("{id}/pay")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PayClaim(int id)
    {
        var claim = await _context.Claims.FindAsync(id);
        if (claim == null)
        {
            return NotFound();
        }

        claim.Status = "Paid";
        claim.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/claims/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteClaim(int id)
    {
        var claim = await _context.Claims.FindAsync(id);
        if (claim == null)
        {
            return NotFound();
        }

        _context.Claims.Remove(claim);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

// DTOs
public class InsuranceClaimDto
{
    public int Id { get; set; }
    public string ClaimNumber { get; set; } = "";
    public int PolicyId { get; set; }
    public string PolicyNumber { get; set; } = "";
    public int? AssetId { get; set; }
    public string? AssetDescription { get; set; }
    public DateTime ClaimDate { get; set; }
    public string Description { get; set; } = "";
    public decimal ClaimAmount { get; set; }
    public decimal? ApprovedAmount { get; set; }
    public string Status { get; set; } = "";
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateClaimDto
{
    public int PolicyId { get; set; }
    public int? AssetId { get; set; }
    public DateTime ClaimDate { get; set; }
    public string Description { get; set; } = "";
    public decimal ClaimAmount { get; set; }
    public string? Notes { get; set; }
}

public class ApproveClaimDto
{
    public decimal ApprovedAmount { get; set; }
    public string? Notes { get; set; }
}

public class RejectClaimDto
{
    public string Reason { get; set; } = "";
}
