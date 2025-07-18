namespace Core.Models
{
    public class InvoiceData
    {
        public OrganizationType OrgType;
        public string InvoiceNumber;
        public string ContractNumber;
        public DateTime Date;
        public string ClientINN;
        public List<Product> Products;
        public string Subject;
        public decimal TotalAmount;
        public string TotalAmountText;
        public ClientInfo Buyer;
        public SupplierInfo Seller;
    }
}
