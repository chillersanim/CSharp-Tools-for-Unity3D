using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity_Tools.Collections;

using Random = UnityEngine.Random;

namespace Unity_Tools.Tests
{
    public class CollectionUtilTest
    {
        [Test]
        public void BinarySearchLocationTest([Values(0, 1, 2, 3, 4, 5, 10, 100, 1000)]
            int itemAmount)
        {
            var items = new List<float>(itemAmount);

            // Create values between -5 and 5
            for (var i = 0; i < itemAmount; i++)
            {
                items.Add(Random.value * 10 - 5);
            }

            items.Sort();

            // Test for any values between -7.5 and 7.5
            for (var i = 0; i < 10; i++)
            {
                var valueToSearch = Random.value * 15 - 7.5f;
                var index = items.BinarySearchLocation(valueToSearch);
                TestBinarySearchLocationResult(items, index, valueToSearch);
            }

            // Test for specific items in the values range
            if (items.Count > 0)
            {
                for (var i = 0; i < 10; i++)
                {
                    var valueToSearch = items[Random.Range(0, items.Count)];
                    var index = items.BinarySearchLocation(valueToSearch);
                    TestBinarySearchLocationResult(items, index, valueToSearch);
                }
            }

            Assert.AreEqual(0, items.BinarySearchLocation(float.NegativeInfinity));
            Assert.AreEqual(items.Count, items.BinarySearchLocation(float.PositiveInfinity));
        }

        private void TestBinarySearchLocationResult(List<float> items, int index, float value)
        {
            if (items.Count == 0)
            {
                Assert.AreEqual(index, 0);
            }
            else if (index == 0)
            {
                Assert.LessOrEqual(value, items[0]);
            }
            else if (index == items.Count)
            {
                Assert.Less(items[index - 1], value);
            }
            else
            {
                Assert.LessOrEqual(value, items[index]);
                Assert.Less(items[index - 1], value);
            }
        }
    }
}
