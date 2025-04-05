using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
namespace Cleverence;
public static class CompressionManager
{
    public static string Compress(string str)
    {
        StringBuilder result = new();

        for ( int i = 0; i < str.Length; )
        {
            char c = str[i];
            if ( !char.IsLetter(c) )
                throw new FormatException();
            int count = Count(c, ref i, str);
            result.Append(c);
            if ( count > 1 )
                result.Append(count.ToString());

        }
        return result.ToString();
    }
    private static int Count(char c, ref int index, string str)
    {
        int count = 0;
        //если мы хотим чтобы считалось не a12 , а a9a3 в условие цикла нужно добавить &&count<9
        for ( ; index < str.Length && str[index] == c; index++, count++ ) ;
        return count;
    }
    private static bool LineIsCorrect(string line) => Regex.IsMatch(line, @"^[a-zA-Z0-9]+$");
    public static string DeCompress(string str)
    {
        StringBuilder result = new();
        for ( int i = 0; i < str.Length; )
        {
            char c = str[i];
            if ( !char.IsLetter(c) )
                throw new FormatException();
            int count = 1;
            if ( i < str.Length - 1 && char.IsDigit(str[i + 1]) )
            {
                i++;
                //если валидна только запись a9a3 , а не a12  , то в качестве count берём следующую за буквой цифру
                //и при следущей итерации цикла выбрасывается exception
                //здесь представлен вариант где валидна запись a12
                count = FindCount(str, ref i);
                for ( int j = count; j > 0; j-- )
                    result.Append(c);
            }
            else
                result.Append(str[i++]);
        }
        return result.ToString();
    }

    private static int FindCount(string str, ref int index)
    {
        StringBuilder result = new();
        for ( ; index < str.Length && char.IsDigit(str[index]); index++ )
            result.Append(str[index]);
        return int.Parse(result.ToString());
    }
}