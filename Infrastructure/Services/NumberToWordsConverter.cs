using Core.Interfaces;

namespace Infrastructure.Services;

public class NumberToWordsConverter : INumberToWordsConverter
{
    public string Convert(decimal value)
    {
        // Для MVP: только целые суммы, только KZT/RUB, без копеек
        var intValue = (long)value;
        if (intValue == 0)
            return "ноль";
        return ToWords(intValue).Trim();
    }

    // Примитивная реализация для русского языка
    private static string ToWords(long number)
    {
        switch (number)
        {
            case 0:
                return "ноль";
            case < 0:
                return "минус " + ToWords(-number);
        }

        string[] units =
        [
            "", "один", "два", "три", "четыре", "пять", "шесть", "семь", "восемь", "девять", "десять", "одиннадцать",
            "двенадцать", "тринадцать", "четырнадцать", "пятнадцать", "шестнадцать", "семнадцать", "восемнадцать",
            "девятнадцать"
        ];
        string[] tens =
        [
            "", "", "двадцать", "тридцать", "сорок", "пятьдесят", "шестьдесят", "семьдесят", "восемьдесят", "девяносто"
        ];
        string[] hundreds =
            ["", "сто", "двести", "триста", "четыреста", "пятьсот", "шестьсот", "семьсот", "восемьсот", "девятьсот"];
        string[] thousands = ["", "тысяча", "миллион", "миллиард"];

        var parts = new List<string>();
        var thousandGroup = 0;
        while (number > 0)
        {
            var n = (int)(number % 1000);
            if (n != 0)
            {
                var group = new List<string>();
                if (n % 100 < 20)
                {
                    if (n % 100 != 0)
                        group.Add(units[n % 100]);
                }
                else
                {
                    if (n % 10 != 0)
                        group.Add(units[n % 10]);
                    if (n / 10 % 10 != 0)
                        group.Add(tens[n / 10 % 10]);
                }

                if (n / 100 % 10 != 0)
                    group.Add(hundreds[n / 100 % 10]);
                if (thousandGroup > 0)
                    group.Add(thousands[thousandGroup]);
                parts.Insert(0, string.Join(" ", group));
            }

            number /= 1000;
            thousandGroup++;
        }

        return string.Join(" ", parts);
    }
}