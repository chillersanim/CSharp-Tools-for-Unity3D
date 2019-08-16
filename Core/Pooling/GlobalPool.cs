using JetBrains.Annotations;

namespace Unity_Tools.Core.Pooling
{
    public static class GlobalPool<T> where T : class, new()
    {
        public static int MaxSize
        {
            get => Pool.MaxSize;
            set => Pool.MaxSize = value;
        }

        private static readonly Pool<T> Pool = new Pool<T>();

        public static T Get()
        {
            return Pool.Get();
        }

        public static void Put([NotNull] T item)
        {
            Pool.Put(item);
        }
    }
}
