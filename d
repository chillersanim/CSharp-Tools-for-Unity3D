warning: LF will be replaced by CRLF in Examples/Common/CameraMovement.cs.
The file will have its original line endings in your working directory
warning: LF will be replaced by CRLF in Examples/Common/Editor/CameraBehaviourEditor.cs.
The file will have its original line endings in your working directory
[1mdiff --git a/Collections/AvlTree.cs b/Collections/AvlTree.cs[m
[1mindex d361bfa..6979296 100644[m
[1m--- a/Collections/AvlTree.cs[m
[1m+++ b/Collections/AvlTree.cs[m
[36m@@ -27,7 +27,7 @@[m [musing System.Threading;[m
 using JetBrains.Annotations;[m
 using UnityEngine;[m
 [m
[31m-namespace Unity_Tools.Collections[m
[32m+[m[32mnamespace UnityTools.Collections[m
 {[m
     /// <summary>[m
     /// The AVL tree with integer keys.[m
[1mdiff --git a/Collections/CollectionMapper.cs b/Collections/CollectionMapper.cs[m
[1mindex 785d7fa..2a4fc68 100644[m
[1m--- a/Collections/CollectionMapper.cs[m
[1m+++ b/Collections/CollectionMapper.cs[m
[36m@@ -25,7 +25,7 @@[m [musing System;[m
 using System.Collections;[m
 using System.Collections.Generic;[m
 [m
[31m-namespace Unity_Tools.Collections[m
[32m+[m[32mnamespace UnityTools.Collections[m
 {[m
     public class CollectionMapper<TIn, TOut> : ICollection<TOut>[m
     {[m
[1mdiff --git a/Collections/EnumerableMapper.cs b/Collections/EnumerableMapper.cs[m
[1mindex 95d1460..f548646 100644[m
[1m--- a/Collections/EnumerableMapper.cs[m
[1m+++ b/Collections/EnumerableMapper.cs[m
[36m@@ -25,7 +25,7 @@[m [musing System;[m
 using System.Collections;[m
 using System.Collections.Generic;[m
 [m
[31m-namespace Unity_Tools.Collections[m
[32m+[m[32mnamespace UnityTools.Collections[m
 {[m
     public class EnumerableMapper<TIn, TOut> : IEnumerable<TOut>[m
     {[m
[1mdiff --git a/Collections/Graph.cs b/Collections/Graph.cs[m
[1mindex 79b8921..3f92d7e 100644[m
[1m--- a/Collections/Graph.cs[m
[1m+++ b/Collections/Graph.cs[m
[36m@@ -26,9 +26,9 @@[m [musing System.Collections;[m
 using System.Collections.Generic;[m
 using System.Collections.ObjectModel;[m
 using JetBrains.Annotations;[m
[31m-using Unity_Tools.Pooling;[m
[32m+[m[32musing UnityTools.Pooling;[m
 [m
[31m-namespace Unity_Tools.Collections[m
[32m+[m[32mnamespace UnityTools.Collections[m
 {[m
     /// <summary>[m
     /// A graph class allows for graph queries using either directed or undirected graph representation.[m
[1mdiff --git a/Collections/IBounds3DCollection.cs b/Collections/IBounds3DCollection.cs[m
[1mindex b8c0bc5..7b22614 100644[m
[1m--- a/Collections/IBounds3DCollection.cs[m
[1m+++ b/Collections/IBounds3DCollection.cs[m
[36m@@ -25,7 +25,7 @@[m [musing System.Collections.Generic;[m
 using JetBrains.Annotations;[m
 using UnityEngine;[m
 [m
[31m-namespace Unity_Tools.Collections[m
[32m+[m[32mnamespace UnityTools.Collections[m
 {[m
     public interface IBounds3DCollection<T> : IEnumerable<T>[m
     {[m
[1mdiff --git a/Collections/IPoint3DCollection.cs b/Collections/IPoint3DCollection.cs[m
[1mindex 7decc06..f283b59 100644[m
[1m--- a/Collections/IPoint3DCollection.cs[m
[1m+++ b/Collections/IPoint3DCollection.cs[m
[36m@@ -23,10 +23,10 @@[m
 [m
 using System.Collections.Generic;[m
 using JetBrains.Annotations;[m
[31m-using Unity_Tools.Core;[m
 using UnityEngine;[m
[32m+[m[32musing UnityTools.Core;[m
 [m
[31m-namespace Unity_Tools.Collections[m
[32m+[m[32mnamespace UnityTools.Collections[m
 {[m
     /// <summary>[m
     /// Interface for generic 3D collections.[m
[36m@@ -89,5 +89,13 @@[m [mnamespace Unity_Tools.Collections[m
         /// <param name="shape">The shape to use.</param>[m
         /// <returns>Returns an <see cref="IEnumerable{T}"/> that provides access to all resulting items.</returns>[m
         IEnumerable<T> ShapeCast<TShape>(TShape shape) where TShape : IVolume;[m
[32m+[m
[32m+[m[32m        /// <summary>[m
[32m+[m[32m        /// Populates the output with all items that are inside the given shape.[m
[32m+[m[32m        /// </summary>[m
[32m+[m[32m        /// <typeparam name="TShape">The type of the shape to cast for.</typeparam>[m
[32m+[m[32m        /// <param name="shape">The shape to use.</param>[m
[32m+[m[32m        /// <param name="output">The output in which to store the shape cast result.</param>[m
[32m+[m[32m        void ShapeCast<TShape>(TShape shape, IList<T> output) where TShape : IVolume;[m
     }[m
 }[m
\ No newline at end of file[m
[1mdiff --git a/Collections/ISphere3DCollection.cs b/Collections/ISphere3DCollection.cs[m
[1mindex 99f5abb..076f84e 100644[m
[1m--- a/Collections/ISphere3DCollection.cs[m
[1m+++ b/Collections/ISphere3DCollection.cs[m
[36m@@ -25,7 +25,7 @@[m [musing System.Collections.Generic;[m
 using JetBrains.Annotations;[m
 using UnityEngine;[m
 [m
[31m-namespace Unity_Tools.Collections[m
[32m+[m[32mnamespace UnityTools.Collections[m
 {[m
     public interface ISphere3DCollection<T> : IEnumerable<T>[m
     {[m
[1mdiff --git a/Collections/Internals/Spatial3DCell.cs b/Collections/Internals/Spatial3DCell.cs[m
[1mindex 2e44c2f..0f7ab20 100644[m
[1m--- a/Collections/Internals/Spatial3DCell.cs[m
[1m+++ b/Collections/Internals/Spatial3DCell.cs[m
[36m@@ -25,7 +25,7 @@[m [musing System.Collections.Generic;[m
 using JetBrains.Annotations;[m
 using UnityEngine;[m
 [m
[31m-namespace Unity_Tools.Collections.Internals[m
[32m+[m[32mnamespace UnityTools.Collections.Internals[m
 {[m
     /// <summary>[m
     ///     The spatial 3 d cell. [m
[36m@@ -101,7 +101,7 @@[m [mnamespace Unity_Tools.Collections.Internals[m
         public int TotalItemAmount;[m
 [m
         /// <summary>[m
[31m-        ///     Initializes static members of the <see cref="Spatial3DCell" /> class.[m
[32m+[m[32m        ///     Initializes static members of the <see cref="Spatial3DCell{T}" /> class.[m
         /// </summary>[m
         static Spatial3DCell()[m
         {[m
[36m@@ -229,7 +229,7 @@[m [mnamespace Unity_Tools.Collections.Internals[m
         /// </param>[m
         /// <param name="addChildren">Should the child array be attached.</param>[m
         /// <returns>[m
[31m-        ///     The <see cref="Spatial3DCell" />.[m
[32m+[m[32m        ///     The <see cref="Spatial3DCell{T}" />.[m
         /// </returns>[m
         [NotNull][m
         public static Spatial3DCell<T> GetCell(Vector3 start, Vector3 size, bool addChildren = false)[m
[36m@@ -441,7 +441,7 @@[m [mnamespace Unity_Tools.Collections.Internals[m
         ///     The index.[m
         /// </param>[m
         /// <returns>[m
[31m-        ///     The <see cref="Spatial3DCell" />.[m
[32m+[m[32m        ///     The <see cref="Spatial3DCell{T}" />.[m
         /// </returns>[m
         [Pure][m
         private Spatial3DCell<T> CreateContainer(int index)[m
[36m@@ -458,7 +458,7 @@[m [mnamespace Unity_Tools.Collections.Internals[m
         ///     The get child array.[m
         /// </summary>[m
         /// <returns>[m
[31m-        ///     The <see cref="Spatial3DCell" />.[m
[32m+[m[32m        ///     The <see cref="Spatial3DCell{T}" />.[m
         /// </returns>[m
         [NotNull][m
         private static Spatial3DCell<T>[] GetChildArray()[m
[1mdiff --git a/Collections/ListMapper.cs b/Collections/ListMapper.cs[m
[1mindex 59cc461..2e11ef4 100644[m
[1m--- a/Collections/ListMapper.cs[m
[1m+++ b/Collections/ListMapper.cs[m
[36m@@ -25,7 +25,7 @@[m [musing System;[m
 using System.Collections;[m
 using System.Collections.Generic;[m
 [m
[31m-namespace Unity_Tools.Collections[m
[32m+[m[32mnamespace UnityTools.Collections[m
 {[m
     /// <summary>[m
     /// A list mapper that changes the interface using a mapping function[m
[1mdiff --git a/Collections/ObservableCollectionMapper.cs b/Collections/ObservableCollectionMapper.cs[m
[1mindex 7326096..77524f9 100644[m
[1m--- a/Collections/ObservableCollectionMapper.cs[m
[1m+++ b/Collections/ObservableCollectionMapper.cs[m
[36m@@ -28,7 +28,7 @@[m [musing System.Collections.ObjectModel;[m
 using System.Collections.Specialized;[m
 using JetBrains.Annotations;[m
 [m
[31m-namespace Unity_Tools.Collections[m
[32m+[m[32mnamespace UnityTools.Collections[m
 {[m
     /// <summary>[m
     ///     The observable collection mapper.[m
[1mdiff --git a/Collections/ObservableFilteredCollection.cs b/Collections/ObservableFilteredCollection.cs[m
[1mindex 191516f..e890116 100644[m
[1m--- a/Collections/ObservableFilteredCollection.cs[m
[1m+++ b/Collections/ObservableFilteredCollection.cs[m
[36m@@ -29,9 +29,9 @@[m [musing System.ComponentModel;[m
 using System.Linq;[m
 using System.Runtime.CompilerServices;[m
 using JetBrains.Annotations;[m
[31m-using Unity_Tools.Core;[m
[32m+[m[32musing UnityTools.Core;[m
 [m
[31m-namespace Unity_Tools.Collections[m
[32m+[m[32mnamespace UnityTools.Collections[m
 {[m
     /// <summary>[m
     ///     The filtered collection.[m
[1mdiff --git a/Collections/PointOctree.cs b/Collections/PointOctree.cs[m
[1mindex 1200d66..e88929a 100644[m
[1m--- a/Collections/PointOctree.cs[m
[1m+++ b/Collections/PointOctree.cs[m
[36m@@ -25,13 +25,13 @@[m [musing System;[m
 using System.Collections;[m
 using System.Collections.Generic;[m
 using System.Diagnostics;[m
[31m-using Unity_Tools.Core;[m
[31m-using Unity_Tools.Pooling;[m
 using UnityEngine;[m
[32m+[m[32musing UnityTools.Core;[m
[32m+[m[32musing UnityTools.Pooling;[m
 using Debug = UnityEngine.Debug;[m
 using Random = UnityEngine.Random;[m
 [m
[31m-namespace Unity_Tools.Collections[m
[32m+[m[32mnamespace UnityTools.Collections[m
 {[m
     public class PointOctree<T> : IPoint3DCollection<T>[m
     {[m
[36m@@ -407,6 +407,115 @@[m [mnamespace Unity_Tools.Collections[m
             CastPathCache.Add(path);[m
         }[m
 [m
[32m+[m[32m        public void ShapeCast<TShape>(TShape shape, IList<T> output) where TShape : IVolume[m
[32m+[m[32m        {[m
[32m+[m[32m            if (output == null)[m
[32m+[m[32m            {[m
[32m+[m[32m                throw new ArgumentNullException(nameof(output));[m
[32m+[m[32m            }[m
[32m+[m
[32m+[m[32m            if (!CastPathCache.TryExtractLast(out var path))[m
[32m+[m[32m            {[m
[32m+[m[32m                path = new CastPathEntry[MaxDepth + 1];[m
[32m+[m[32m            }[m
[32m+[m
[32m+[m[32m            var pathDepth = 0;[m
[32m+[m[32m            var localMin = min;[m
[32m+[m[32m            var halfSize = size / 2f;[m
[32m+[m
[32m+[m[32m            var index = 0;[m
[32m+[m[32m            var childIndex = ChildIndexOffset - 1;[m
[32m+[m[32m            var fullyInside = shape.ContainsAabb(min, min + size);[m
[32m+[m
[32m+[m[32m            while (pathDepth >= 0)[m
[32m+[m[32m            {[m
[32m+[m[32m                childIndex++;[m
[32m+[m
[32m+[m[32m                if (index < 0)[m
[32m+[m[32m                {[m
[32m+[m[32m                    var leaf = leafs[-(index + 1)];[m
[32m+[m[32m                    for (var i = 0; i < leaf.Count; i++)[m
[32m+[m[32m                    {[m
[32m+[m[32m                        if (fullyInside || shape.ContainsPoint(leaf.Content[i].Position))[m
[32m+[m[32m                        {[m
[32m+[m
[32m+[m[32m                            output.Add(leaf.Content[i].item);[m
[32m+[m[32m                        }[m
[32m+[m[32m                    }[m
[32m+[m[32m                }[m
[32m+[m[32m                else[m
[32m+[m[32m                {[m
[32m+[m[32m                    if (childIndex < NodeSize)[m
[32m+[m[32m                    {[m
[32m+[m[32m                        if (nodes[index + childIndex] == 0)[m
[32m+[m[32m                        {[m
[32m+[m[32m                            // Child is empty[m
[32m+[m[32m                            continue;[m
[32m+[m[32m                        }[m
[32m+[m
[32m+[m[32m                        var isInside = fullyInside;[m
[32m+[m[32m                        var childFullyInside = fullyInside;[m
[32m+[m[32m                        var childMin = localMin;[m
[32m+[m
[32m+[m[32m                        if (!fullyInside)[m
[32m+[m[32m                        {[m
[32m+[m[32m                            // Child aabb only matters if it is not fully inside, otherwise we won't need it for sub nodes[m
[32m+[m[32m                            var quadrantIndex = childIndex - ChildIndexOffset;[m
[32m+[m
[32m+[m[32m                            if ((quadrantIndex & 4) == 4)[m
[32m+[m[32m                            {[m
[32m+[m[32m                                childMin.x += halfSize.x;[m
[32m+[m[32m                            }[m
[32m+[m
[32m+[m[32m                            if ((quadrantIndex & 2) == 2)[m
[32m+[m[32m                            {[m
[32m+[m[32m                                childMin.y += halfSize.y;[m
[32m+[m[32m                            }[m
[32m+[m
[32m+[m[32m                            if ((quadrantIndex & 1) == 1)[m
[32m+[m[32m                            {[m
[32m+[m[32m                                childMin.z += halfSize.z;[m
[32m+[m[32m                            }[m
[32m+[m
[32m+[m[32m                            var childMax = childMin + halfSize;[m
[32m+[m[32m                            isInside = shape.IntersectsAabb(childMin, childMax);[m
[32m+[m
[32m+[m[32m                            if (isInside)[m
[32m+[m[32m                            {[m
[32m+[m[32m                                childFullyInside = shape.ContainsAabb(childMin, childMax);[m
[32m+[m[32m                            }[m
[32m+[m[32m                        }[m
[32m+[m
[32m+[m[32m                        if (isInside)[m
[32m+[m[32m                        {[m
[32m+[m[32m                            path[pathDepth] = new CastPathEntry(index, childIndex, fullyInside, localMin, halfSize);[m
[32m+[m[32m                            pathDepth++;[m
[32m+[m[32m                            index = nodes[index + childIndex];[m
[32m+[m[32m                            childIndex = ChildIndexOffset - 1;[m
[32m+[m[32m                            fullyInside = childFullyInside;[m
[32m+[m[32m                            localMin = childMin;[m
[32m+[m[32m                            halfSize /= 2f;[m
[32m+[m[32m                        }[m
[32m+[m
[32m+[m[32m                        continue;[m
[32m+[m[32m                    }[m
[32m+[m[32m                }[m
[32m+[m
[32m+[m[32m                // Go one layer up[m
[32m+[m[32m                pathDepth--;[m
[32m+[m[32m                if (pathDepth < 0) break;[m
[32m+[m
[32m+[m[32m                var c = path[pathDepth];[m
[32m+[m[32m                index = c.Index;[m
[32m+[m[32m                childIndex = c.ChildIndex;[m
[32m+[m[32m                fullyInside = c.Flag;[m
[32m+[m[32m                localMin = c.LocalMin;[m
[32m+[m[32m                halfSize = c.HalfSize;[m
[32m+[m[32m            }[m
[32m+[m
[32m+[m[32m            CastPathCache.Add(path);[m
[32m+[m[32m        }[m
[32m+[m
         /// <inheritdoc/>[m
         public bool MoveItem(T item, Vector3 @from, Vector3 to)[m
         {[m
[1mdiff --git a/Collections/Simple3DBoundsCollection.cs b/Collections/Simple3DBoundsCollection.cs[m
[1mindex 02b0b95..9a969e3 100644[m
[1m--- a/Collections/Simple3DBoundsCollection.cs[m
[1m+++ b/Collections/Simple3DBoundsCollection.cs[m
[36m@@ -24,10 +24,10 @@[m
 using System.Collections;[m
 using System.Collections.Generic;[m
 using JetBrains.Annotations;[m
[31m-using Unity_Tools.Core;[m
[32m+[m[32musing UnityTools.Core;[m
 using UnityEngine;[m
 [m
[31m-namespace Unity_Tools.Collections[m
[32m+[m[32mnamespace UnityTools.Collections[m
 {[m
     /// <summary>[m
     /// Reference implementation for <see cref="IBounds3DCollection{T}"/>.<br/>[m
[1mdiff --git a/Collections/Simple3DCollection.cs b/Collections/Simple3DCollection.cs[m
[1mindex b458745..9143c72 100644[m
[1m--- a/Collections/Simple3DCollection.cs[m
[1m+++ b/Collections/Simple3DCollection.cs[m
[36m@@ -21,13 +21,14 @@[m
 // The above copyright notice and this permission notice shall be included in all[m
 // copies or substantial portions of the Software.[m
 [m
[32m+[m[32musing System;[m
 using System.Collections;[m
 using Sys