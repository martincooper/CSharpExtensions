using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using LanguageExt;
using static CSharpExtensions.CustomException;

using static CSharpExtensions.FunctionalExtensions;
using static LanguageExt.Prelude;

namespace CSharpExtensions
{
    public static class XElementTryExtensions
    {
        /// <summary>
        /// Parses XML Text to XElement object.
        /// </summary>
        /// <param name="xmlText">The XML text to parse.</param>
        /// <returns></returns>
        public static Try<XElement> TryParseXml(string xmlText) =>
            Try(() => XElement.Parse(xmlText));
        
        //// *** Attribute Values ***

        /// <summary>
        /// Returns the value of the specified attribute.
        /// </summary>
        /// <param name="element">The starting element.</param>
        /// <param name="pathNames">The path names to the attribute.</param>
        /// <returns></returns>
        public static Try<string> TryAttributeValue(this XElement element, params string[] pathNames) =>
            Try(element).TryAttributeValue<string>(pathNames);
        
        /// <summary>
        /// Returns the value of the specified attribute.
        /// </summary>
        /// <param name="element">The starting element.</param>
        /// <param name="pathNames">The path names to the attribute.</param>
        /// <returns></returns>
        public static Try<TResult> TryAttributeValue<TResult>(this XElement element, params string[] pathNames) =>
            Try(element).TryAttributeValue<TResult>(pathNames);
        
        /// <summary>
        /// Returns the value of the specified attribute.
        /// </summary>
        /// <param name="element">The starting element.</param>
        /// <param name="pathNames">The path names to the attribute.</param>
        /// <returns></returns>
        public static Try<string> TryAttributeValue(this Try<XElement> element, params string[] pathNames) =>
            element.TryAttributeValue<string>(pathNames);
        
        /// <summary>
        /// Returns the value of the specified attribute.
        /// </summary>
        /// <param name="element">The starting element.</param>
        /// <param name="pathNames">The path names to the attribute.</param>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <returns></returns>
        public static Try<TResult> TryAttributeValue<TResult>(this Try<XElement> element, params string[] pathNames) =>
            element
                .TryElementAt(pathNames.AllButLast())
                .Bind(elem => TrySingleAttributeValue<TResult>(elem, pathNames.Last()));
       
        private static Try<TResult> TrySingleAttributeValue<TResult>(this XElement element, string attributeName) =>
            element
                .Attribute(attributeName)
                .Optional()
                .Match(
                    Some: attribute => attribute.Value.ConvertToType<TResult>(),
                    None: () => CustomError<TResult>($"Attribute with name '{attributeName}' not found."));

        //// *** Element Values ***

        /// <summary>
        /// Returns the value of the specified element.
        /// </summary>
        /// <param name="element">The starting / parent element.</param>
        /// <param name="elementNames">The child element names.</param>
        /// <returns></returns>
        public static Try<string> TryElementValue(this XElement element, params string[] elementNames) =>
            element.TryElementValue<string>(elementNames);

        /// <summary>
        /// Returns the value of the specified element.
        /// </summary>
        /// <param name="element">The starting / parent element.</param>
        /// <param name="elementNames">The child element names.</param>
        /// <returns></returns>
        public static Try<TResult> TryElementValue<TResult>(this XElement element, params string[] elementNames) =>
            element
                .TryElementAt(elementNames)
                .Bind(elem => elem.Value.ConvertToType<TResult>());
        
        /// <summary>
        /// Returns the value of the specified element.
        /// </summary>
        /// <param name="element">The starting / parent element.</param>
        /// <param name="elementNames">The child element names.</param>
        /// <returns></returns>
        public static Try<string> TryElementValue(this Try<XElement> element, params string[] elementNames) =>
            element.Bind(elem => elem.TryElementValue(elementNames));

        /// <summary>
        /// Returns the value of the specified element.
        /// </summary>
        /// <param name="element">The starting / parent element.</param>
        /// <param name="elementNames">The child element names.</param>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <returns></returns>
        public static Try<TResult> TryElementValue<TResult>(this Try<XElement> element, params string[] elementNames) =>
            element.Bind(elem => elem.TryElementValue<TResult>(elementNames));
        
        //// *** Element At ***        

        /// <summary>
        /// Returns an XElement at the specified path.
        /// </summary>
        /// <param name="element">The starting / parent element.</param>
        /// <param name="elementNames">The child element names.</param>
        /// <returns></returns>
        public static Try<XElement> TryElementAt(this XElement element, params string[] elementNames) =>
            elementNames.Aggregate(Try(element), TryElementAt);
        
        /// <summary>
        /// Returns an XElement at the specified path.
        /// </summary>
        /// <param name="element">The starting / parent element.</param>
        /// <param name="elementNames">The child element names.</param>
        /// <returns></returns>
        public static Try<XElement> TryElementAt(this Try<XElement> element, params string[] elementNames) =>
            element.Bind(elem => elem.TryElementAt(elementNames));
        
        //// *** Elements At ***
        
        /// <summary>
        /// Returns a collection of XElements at the specified path.
        /// </summary>
        /// <param name="element">The starting / parent element.</param>
        /// <param name="elementNames">The child element names.</param>
        /// <returns></returns>
        public static Try<XElement[]> TryElementsAt(this XElement element, params string[] elementNames) =>
            element
                .TryElementAt(elementNames.AllButLast())
                .TryElementsAt(elementNames.Last());
        
        /// <summary>
        /// Returns a collection of XElements at the specified path.
        /// </summary>
        /// <param name="element">The starting / parent element.</param>
        /// <param name="elementNames">The child element names.</param>
        /// <returns></returns>
        public static Try<XElement[]> TryElementsAt(Try<XElement> element, params string[] elementNames) =>
            element.Bind(elem => elem.TryElementsAt(elementNames));
        
        //// **********************
        
        private static Try<XElement> TryElementAt(Try<XElement> element, string elementName) =>
            element.Match(
                Succ: elem => TryElementAt(elem, elementName),
                Fail: Try<XElement>);
        
        private static Try<XElement> TryElementAt(XElement element, string elementName) =>
            element
                .Element(elementName)
                .Optional()
                .Match(
                    Some: Try,
                    None: () => CustomError<XElement>($"Element with name '{elementName}' not found."));
        
        private static Try<XElement[]> TryElementsAt(this XElement element, string elementName) =>
            Select(element.Elements(elementName).ToArray())
                .MapTo(elems => elems.Any() 
                    ? Try(elems) 
                    : CustomError<XElement[]>($"Elements with name '{elementName}' not found."));
        
        private static Try<XElement[]> TryElementsAt(this Try<XElement> element, string elementName) =>
            element.Match(
                Succ: elem => elem.TryElementsAt(elementName),
                Fail: Try<XElement[]>);
        
        private static string[] AllButLast(this string[] items) => 
            items
                .Take(items.Length - 1)
                .ToArray();

        /// <summary>
        /// Builds a new XElement removing all namespaces from the original.
        /// </summary>
        /// <param name="element">The parent element.</param>
        public static Try<XElement> TryRemoveAllNamespaces(XElement element) =>
            Try(() => RemoveAllNamespaces(element));
        
        private static XElement RemoveAllNamespaces(XElement element) =>
            new XElement(element.Name.LocalName,
                from node in element.Nodes()
                select node is XElement elem
                    ? RemoveAllNamespaces(elem)
                    : node,
                element.HasAttributes
                    ? RemoveNamespacesFromAttributes(element)
                    : Enumerable.Empty<XAttribute>());
        
        public static IEnumerable<XAttribute> RemoveNamespacesFromAttributes(XElement element) =>
            from attribute in element.Attributes()
            where !attribute.IsNamespaceDeclaration
            select new XAttribute(attribute.Name.LocalName, attribute.Value);

    }
}