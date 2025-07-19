using System;
using System.Collections.Generic;

namespace Core.Models;

public class InvoiceInput
{
    public OrganizationType OrgType { get; set; }
    public string CompanyINN { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string CompanyAddress { get; set; } = string.Empty;
    public string ContractNumber { get; set; } = string.Empty;
    public DateTime ContractDate { get; set; }
    public List<Product> Products { get; set; } = new();
    // Можно добавить другие поля по необходимости
}