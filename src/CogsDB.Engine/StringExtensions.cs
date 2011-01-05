using System;

namespace CogsDB.Engine
{
    public static class StringExtensions
    {
        public static string TextBefore(this string key, string value)
        {
            if (key == null) throw new NullReferenceException();
            var patternIndex = key.IndexOf(value);
            return patternIndex < 0 ? String.Empty : key.Substring(0, patternIndex);
        }

        public static string TextAfter(this string key, string value)
        {
            if (key == null) throw new NullReferenceException();
            var patternIndex = key.IndexOf(value);
            return patternIndex < 0 ? String.Empty : key.Substring(patternIndex + value.Length);
        }
    }
}
