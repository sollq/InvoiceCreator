using Core.Models;

namespace Core.Interfaces;

public interface INumberGeneratorService
{
    string GetNextInvoiceNumber(OrganizationType org);
    string GetNextContractNumber(OrganizationType org);
}