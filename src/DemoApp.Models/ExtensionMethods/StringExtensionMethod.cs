using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DemoApp.Models.ExtensionMethods
{
    public static class StringExtensionMethod
    {
        public static List<string> ToStringList(this string message)
        {
            return new List<string> { message };
        }

        public static string ToCamelCase(this string text)
        {
            return text[0].ToString().ToLower() + text.Substring(1);
        }

        public static string GenerateSlug(this string phrase)
        {
            string str = phrase.RemoveAccent().ToLower();
            // invalid chars
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // convert multiple spaces into one space
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // cut and trim
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
            str = Regex.Replace(str, @"\s", "-"); // hyphens
            return str;
        }

        public static string RemoveAccent(this string txt)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }
    }
}
