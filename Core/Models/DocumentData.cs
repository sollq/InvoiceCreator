namespace Core.Models;

public class DocumentData
{
    public required ClientInfo Buyer;
    public string? ClientINN;
    public string? ContractNumber;
    public DateTime Date;
    public required string InvoiceNumber;
    public DocumentType OrgType;
    public required List<Product> Products;
    public required SupplierInfo Seller;
    public decimal TotalAmount;
    public string? TotalAmountText;
    public string? VatText;
    public string? ContractTitle { get; set; }
}