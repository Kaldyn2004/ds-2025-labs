using System.Text.RegularExpressions;

namespace StringLib;

public static class TextUtil
{
    public static List<string> SplitIntoWords(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return [];
        }

        // Регулярное выражение для поиска слов:
        // - Слово начинается и заканчивается на букву.
        // - Может содержать апострофы и дефисы внутри.
        // - Не содержит чисел или знаков препинания.
        const string pattern = @"\p{L}+(?:[\-\']\p{L}+)*";
        Regex regex = new(pattern, RegexOptions.Compiled);

        return regex.Matches(text)
            .Select(match => match.Value)
            .ToList();
    }

    public static int CountVowels(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return 0;
        }

        // Регулярное выражение для подсчета согласных букв:
        const string pattern = @"[бвгджзйклмнпрстфхцчшщbcdfghjklmnpqrstvwxz]";
        Regex regex = new(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

        return regex.Matches(text).Count;
    }
}