using System;
using LanguageExt;
using static LanguageExt.Prelude;

namespace CSharpExtensions
{
    public class CustomException : Exception
    {
        public CustomException()
        { }

        public CustomException(string message) : base(message)
        { }
        
        public CustomException(string message, Exception innerException) : base(message, innerException)
        { }

        public static Try<T> CustomError<T>(string message) =>
            Try<T>(new CustomException(message));
        
        public static Try<T> CustomError<T>(string message, Exception innerException) =>
            Try<T>(new CustomException(message, innerException));        
    }
}