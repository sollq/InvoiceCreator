using Core.Interfaces;
using Humanizer;
using System.Globalization;
using Core.Models;
using Infrastructure.Services.Interfaces;

namespace Infrastructure.Services;

public class NumberToWordsConverter : INumberToWordsConverter
{
    public string Convert(decimal value, DocumentType inputType)
    {
        if (inputType == DocumentType.Kz || inputType == DocumentType.KzAkt)
        {
            return ConvertKz(value);
        }
        return $"{((long)value).ToWords(new CultureInfo("ru"))} рублей {((int)((value - (long)value) * 100)).ToWords(new CultureInfo("ru"))} копеек";
    }
    private static string ConvertKz(decimal value)
    {
        return $"{((long)value).ToWords(new CultureInfo("ru"))} тенге {((int)((value - (long)value) * 100)).ToWords(new CultureInfo("ru"))} тиын";
    }
}