namespace LoxSharp.Helpers
{
    internal static class StringHelpers
    {
        public static bool IsDigit(this char c)
        {
            return c >= '0' && c <= '9';
        }

        public static bool IsAlpha(this char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';
        }

        public static bool IsAlphaNumeric(this char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }
    }
}
