namespace Igbadun
{
    public static class StringExtensions
    {
        public static string Substr(this string input, int beginIndex, int endIndex)
        {
            int len = endIndex - beginIndex;
            return input.Substring(beginIndex, len);
        }
    }
}