using Core.Models;

namespace Infrastructure.Services.Interfaces;

public interface INumberToWordsConverter
{
    string Convert(decimal value, DocumentType inputType);
}