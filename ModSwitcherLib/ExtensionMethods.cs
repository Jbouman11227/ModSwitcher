namespace ModSwitcherLib
{
    public static class ExtensionMethods
    {
        public static string AddPeriod(this string str)
        {
            return str + (str.EndsWith(".") ? string.Empty : ".");
        }
    }
}
