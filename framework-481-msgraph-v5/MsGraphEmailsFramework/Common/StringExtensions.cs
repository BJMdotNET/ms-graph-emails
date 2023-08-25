using System;
using System.Text;

namespace MsGraphEmailsFramework.Common
{
    internal static class StringExtensions
    {
        public static bool EqualsIgnoringCase(this string source, string comparison)
            => source.Equals(comparison, StringComparison.OrdinalIgnoreCase);

        public static bool ContainsIgnoringCase(this string source, string substring)
        {
            return Contains(source, substring, StringComparison.InvariantCultureIgnoreCase);
        }

        private static bool Contains(this string source, string substring, StringComparison stringComparison)
        {
            if (substring == null)
            {
                throw new ArgumentNullException(nameof(substring), $"{nameof(substring)} cannot be null.");
            }

            if (!Enum.IsDefined(typeof(StringComparison), stringComparison))
            {
                throw new ArgumentException($"{nameof(stringComparison)} is not a member of StringComparison", nameof(stringComparison));
            }

            return source.IndexOf(substring, stringComparison) >= 0;
        }

        public static bool EndsWithIgnoringCase(this string source, string substring)
            => source.EndsWith(substring, StringComparison.OrdinalIgnoreCase);
    }
}
