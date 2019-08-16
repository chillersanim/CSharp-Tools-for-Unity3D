using System;
using JetBrains.Annotations;

namespace Unity_Tools.Core.Pooling
{
    public class SpecializedPool<T> : PoolBase<T> where T : class
    {
        [NotNull]
        private readonly Func<T> constructor;

        public SpecializedPool([NotNull]Func<T> constructor)
        {
            this.constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
        }

        protected override T CreateItem()
        {
            return constructor();
        }
    }
}