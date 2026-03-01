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
public class BillingController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<BillingController> _logger;

    public BillingController(ApplicationDbContext context, ILogger<BillingController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/billing/invoices
    [HttpGet("invoices")]
    public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetInvoices(
        [FromQuery] string? status = null,
        [FromQuery] int? policyId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var query = _context.Invoices
            .Include(i => i.Policy)
            .Include(i => i.InvoiceItems)
            .Include(i => i.Payments)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(i => i.Status == status);

        if (policyId.HasValue)
            query = query.Where(i => i.PolicyId == policyId);

        if (fromDate.HasValue)
            query = query.Where(i => i.InvoiceDate >= fromDate);

        if (toDate.HasValue)
            query = query.Where(i => i.InvoiceDate <= toDate);

        var invoices = await query
            .OrderByDescending(i => i.CreatedAt)
            .Select(i => new InvoiceDto
            {
                Id = i.Id,
                InvoiceNumber = i.InvoiceNumber,
                PolicyId = i.PolicyId,
                PolicyNumber = i.Policy != null ? i.Policy.PolicyNumber : "",
                CustomerName = i.Policy != null ? i.Policy.PolicyHolder : "",
                InvoiceDate = i.InvoiceDate,
                DueDate = i.DueDate,
                Amount = i.Amount,
                Tax = i.Tax,
                TotalAmount = i.TotalAmount,
                Status = i.Status,
                PaymentMethod = i.PaymentMethod,
                PaymentDate = i.PaymentDate,
                PaymentStatus = GetPaymentStatus(i),
                PaidAmount = i.Payments.Where(p => p.Status == "Completed").Sum(p => p.Amount),
                ItemCount = i.InvoiceItems.Count
            })
            .ToListAsync();

        return Ok(invoices);
    }

    // GET: api/billing/invoices/5
    [HttpGet("invoices/{id}")]
    public async Task<ActionResult<InvoiceDetailDto>> GetInvoice(int id)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Policy)
            .Include(i => i.InvoiceItems)
            .Include(i => i.Payments)
                .ThenInclude(p => p.Creator)
            .Include(i => i.Creator)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice == null)
            return NotFound();

        var result = new InvoiceDetailDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            PolicyId = invoice.PolicyId,
            PolicyNumber = invoice.Policy?.PolicyNumber ?? "",
            CustomerName = invoice.Policy?.PolicyHolder ?? "",
            CustomerEmail = invoice.Policy?.Email ?? "",
            CustomerPhone = invoice.Policy?.Phone ?? "",
            InvoiceDate = invoice.InvoiceDate,
            DueDate = invoice.DueDate,
            Amount = invoice.Amount,
            Tax = invoice.Tax,
            TotalAmount = invoice.TotalAmount,
            Status = invoice.Status,
            PaymentMethod = invoice.PaymentMethod,
            PaymentDate = invoice.PaymentDate,
            Notes = invoice.Notes,
            PdfUrl = invoice.PdfUrl,
            CreatedAt = invoice.CreatedAt,
            CreatedByName = invoice.Creator?.Name ?? "System",
            Items = invoice.InvoiceItems.Select(item => new InvoiceItemDto
            {
                Id = item.Id,
                Description = item.Description,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Amount = item.Amount,
                ItemType = item.ItemType
            }).ToList(),
            Payments = invoice.Payments.Select(p => new PaymentSummaryDto
            {
                Id = p.Id,
                PaymentNumber = p.PaymentNumber,
                PaymentDate = p.PaymentDate,
                Amount = p.Amount,
                PaymentMethod = p.PaymentMethod,
                Status = p.Status,
                Reference = p.Reference,
                CreatedByName = p.Creator?.Name ?? "System"
            }).ToList()
        };

        return Ok(result);
    }

    // POST: api/billing/invoices
    [HttpPost("invoices")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<Invoice>> CreateInvoice(CreateInvoiceDto createDto)
    {
        var userId = GetCurrentUserId();
        
        // Generate invoice number
        var invoiceCount = await _context.Invoices.CountAsync() + 1;
        var invoiceNumber = $"INV-{DateTime.Now:yyyyMMdd}-{invoiceCount:D4}";

        // Calculate subtotal from items
        var subtotal = createDto.Items.Sum(item => item.Quantity * item.UnitPrice);

        var invoice = new Invoice
        {
            InvoiceNumber = invoiceNumber,
            PolicyId = createDto.PolicyId,
            InvoiceDate = createDto.InvoiceDate,
            DueDate = createDto.DueDate,
            Amount = subtotal,
            Tax = createDto.Tax,
            TotalAmount = subtotal + createDto.Tax,
            Status = "Draft",
            Notes = createDto.Notes,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        // Add invoice items
        foreach (var itemDto in createDto.Items)
        {
            var item = new InvoiceItem
            {
                InvoiceId = invoice.Id,
                Description = itemDto.Description,
                Quantity = itemDto.Quantity,
                UnitPrice = itemDto.UnitPrice,
                Amount = itemDto.Quantity * itemDto.UnitPrice,
                ItemType = itemDto.ItemType,
                ReferenceId = itemDto.ReferenceId,
                ReferenceType = itemDto.ReferenceType
            };
            _context.InvoiceItems.Add(item);
        }

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetInvoice), new { id = invoice.Id }, invoice);
    }

    // POST: api/billing/invoices/5/send
    [HttpPost("invoices/{id}/send")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> SendInvoice(int id)
    {
        var invoice = await _context.Invoices.FindAsync(id);
        if (invoice == null)
            return NotFound();

        invoice.Status = "Sent";
        invoice.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // TODO: Send email with PDF attachment
        _logger.LogInformation("Invoice {InvoiceNumber} marked as sent", invoice.InvoiceNumber);

        return Ok(new { message = "Invoice sent successfully" });
    }

    // POST: api/billing/invoices/5/payments
    [HttpPost("invoices/{id}/payments")]
    public async Task<ActionResult<Payment>> AddPayment(int id, AddPaymentDto paymentDto)
    {
        var userId = GetCurrentUserId();
        var invoice = await _context.Invoices
            .Include(i => i.Payments)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice == null)
            return NotFound();

        // Generate payment number
        var paymentCount = await _context.Payments.CountAsync() + 1;
        var paymentNumber = $"PAY-{DateTime.Now:yyyyMMdd}-{paymentCount:D4}";

        var payment = new Payment
        {
            PaymentNumber = paymentNumber,
            InvoiceId = id,
            PaymentDate = paymentDto.PaymentDate,
            Amount = paymentDto.Amount,
            PaymentMethod = paymentDto.PaymentMethod,
            Reference = paymentDto.Reference,
            Status = "Completed",
            TransactionId = paymentDto.TransactionId,
            Notes = paymentDto.Notes,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Payments.Add(payment);
        
        // Update invoice status if fully paid
        var totalPaid = invoice.Payments.Where(p => p.Status == "Completed").Sum(p => p.Amount) + paymentDto.Amount;
        if (totalPaid >= invoice.TotalAmount)
        {
            invoice.Status = "Paid";
            invoice.PaymentDate = paymentDto.PaymentDate;
            invoice.PaymentMethod = paymentDto.PaymentMethod;
        }
        else
        {
            invoice.Status = "Partial";
        }

        invoice.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(payment);
    }

    // GET: api/billing/dashboard
    [HttpGet("dashboard")]
    public async Task<ActionResult<BillingDashboardDto>> GetDashboard()
    {
        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1);
        var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

        // Current month stats
        var currentMonthInvoices = await _context.Invoices
            .Where(i => i.InvoiceDate >= startOfMonth && i.InvoiceDate <= endOfMonth)
            .ToListAsync();

        var currentMonthPayments = await _context.Payments
            .Where(p => p.PaymentDate >= startOfMonth && p.PaymentDate <= endOfMonth && p.Status == "Completed")
            .ToListAsync();

        // Overdue invoices
        var overdueInvoices = await _context.Invoices
            .Where(i => i.Status != "Paid" && i.DueDate < now)
            .CountAsync();

        var overdueAmount = await _context.Invoices
            .Where(i => i.Status != "Paid" && i.DueDate < now)
            .SumAsync(i => (decimal?)i.TotalAmount - i.Payments.Where(p => p.Status == "Completed").Sum(p => p.Amount)) ?? 0;

        // Upcoming invoices
        var upcomingInvoices = await _context.RecurringBillings
            .Where(r => r.IsActive && r.NextBillingDate <= now.AddDays(30))
            .CountAsync();

        var result = new BillingDashboardDto
        {
            CurrentMonthInvoiced = currentMonthInvoices.Sum(i => i.TotalAmount),
            CurrentMonthPaid = currentMonthPayments.Sum(p => p.Amount),
            OverdueInvoices = overdueInvoices,
            OverdueAmount = overdueAmount,
            UpcomingInvoices = upcomingInvoices,
            TotalOutstanding = await _context.Invoices
                .Where(i => i.Status != "Paid")
                .SumAsync(i => (decimal?)i.TotalAmount - i.Payments.Where(p => p.Status == "Completed").Sum(p => p.Amount)) ?? 0
        };

        return Ok(result);
    }

    // GET: api/billing/invoices/5/pdf
    [HttpGet("invoices/{id}/pdf")]
    public async Task<IActionResult> GetInvoicePdf(int id)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Policy)
            .Include(i => i.InvoiceItems)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice == null)
            return NotFound();

        // TODO: Generate PDF
        // For now, return a simple message
        return Ok(new { message = "PDF generation not implemented yet" });
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim != null && int.TryParse(userIdClaim, out int userId))
            return userId;
        return null;
    }

    private string GetPaymentStatus(Invoice invoice)
    {
        var paidAmount = invoice.Payments.Where(p => p.Status == "Completed").Sum(p => p.Amount);
        if (paidAmount == 0)
            return "Unpaid";
        if (paidAmount >= invoice.TotalAmount)
            return "Paid";
        return "Partial";
    }
}

// DTOs
public class InvoiceDto
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = "";
    public int PolicyId { get; set; }
    public string PolicyNumber { get; set; } = "";
    public string CustomerName { get; set; } = "";
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal Amount { get; set; }
    public decimal Tax { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "";
    public string? PaymentMethod { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string PaymentStatus { get; set; } = "";
    public decimal PaidAmount { get; set; }
    public int ItemCount { get; set; }
}

public class InvoiceDetailDto
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = "";
    public int PolicyId { get; set; }
    public string PolicyNumber { get; set; } = "";
    public string CustomerName { get; set; } = "";
    public string CustomerEmail { get; set; } = "";
    public string CustomerPhone { get; set; } = "";
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal Amount { get; set; }
    public decimal Tax { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "";
    public string? PaymentMethod { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string? Notes { get; set; }
    public string? PdfUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedByName { get; set; } = "";
    public List<InvoiceItemDto> Items { get; set; } = new();
    public List<PaymentSummaryDto> Payments { get; set; } = new();
}

public class InvoiceItemDto
{
    public int Id { get; set; }
    public string Description { get; set; } = "";
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Amount { get; set; }
    public string ItemType { get; set; } = "";
}

public class PaymentSummaryDto
{
    public int Id { get; set; }
    public string PaymentNumber { get; set; } = "";
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = "";
    public string Status { get; set; } = "";
    public string? Reference { get; set; }
    public string CreatedByName { get; set; } = "";
}

public class CreateInvoiceDto
{
    public int PolicyId { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal Tax { get; set; }
    public string? Notes { get; set; }
    public List<CreateInvoiceItemDto> Items { get; set; } = new();
}

public class CreateInvoiceItemDto
{
    public string Description { get; set; } = "";
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string ItemType { get; set; } = "";
    public int? ReferenceId { get; set; }
    public string? ReferenceType { get; set; }
}

public class AddPaymentDto
{
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = "";
    public string? Reference { get; set; }
    public string? TransactionId { get; set; }
    public string? Notes { get; set; }
}

public class BillingDashboardDto
{
    public decimal CurrentMonthInvoiced { get; set; }
    public decimal CurrentMonthPaid { get; set; }
    public int OverdueInvoices { get; set; }
    public decimal OverdueAmount { get; set; }
    public int UpcomingInvoices { get; set; }
    public decimal TotalOutstanding { get; set; }
}
