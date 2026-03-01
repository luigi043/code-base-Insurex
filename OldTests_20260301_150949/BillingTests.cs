using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace InsureX.IntegrationTests;

public class BillingTests : BaseTest
{
    public BillingTests(TestWebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetBillingDashboard_ReturnsSuccess()
    {
        // Act
        var response = await _client.GetAsync("/api/billing/dashboard");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var dashboard = await response.Content.ReadFromJsonAsync<BillingDashboardDto>();
        Assert.NotNull(dashboard);
    }

    [Fact]
    public async Task CreateInvoice_ReturnsSuccess()
    {
        // Arrange
        var newInvoice = new
        {
            policyId = _testPolicyId,
            invoiceDate = DateTime.UtcNow,
            dueDate = DateTime.UtcNow.AddDays(30),
            tax = 150.00M,
            notes = "Test invoice from integration test",
            items = new[]
            {
                new
                {
                    description = "Monthly Premium",
                    quantity = 1,
                    unitPrice = 1000.00M,
                    itemType = "Premium"
                },
                new
                {
                    description = "Administration Fee",
                    quantity = 1,
                    unitPrice = 50.00M,
                    itemType = "Fee"
                }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/billing/invoices", newInvoice);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var invoice = await response.Content.ReadFromJsonAsync<Invoice>();
        Assert.NotNull(invoice);
        Assert.Equal(_testPolicyId, invoice.PolicyId);
    }

    [Fact]
    public async Task GetInvoices_ReturnsList()
    {
        // Act
        var response = await _client.GetAsync("/api/billing/invoices");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var invoices = await response.Content.ReadFromJsonAsync<List<InvoiceDto>>();
        Assert.NotNull(invoices);
    }

    [Fact]
    public async Task CreateAndGetInvoice_ReturnsCorrectInvoice()
    {
        // Create invoice
        var newInvoice = new
        {
            policyId = _testPolicyId,
            invoiceDate = DateTime.UtcNow,
            dueDate = DateTime.UtcNow.AddDays(30),
            tax = 150.00M,
            notes = "Test invoice for get test",
            items = new[]
            {
                new
                {
                    description = "Monthly Premium",
                    quantity = 1,
                    unitPrice = 1000.00M,
                    itemType = "Premium"
                }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/billing/invoices", newInvoice);
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<Invoice>();
        
        // Get invoice by ID
        var getResponse = await _client.GetAsync($"/api/billing/invoices/{created.Id}");
        getResponse.EnsureSuccessStatusCode();
        
        var retrieved = await getResponse.Content.ReadFromJsonAsync<InvoiceDetailDto>();
        Assert.NotNull(retrieved);
        Assert.Equal(created.Id, retrieved.Id);
        Assert.Equal("Test invoice for get test", retrieved.Notes);
        Assert.Single(retrieved.Items);
    }

    [Fact]
    public async Task AddPaymentToInvoice_UpdatesStatus()
    {
        // Create invoice
        var newInvoice = new
        {
            policyId = _testPolicyId,
            invoiceDate = DateTime.UtcNow,
            dueDate = DateTime.UtcNow.AddDays(30),
            tax = 0,
            items = new[]
            {
                new
                {
                    description = "Test Item",
                    quantity = 1,
                    unitPrice = 500.00M,
                    itemType = "Premium"
                }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/billing/invoices", newInvoice);
        createResponse.EnsureSuccessStatusCode();
        var invoice = await createResponse.Content.ReadFromJsonAsync<Invoice>();

        // Add payment
        var payment = new
        {
            paymentDate = DateTime.UtcNow,
            amount = 500.00M,
            paymentMethod = "Bank Transfer",
            reference = "TEST-REF-123",
            notes = "Test payment"
        };

        var paymentResponse = await _client.PostAsJsonAsync($"/api/billing/invoices/{invoice.Id}/payments", payment);
        paymentResponse.EnsureSuccessStatusCode();

        // Get updated invoice
        var getResponse = await _client.GetAsync($"/api/billing/invoices/{invoice.Id}");
        getResponse.EnsureSuccessStatusCode();
        
        var updated = await getResponse.Content.ReadFromJsonAsync<InvoiceDetailDto>();
        Assert.NotNull(updated);
        Assert.Equal("Paid", updated.Status);
        Assert.Single(updated.Payments);
        Assert.Equal(500.00M, updated.Payments[0].Amount);
    }

    [Fact]
    public async Task SendInvoice_UpdatesStatus()
    {
        // Create invoice
        var newInvoice = new
        {
            policyId = _testPolicyId,
            invoiceDate = DateTime.UtcNow,
            dueDate = DateTime.UtcNow.AddDays(30),
            tax = 0,
            items = new[]
            {
                new
                {
                    description = "Test Item",
                    quantity = 1,
                    unitPrice = 500.00M,
                    itemType = "Premium"
                }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/billing/invoices", newInvoice);
        createResponse.EnsureSuccessStatusCode();
        var invoice = await createResponse.Content.ReadFromJsonAsync<Invoice>();

        // Send invoice
        var sendResponse = await _client.PostAsync($"/api/billing/invoices/{invoice.Id}/send", null);
        sendResponse.EnsureSuccessStatusCode();

        // Get updated invoice
        var getResponse = await _client.GetAsync($"/api/billing/invoices/{invoice.Id}");
        getResponse.EnsureSuccessStatusCode();
        
        var updated = await getResponse.Content.ReadFromJsonAsync<InvoiceDetailDto>();
        Assert.NotNull(updated);
        Assert.Equal("Sent", updated.Status);
    }

    // DTO classes
    private class Invoice
    {
        public int Id { get; set; }
        public int PolicyId { get; set; }
        public string InvoiceNumber { get; set; } = "";
    }

    private class InvoiceDto
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = "";
        public string Status { get; set; } = "";
    }

    private class InvoiceDetailDto
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = "";
        public string Status { get; set; } = "";
        public string? Notes { get; set; }
        public List<InvoiceItemDto> Items { get; set; } = new();
        public List<PaymentSummaryDto> Payments { get; set; } = new();
    }

    private class InvoiceItemDto
    {
        public int Id { get; set; }
        public string Description { get; set; } = "";
        public decimal Amount { get; set; }
    }

    private class PaymentSummaryDto
    {
        public int Id { get; set; }
        public string PaymentNumber { get; set; } = "";
        public decimal Amount { get; set; }
        public string Status { get; set; } = "";
    }

    private class BillingDashboardDto
    {
        public decimal CurrentMonthInvoiced { get; set; }
        public decimal CurrentMonthPaid { get; set; }
        public int OverdueInvoices { get; set; }
        public decimal OverdueAmount { get; set; }
        public int UpcomingInvoices { get; set; }
        public decimal TotalOutstanding { get; set; }
    }
}
