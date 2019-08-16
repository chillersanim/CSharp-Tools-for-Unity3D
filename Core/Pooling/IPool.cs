using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unity_Tools.Core.Pooling
{
    public interface IPool<T> where T : class
    {
        int MaxSize { get; set; }

        void Put(T item);

        T Get();
    }
}
