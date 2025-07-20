using Core.Models;

namespace Infrastructure.Pdf.Interfaces;

public interface IPdfGenerator
{
    byte[] Generate(DocumentData data);

    bool CanHandle(DocumentType type);
}