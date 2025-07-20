using Core.Models;

namespace Core.Interfaces;

public interface IMyCompanyInfoProvider
{
    SupplierInfo GetInfo(DocumentType type);
}