using Core.Models;

namespace Core.Interfaces;

public interface IInvoicePdfGeneratorFactory
{
    IInvoicePdfGenerator GetGenerator(OrganizationType type);
}