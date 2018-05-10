namespace Common.Other
{
    public class StringWidthCheck
    {
        public static string GetStringWidthLimit(string content, int limit)
        {
            int halfWidth = CountHalfWidthString(content);
            int fullWidth = (content.Length - halfWidth) * 2;
            if (fullWidth + halfWidth <= limit)
                return content;

            // Or else
            string substring = content.Substring(0, content.Length - 1);
            return GetStringWidthLimit(substring, limit);
        }

        private static int CountHalfWidthString(string content)
        {
            int count = 0;
            foreach (var c in content)
            {
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9'))
                    count++;
            }
            return count;
        }
    }
}