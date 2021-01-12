using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityTools.Collections;
using UnityTools.Core;
using Random = UnityEngine.Random;

namespace UnityTools.Tests
{
    public class OpenQueueTest
    {
        [Test]
        public void EnqueueDequeueTest([Range(100, 1000, 100)]int count)
        {
            GenerateAndTestQueue(count, 0, count / 10, out var openQueue, out var items);

            for (var i = 0; i < items.Length; i++)
            {
                var item = openQueue.Dequeue();
                Assert.AreEqual(item, items[i]);
                Assert.AreEqual(openQueue.Count, items.Length - i - 1);
            }
        }

        [Test]
        public void DequeueWhereTest([Range(100, 1000, 100)]int count)
        {
            var min = 0;
            var max = count / 10;

            GenerateAndTestQueue(count, min, max - min, out var openQueue, out var items, true);
            var enumerableQueue = (IEnumerable<int>) openQueue;

            var currentItems = new List<int>(items);
            var dequeuedItems = new List<int>();

            var range10 = (max - min) / 10;
            var center = min + (max - min) / 2;

            var predicates = new List<Predicate<int>>();
            
            // Remove top 10%
            var top10 = max - range10;
            predicates.Add(item => item >= top10);

            // Remove bottom 10%
            var bottom10 = min + range10;
            predicates.Add(item => item <= bottom10);

            // Remove middle 10%
            var center10 = center - range10 / 2;
            predicates.Add(item => item >= center10 && item < center10 + range10);
            
            // Remove upper 50%
            predicates.Add(item => item >= center);

            // Remove rest
            predicates.Add(item => true);

            foreach (var predicate in predicates)
            {
                dequeuedItems.Clear();
                openQueue.DequeueAll(predicate, dequeuedItems);
                TestOrder(currentItems, dequeuedItems, predicate);
                currentItems.RemoveAll(predicate);

                Assert.AreEqual(openQueue.Count, currentItems.Count);

                var index = 0;
                foreach (var item in enumerableQueue)
                {
                    Assert.AreEqual(item, currentItems[index++]);
                }
            }
        }

        [Test]
        public void EmptyTest()
        {
            var openQueue = new OpenQueue<int>();

            Assert.AreEqual(openQueue.Count, 0);

            Assert.Throws<InvalidOperationException>(() => openQueue.Dequeue());
            Assert.Throws<InvalidOperationException>(() => openQueue.Peek());
            Assert.Throws<InvalidOperationException>(() => openQueue.PeekLast());

            Assert.DoesNotThrow(() => openQueue.Clear());
            Assert.DoesNotThrow(() => openQueue.Contains(0));
            Assert.DoesNotThrow(() => openQueue.All(i => true));
            Assert.DoesNotThrow(() => openQueue.Any(i => true));
            Assert.DoesNotThrow(() => openQueue.CountMatches(0));
            Assert.DoesNotThrow(() => openQueue.CountMatches(i => true));
            Assert.DoesNotThrow(() => openQueue.TryDequeue(out _));
            Assert.DoesNotThrow(() => openQueue.TryDequeueFirst(item => true, out _));
        }

        private void GenerateTestData(int count, int from, int range, out int[] result, bool shuffle = true)
        {
            Random.InitState(1337);

            if (range < 2)
            {
                range = 2;
            }

            result = new int[count];
            
            // Generate #variance amount of different items, fill the result with the variant items in a circular matter
            for (var i = 0; i < count; i++)
            {
                var item = (i % range) + from;
                result[i] = item;
            }

            if (shuffle)
            {
                result.Shuffle();
            }
        }

        private void GenerateAndTestQueue(int count, int from, int range, out OpenQueue<int> openQueue, out int[] items, bool shuffle = true)
        {
            GenerateTestData(count, from, range, out items, shuffle);
            openQueue = new OpenQueue<int>();

            Assert.NotNull(openQueue);
            Assert.AreEqual(openQueue.Count, 0);

            for (var i = 0; i < items.Length; i++)
            {
                var item = items[i];
                openQueue.Enqueue(item);

                Assert.AreEqual(openQueue.Count, i + 1);
                Assert.AreEqual(openQueue.PeekLast(), item);
            }

            Assert.AreEqual(openQueue.Peek(), items[0]);
        }

        private void TestOrder(IList<int> source, IList<int> dequeued, Predicate<int> predicate)
        {
            var deqIndex = 0;

            for (var i = 0; i < source.Count; i++)
            {
                var item = source[i];
                if (predicate(item))
                {
                    Assert.IsTrue(dequeued.Count > deqIndex); 
                    Assert.AreEqual(item, dequeued[deqIndex]);
                    deqIndex++;
                }
            }

            Assert.AreEqual(deqIndex, dequeued.Count);
        }
    }
}
