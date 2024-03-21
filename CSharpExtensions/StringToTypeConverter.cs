using System;
using System.Collections.Generic;
using System.Globalization;
using LanguageExt;
using static LanguageExt.Prelude;

namespace CSharpExtensions
{
    /// <summary>
    /// StringToTypeConverter class. Used for parsing string values into strongly types objects.
    /// </summary>
    public static class StringToTypeConverter
    {
        private static Dictionary<Type, Func<string, Try<object>>> TryValueFromStringMapper { get; set; }

        static StringToTypeConverter()
        {
            TryValueFromStringMapper = new Dictionary<Type, Func<string, Try<object>>>
            {
                { typeof(string), p => Try((object)p) },
                { typeof(Type), p => TryTypeFromString(p).Map(r => (object)r) },
                { typeof(bool), p => TryBoolFromString(p).Map(r => (object)r) },
                { typeof(int), p => TryIntFromString(p).Map(r => (object)r) },
                { typeof(long), p => TryLongFromString(p).Map(r => (object)r) },
                { typeof(decimal), p => TryDecimalFromString(p).Map(r => (object)r) },
                { typeof(double), p => TryDoubleFromString(p).Map(r => (object)r) },
                { typeof(DateTime), p => TryDateTimeFromString(p).Map(r => (object)r) }
            };
        }

        /// <summary>
        /// Generic extension method to convert a string value to the specified type.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <param name="defaultValue">The default value to use.</param>
        /// <typeparam name="TValueType">The value type to convert to.</typeparam>
        public static TValueType ConvertToType<TValueType>(this string value, TValueType defaultValue) =>
            TryValueFromString<TValueType>(value, defaultValue);
        
        /// <summary>
        /// Generic extension method to convert a string value to the specified type.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <typeparam name="TValueType">The value type to convert to.</typeparam>
        public static Try<TValueType> ConvertToType<TValueType>(this string value) =>
            TryValueFromString<TValueType>(value);
        
        /// <summary>
        /// Generic method to convert a string value to the specified type.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <param name="defaultValue">The default value to use.</param>
        /// <typeparam name="TValueType">The value type to convert to.</typeparam>
        public static TValueType TryValueFromString<TValueType>(string value, TValueType defaultValue) =>
            TryValueFromString<TValueType>(value).IfFail(defaultValue);
        
        /// <summary>
        /// Generic method to convert a string value to the specified type.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <typeparam name="TValueType">The value type to convert to.</typeparam>
        public static Try<TValueType> TryValueFromString<TValueType>(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Try<TValueType>(new ArgumentException($"Value specified was null or empty."));

            var valueType = typeof(TValueType);

            // First check if we have a defined mapper for the type.
            if (TryValueFromStringMapper.ContainsKey(valueType))
                return TryValueFromStringMapper[valueType](value)
                    .Match(
                        Succ: val => Try((TValueType)val),
                        Fail: Try<TValueType>);

            // Check if it's an Enum.
            return valueType.IsEnum 
                ? EnumTryParse<TValueType>(value) 
                : Try<TValueType>(new ArgumentException($"Could not convert '{value}' to type '{valueType.Name}'."));
        }
        
        /// <summary>
        /// Converts a Type defined as a string to its typed value.
        /// </summary>
        /// <param name="value">The type as a string.</param>
        /// <param name="defaultValue">The default value to use if it couldn't be parsed.</param>
        public static Type TryTypeFromString(string value, Type defaultValue) =>
            TryTypeFromString(value).IfFail(defaultValue);
        
        /// <summary>
        /// Converts a Type defined as a string to its typed value.
        /// </summary>
        /// <param name="value">The type as a string.</param>
        public static Try<Type> TryTypeFromString(string value) =>
            Try(() => Type.GetType(value))
                .MapFail(err => Try<Type>(new ArgumentException($"Error converting '{value}' to  Type.")));
        
        /// <summary>
        /// Parses a string boolean value. Includes additional values such as Yes/No, True/False.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <param name="defaultValue">The default value to use if it couldn't be parsed.</param>
        public static bool TryBoolFromString(string value, bool defaultValue) =>
            TryBoolFromString(value).IfFail(defaultValue);
        
        /// <summary>
        /// Parses a string boolean value. Includes additional values such as Yes/No, True/False.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        public static Try<bool> TryBoolFromString(string value)
        {
            if (value == null)
                return Try<bool>(new ArgumentException("Error converting 'null' to bool."));

            var valueUpper = value.Trim().ToUpper();

            if (valueUpper.EqualsAny("Y", "YES", "T", "TRUE", "1"))
                return Try(true);
            
            if (valueUpper.EqualsAny("N", "NO", "F", "FALSE", "0"))
                return Try(false);

            // Do a standard bool parse check. 
            return bool.TryParse(value, out var returnValue) 
                ? Try(returnValue) 
                : Try<bool>(new ArgumentException($"Error converting '{value}' to bool."));
        }

        /// <summary>
        /// Parses a string int value.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <param name="defaultValue">The default value.</param>
        public static int TryIntFromString(string value, int defaultValue) =>
            TryIntFromString(value).IfFail(defaultValue);
        
        /// <summary>
        /// Parses a string int value.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        public static Try<int> TryIntFromString(string value) =>
            int.TryParse(value, out var returnValue) 
                ? Try(returnValue) 
                : Try<int>(new ArgumentException($"Error converting '{value}' to int."));
        
        /// <summary>
        /// Parses a string long value.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <param name="defaultValue">The default value.</param>
        public static long TryLongFromString(string value, long defaultValue) =>
            TryLongFromString(value).IfFail(defaultValue);
        
        /// <summary>
        /// Parses a string long value.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        public static Try<long> TryLongFromString(string value) =>
            long.TryParse(value, out var returnValue) 
                ? Try(returnValue) 
                : Try<long>(new ArgumentException($"Error converting '{value}' to long."));
        
        /// <summary>
        /// Parses a decimal decimal value.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <param name="defaultValue">The default value.</param>
        public static decimal TryDecimalFromString(string value, decimal defaultValue) =>
            TryDecimalFromString(value).IfFail(defaultValue);
        
        /// <summary>
        /// Parses a string decimal value.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        public static Try<decimal> TryDecimalFromString(string value) =>
            decimal.TryParse(value, NumberStyles.Any, null, out var returnValue) 
                ? Try(returnValue) 
                : Try<decimal>(new ArgumentException($"Error converting '{value}' to decimal."));

        /// <summary>
        /// Parses a string double value.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <param name="defaultValue">The default value.</param>
        public static double TryDoubleFromString(string value, double defaultValue) =>
            TryDoubleFromString(value).IfFail(defaultValue);
        
        /// <summary>
        /// Parses a string double value.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        public static Try<double> TryDoubleFromString(string value) =>
            double.TryParse(value, out var returnValue) 
                ? Try(returnValue) 
                : Try<double>(new ArgumentException($"Error converting '{value}' to double."));

        /// <summary>
        /// Parses a string DateTime value.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <param name="defaultValue">The default value.</param>
        public static DateTime TryDateTimeFromString(string value, DateTime defaultValue) =>
            TryDateTimeFromString(value).IfFail(defaultValue);
        
        /// <summary>
        /// Parses a string DateTime value.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        public static Try<DateTime> TryDateTimeFromString(string value) =>
            DateTime.TryParse(value, out var returnValue) 
                ? Try(returnValue) 
                : Try<DateTime>(new ArgumentException($"Error converting '{value}' to DateTime."));
        
        /// <summary>
        /// Parses an enum, specifying a default value if parsing fails.
        /// </summary>
        /// <param name="stringValue">The value to parse.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <typeparam name="TEnumType">The enum type.</typeparam>
        /// <returns></returns>
        public static TEnumType EnumFromString<TEnumType>(string stringValue, TEnumType defaultValue) =>
            EnumTryParse<TEnumType>(stringValue).IfFail(defaultValue);
        
        /// <summary>
        /// Generic TryParse method for enumeration values.
        /// </summary>
        /// <param name="strValue">The value as a string.</param>
        /// <typeparam name="TEnumType">The enumeration type</typeparam>
        /// <returns></returns>
        public static Try<TEnumType> EnumTryParse<TEnumType>(string strValue)
        {
            var result = EnumTryParse(typeof(TEnumType), strValue);

            if (result.IsSucc() && result.GetValue() is TEnumType)
                return Try((TEnumType)result.GetValue());
            
            return Try<TEnumType>(new ArgumentException($"Value {strValue} couldn't be converted to enum {typeof(TEnumType).Name}."));
        }
        
        /// <summary>
        /// TryParse method for enumeration values.
        /// </summary>
        /// <param name="enumType">The enumeration type.</param>
        /// <param name="strValue">The value as a string.</param>
        /// <returns>Returns the parsed value if able to be converted.</returns>
        public static Try<object> EnumTryParse(Type enumType, string strValue)
        {
            if (strValue == null) return Try<object>(new ArgumentException($"Value specified was null."));
            if (!enumType.IsEnum) return Try<object>(new ArgumentException($"Type {enumType} not a valid enum."));

            if (Enum.IsDefined(enumType, strValue))
                return Try(Enum.Parse(enumType, strValue, true));

            foreach (string value in Enum.GetNames(enumType))
            {
                if (value.Equals(strValue, StringComparison.OrdinalIgnoreCase))
                    return Try(Enum.Parse(enumType, value));
            }
            
            return Try<object>(new ArgumentException($"Value {strValue} couldn't be converted to enum {enumType.Name}."));
        }
    }
}