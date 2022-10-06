using System;
using LanguageExt;

namespace CSharpExtensions
{
    public static class FunctionalExtensions
    {
        public static Unit Do(Action action)
        {
            action();
            return Unit.Default;
        } 
        
        public static T Do<T>(this T item, Action action) => 
            Do(action)
                .Select(item);

        public static T Do<T>(this T item, Action<T> action) =>
            Do(() => action(item))
                .Select(item);

        public static T Select<T>(T item) => item;

        public static T Select<T>(Func<T> func) => func();
        
        public static T Select<T>(this object obj, T item) => item;
        
        public static T Select<T>(this object obj, Func<T> func) => func();

        public static TResult MapTo<TInput, TResult>(this TInput item, Func<TInput, TResult> map) => map(item);
    }
}