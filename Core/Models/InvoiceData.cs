namespace Core.Models;

public class InvoiceData
{
    public required ClientInfo Buyer;
    public string? ClientINN;
    public string? ContractNumber;
    public DateTime Date;
    public required string InvoiceNumber;
    public OrganizationType OrgType;
    public required List<Product> Products;
    public required SupplierInfo Seller;
    public string? Subject;
    public decimal TotalAmount;
    public string? TotalAmountText;
}