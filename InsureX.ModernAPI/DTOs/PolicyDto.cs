namespace InsureX.ModernAPI.DTOs;

public class PolicyDto
{
    public int Id { get; set; }
    public string PolicyNumber { get; set; } = string.Empty;
    public string PolicyHolder { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Premium { get; set; }
    public string PolicyType { get; set; } = string.Empty;
    public int? PartnerId { get; set; }
    public string? Notes { get; set; }
    public int AssetCount { get; set; }
    public decimal TotalInsuredValue { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreatePolicyDto
{
    public string PolicyHolder { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Premium { get; set; }
    public string PolicyType { get; set; } = string.Empty;
    public int? PartnerId { get; set; }
    public string? Notes { get; set; }
}
