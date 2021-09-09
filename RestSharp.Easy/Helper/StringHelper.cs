using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RestSharp.Easy.Helper
{
    public static class StringHelper
    {
        public static string ToSnakeCase(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            text = text.ToCamelCase();
            text = string.Concat(text.Select((_char, i) => i > 0 && char.IsUpper(_char) ? $"_{_char.ToString()}" : _char.ToString())).ToLower();

            return text;
        }

        public static string ToCamelCase(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            var finalValue = "";
            var upperNext = false;
            for (int i = 0; i < text.Length - 1; i++)
            {
                finalValue += (upperNext) ? char.ToUpperInvariant(text[i]) : text[i];
                upperNext = text[i] == '_';
            }

            finalValue += text[text.Length - 1].ToString();

            finalValue = finalValue.Replace("_", "");
            if (finalValue.Length == 0)
            {
                return null;
            }

            finalValue = Regex.Replace(finalValue, "([A-Z])([A-Z]+)($|[A-Z])",
                m => m.Groups[1].Value + m.Groups[2].Value.ToLower() + m.Groups[3].Value);

            return char.ToLowerInvariant(finalValue[0]) + finalValue.Substring(1);
        }

        public static string ToLowerCase(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            return text.ToLowerInvariant().Replace("_", "");
        }
    }
}
