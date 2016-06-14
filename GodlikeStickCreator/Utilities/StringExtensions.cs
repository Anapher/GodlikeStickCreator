using System;
using System.Text;

namespace GodlikeStickCreator.Utilities
{
    public static class StringExtensions
    {
        //Source: https://stackoverflow.com/questions/244531/is-there-an-alternative-to-string-replace-that-is-case-insensitive
        public static string Replace(this string str, string oldValue, string newValue, StringComparison comparison)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            // skip the loop entirely if oldValue and newValue are the same
            if (string.Compare(oldValue, newValue, comparison) == 0) return str;

            if (oldValue.Length > str.Length)
                return str;

            var sb = new StringBuilder();

            int previousIndex = 0;
            int index = str.IndexOf(oldValue, comparison);

            while (index != -1)
            {
                sb.Append(str.Substring(previousIndex, index - previousIndex));
                sb.Append(newValue);
                index += oldValue.Length;

                previousIndex = index;
                index = str.IndexOf(oldValue, index, comparison);
            }
            sb.Append(str.Substring(previousIndex));

            return sb.ToString();
        }
    }
}