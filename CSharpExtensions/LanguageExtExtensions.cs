using System;
using LanguageExt;
using static LanguageExt.Prelude;

namespace CSharpExtensions
{
    public static class LanguageExtExtensions
    {
        /// <summary>
        /// Returns a Some/Exception if it's in a Fail state.
        /// </summary>
        /// <param name="try">The try item.</param>
        /// <typeparam name="T">The try type.</typeparam>
        /// <returns>Returns the exception.</returns>
        public static Option<Exception> GetException<T>(this Try<T> @try) =>
            @try.Match(v => Option<Exception>.None, Some);
        
        public static Try<T> MapFail<T>(this Try<T> @try, Func<Exception, Try<T>> func) =>
            @try.Match(v => @try, func);
        
        public static Try<T> MapFail<T>(this Try<T> @try, Func<Try<T>> func) =>
            @try.Match(v => @try, func());
        
        public static Try<T> MapFail<T>(this Try<T> @try, T item) =>
            @try.Match(v => @try, Try(item));
        
        public static Try<T> MapFail<T>(this Try<T> @try, Exception exception) =>
            @try.Match(v => @try, Try<T>(exception));
        
        public static Try<T> MapFail<T>(this Try<T> @try, Func<Exception, Exception> func) =>
            @try.Match(v => @try, ex => Try<T>(func(ex)));
    }
}