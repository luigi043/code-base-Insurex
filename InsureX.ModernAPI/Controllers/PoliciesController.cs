using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InsureX.ModernAPI.Data;
using InsureX.ModernAPI.Models;

namespace InsureX.ModernAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PoliciesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PoliciesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/policies
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Policy>>> GetPolicies()
    {
        return await _context.Policies
            .Where(p => !p.IsDeleted)
            .Include(p => p.Assets)
            .Include(p => p.Claims)
            .Include(p => p.Transactions)
            .ToListAsync();
    }

    // GET: api/policies/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Policy>> GetPolicy(int id)
    {
        var policy = await _context.Policies
            .Include(p => p.Assets)
            .Include(p => p.Claims)
            .Include(p => p.Transactions)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (policy == null)
        {
            return NotFound();
        }

        return policy;
    }

    // POST: api/policies
    [HttpPost]
    public async Task<ActionResult<Policy>> CreatePolicy(Policy policy)
    {
        policy.CreatedAt = DateTime.UtcNow;
        policy.PolicyNumber = GeneratePolicyNumber();
        
        _context.Policies.Add(policy);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPolicy), new { id = policy.Id }, policy);
    }

    // PUT: api/policies/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePolicy(int id, Policy policy)
    {
        if (id != policy.Id)
        {
            return BadRequest();
        }

        policy.UpdatedAt = DateTime.UtcNow;
        _context.Entry(policy).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PolicyExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/policies/5 (soft delete)
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePolicy(int id)
    {
        var policy = await _context.Policies.FindAsync(id);
        if (policy == null)
        {
            return NotFound();
        }

        policy.IsDeleted = true;
        policy.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET: api/policies/5/assets
    [HttpGet("{id}/assets")]
    public async Task<ActionResult<IEnumerable<Asset>>> GetPolicyAssets(int id)
    {
        return await _context.Assets
            .Where(a => a.PolicyId == id && !a.IsDeleted)
            .ToListAsync();
    }

    // GET: api/policies/5/claims
    [HttpGet("{id}/claims")]
    public async Task<ActionResult<IEnumerable<InsuranceClaim>>> GetPolicyClaims(int id)
    {
        return await _context.Claims
            .Where(c => c.PolicyId == id)
            .ToListAsync();
    }

    // GET: api/policies/5/transactions
    [HttpGet("{id}/transactions")]
    public async Task<ActionResult<IEnumerable<Transaction>>> GetPolicyTransactions(int id)
    {
        return await _context.Transactions
            .Where(t => t.PolicyId == id)
            .ToListAsync();
    }

    private bool PolicyExists(int id)
    {
        return _context.Policies.Any(e => e.Id == id && !e.IsDeleted);
    }

    private string GeneratePolicyNumber()
    {
        return $"POL-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}";
    }
}