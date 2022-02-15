namespace Questionnaire.Common.Extensions;

public static class YearsExtension
{
    public static string GetYears(this int year)
    {
        if (year > 19 || year < 10)
        {
            var last = year % 10;
            if (last == 1) return $"{year} год"; 
            if (last == 0 || last >= 5) return $"{year} лет";
            return $"{year} года";
        }
        return $"{year} лет";
    }
}