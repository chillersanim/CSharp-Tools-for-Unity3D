namespace Unity_Tools.Core
{
    /// <summary>
    /// Interface for classes that can be cleaned and reused
    /// </summary>
    public interface IReusable
    {
        /// <summary>
        /// Cleans this instance and restores the initial state as it was directly after construction.
        /// </summary>
        void Reuse();
    }
}
