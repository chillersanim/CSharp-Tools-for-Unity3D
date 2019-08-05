// COGARCH COGARCH_Common IFilter.cs
// Copyright © Jasper Ermatinger

#region usings

using JetBrains.Annotations;

#endregion

namespace Common
{
    #region Usings

    #endregion

    /// <summary>
    ///     The Filter interface.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public interface IFilter<in T>
    {
        /// <summary>
        ///     Evaluates whether the item matches the filter criterias.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <returns>
        ///     Returns <c>true</c> if the item matches the filter criterias, <c>false</c> otherwise.
        /// </returns>
        bool Filter([CanBeNull] T item);
    }
}