namespace Core.Models;

public class Input
{
    public DocumentType Type { get; set; }
    public string CompanyINN { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string CompanyAddress { get; set; } = string.Empty;
    public string ContractNumber { get; set; } = string.Empty;
    public string InvoiceNumber { get; set; } = string.Empty;
    public string CompanyKPP { get; set; } = string.Empty;
    public DateTime ContractDate { get; set; }
    public List<Product> Products { get; set; } = [];
}