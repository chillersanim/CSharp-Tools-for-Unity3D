using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Unity_Tools.Collections;
using Unity_Tools.Core;

namespace Assets.Unity_Tools.Tests
{
    public class Simple3DCollectionTest : I3DCollectionTest<string>
    {
        protected override I3DCollection<string> CreateInstance()
        {
            return new Simple3DCollection<string>();
        }

        protected override string GetItem(int i)
        {
            return i.ToString();
        }
    }
}
