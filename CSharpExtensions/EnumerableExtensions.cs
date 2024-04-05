using System;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using Optional = LanguageExt.Optional;

namespace CSharpExtensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// An EqualsAny implementation. Checks if the item "Equals" any of the items passed.
        /// </summary>
        /// <param name="item">The item to compare.</param>
        /// <param name="compareToItems">The items to compare to</param>
        /// <typeparam name="T">The item type.</typeparam>
        /// <returns></returns>
        public static bool EqualsAny<T>(this T item, params T[] compareToItems) =>
            compareToItems.Any(p => p.Equals(item));

        /// <summary>
        /// Checks a collection of strings against a single value and returns
        /// the first one which matches, ignoring case.
        /// </summary>
        /// <param name="items">The collection of strings.</param>
        /// <param name="item">The string to check for.</param>
        /// <returns></returns>
        public static Option<string> FirstMatch(this string[] items, string item) =>
            Optional(items.FirstOrDefault(i => i.Equals(item, StringComparison.OrdinalIgnoreCase)));
    }
}