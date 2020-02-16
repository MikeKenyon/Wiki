using System;
using System.Collections.Generic;
using System.Text;

namespace Wiki
{
    /// <summary>
    /// A list of extension methods used and for this library.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Applies some action on every element in a list.
        /// </summary>
        /// <typeparam name="T">The type of elements of the list.</typeparam>
        /// <param name="list">A list of those elements.</param>
        /// <param name="action">An action to perform on each element.</param>
        internal static void Apply<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
        }

        /// <summary>
        /// Sanatizes a text string a valid key.
        /// </summary>
        /// <param name="original">The original string.</param>
        /// <returns>A sanatized version of that string.</returns>
        internal static string Sanatize(this string original)
        {
            var bw = true;
            var sb = new StringBuilder(original.Length);

            foreach(var symbol in original)
            {
                if(char.IsNumber(symbol))
                {
                    sb.Append(symbol); // Doesn't effect 'between' state.
                }
                else if (char.IsLetter(symbol))
                {
                    sb.Append(bw ? char.ToUpperInvariant(symbol) : symbol);
                    bw = false;
                }
                else
                {
                    if (ValidKeySymbol(symbol))
                    {
                        sb.Append(symbol);
                    }
                    // else {} -- skipping the symbol.
                    bw = true;
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Filter for valid symbol characters.
        /// </summary>
        /// <param name="symbol">Character to test.</param>
        /// <returns><see langword="true"/> if this is a legit character</returns>
        private static bool ValidKeySymbol(char symbol)
        {
            switch(symbol)
            {
                case '-':
                case '.':
                    return true;
                default:
                    return false;
            }
            //TODO: Once 2.1 is supported by UWP
            //return symbol switch
            //{
            //    '-' => true,
            //    '.' => true,
            //    _ => false,
            //};
        }
    }
}
