namespace Core.Models;

public class SupplierInfo
{
    public required string Address { get; set; }
    public string? BankAccount { get; set; }
    public required string BankDetails { get; set; }
    public string? BIK { get; set; }
    public string? INN { get; set; }
    public string? Kbe { get; set; }
    public required string Name { get; set; }
    public string? PaymentCode { get; set; }
    public string? KPP { get; set; }
    public string? OGRN { get; set; }
    public string? CoreAcc { get; set; }
    public string? BankCoreAcc { get; set; }
}