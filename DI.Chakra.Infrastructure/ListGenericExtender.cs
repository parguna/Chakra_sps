using System;
using System.Collections.Generic;

namespace DI.Chakra.Infrastructure
{
    /// <summary>
    /// The extension method holder for IList generic type.
    /// </summary>
    public static class ListGenericExtender
    {
        #region Public Extension Methods
        /// <summary>
        /// Converts the element within the list from one type to another, and generates a new
        /// list.
        /// </summary>
        /// <typeparam name="TSource">The source type of the elements within the list.</typeparam>
        /// <typeparam name="TResult">The target type of the list.</typeparam>
        /// <param name="sourceList">The list in which the elements should be converted.</param>
        /// <param name="convertFunction">The converting function.</param>
        /// <returns>The generated new list.</returns>
        public static IList<TResult> IterateConvert<TSource, TResult>(this IList<TSource> sourceList,
            Func<TSource, TResult> convertFunction)
        {
            IList<TResult> lstResult = new List<TResult>();
            foreach (TSource source in sourceList)
                lstResult.Add(convertFunction(source));
            return lstResult;
        }
        #endregion
    }
}
