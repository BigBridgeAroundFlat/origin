/// <summary>
/// StringExtensions
/// </summary>
public static class StringExtensions
{
    public static bool IsEmpty(this string source)
    {
        if (string.IsNullOrEmpty(source))
        {
            return true;
        }
        return false;
    }
}