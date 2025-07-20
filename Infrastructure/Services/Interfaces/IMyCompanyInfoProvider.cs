using Core.Models;

namespace Infrastructure.Services.Interfaces;

public interface IMyCompanyInfoProvider
{
    SupplierInfo GetInfo(DocumentType type);
}