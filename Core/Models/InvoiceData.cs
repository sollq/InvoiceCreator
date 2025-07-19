namespace Core.Models;

public class InvoiceData
{
    public ClientInfo Buyer;
    public string ClientINN;
    public string ContractNumber;
    public DateTime Date;
    public string InvoiceNumber;
    public OrganizationType OrgType;
    public List<Product> Products;
    public SupplierInfo Seller;
    public string Subject;
    public decimal TotalAmount;
    public string TotalAmountText;
}