using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
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

        public static TType GetValue<TType>(this Try<TType> tryValue) =>
            tryValue.Match(
                Succ: val => val,
                Fail: err => throw new ApplicationException("Error getting a value from a Try with fail state."));
        
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

        public static Try<TType> ToTry<TType>(this TType item) => Try(item);

        public static Try<TType> OptionToTry<TType>(this Option<TType> item, Exception exception) =>
            item.Match(
                Some: Try,
                None: () => Try<TType>(exception));

        public static Option<TType> TryToOption<TType>(this Try<TType> item) =>
            item.Match(
                Succ: Some,
                Fail: err => None);

        public static Option<TType> Optional<TType>(this TType item) => Prelude.Optional(item);

        public static Try<TType>[] Flatten<TType>(this IEnumerable<Try<Try<TType>>> items) =>
            items
                .ToSeq()
                .Flatten();

        public static Try<TType>[] Flatten<TType>(this Seq<Try<Try<TType>>> items) =>
            items
                .Select(c => c.Flatten())
                .ToArray();

        public static Try<TType> DoIfSucc<TType>(this Try<TType> @try, Action<TType> @delegate)
        {
            @try.Match(@delegate, _ => { });
            return @try;
        }
        
        public static Try<TType> DoIfFail<TType>(this Try<TType> @try, Action<Exception> @delegate)
        {
            @try.Match(_ => { }, @delegate);
            return @try;
        }
        
        public static Option<TType> DoIfSome<TType>(this Option<TType> option, Action<TType> @delegate)
        {
            option.Match(@delegate, () => { });
            return option;
        }
        
        public static Option<TType> DoIfNone<TType>(this Option<TType> option, Action @delegate)
        {
            option.Match(_ => { }, @delegate);
            return option;
        }

        public static Try<Unit> TryAction(Action action) =>
            Try(() =>
            {
                action();
                return Unit.Default;
            });
        
        public static Try<TR> ThenTry<T, TR>(this Try<T> tryResult, Func<T, TR> func) =>
            tryResult.Match(
                Succ: val => Try(() => func(val)),
                Fail: Try<TR>);
        
        public static Try<T> ThenTry<T>(this Try<T> tryResult, Action<T> action) =>
            tryResult.Match(
                Succ: val => Try(() => { action(val); return val; }),
                Fail: Try<T>);

        public static T[] GetSuccesses<T>(this IEnumerable<Try<T>> items) =>
            items
                .Where(item => item.IsSucc())
                .Select(item => item.GetValue())
                .ToArray();
        
        public static Exception[] GetFailuress<T>(this IEnumerable<Try<T>> items) =>
            items
                .Where(item => item.IsFail())
                .Select(item => item.GetException().ValueUnsafe())
                .ToArray();

        public static (T[] Successes, Exception[] Failures) Partition<T>(this IEnumerable<Try<T>> items) =>
            items
                .ToArray()
                .MapTo(i => (Successes: i.GetSuccesses(), Failures: i.GetFailuress()));
    }
}