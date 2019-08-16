using JetBrains.Annotations;

namespace Unity_Tools.Core.Pooling
{
    public sealed class Pool<T> : PoolBase<T> where T : class, new()
    {
        protected override T CreateItem()
        {
            return new T();
        }
    }
}
