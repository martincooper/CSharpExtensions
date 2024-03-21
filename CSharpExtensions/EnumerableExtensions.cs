using System.Linq;

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
    }
}