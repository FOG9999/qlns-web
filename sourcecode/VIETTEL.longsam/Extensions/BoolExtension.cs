namespace System
{
    public static class BoolExtension
    {
        public static string ToStringBool(this bool value)
        {
            return value ? "1" : "0";
        }
    }
}
