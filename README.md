# C# tools for Unity3D

C# tools for Unity is a collection of classes, that can be used as basis for your own code.  
The goal of this script collection is to provide a basis to build upon and take away some of the difficulties in programming 3D code.  
The goal is not to provide finished components, but means to create them.

See the [wiki](https://github.com/chillersanim/CSharp-Tools-for-Unity3D/wiki) for more information.  
Discuss ideas and concerns in [this forum](https://forum.unity.com/threads/open-source-c-library-for-unity-developers.731399/).

This collection is a work in progress and prone to changes.

----

Currently in C# Tools for Unity available:  

**Core** [[Wiki](https://github.com/chillersanim/CSharp-Tools-for-Unity3D/wiki/Core)] 
 - [Math3D](https://github.com/chillersanim/CSharp-Tools-for-Unity3D/blob/master/Core/Math3D.cs)
   - Many useful methods for common 3D math operations
   - Nearest point, intersection, etc.
 - [Nums](https://github.com/chillersanim/CSharp-Tools-for-Unity3D/blob/master/Core/Nums.cs)
   - General purpose operations (Generic GetHashCode, Sort, Revert)
 - [VectorMath](https://github.com/chillersanim/CSharp-Tools-for-Unity3D/blob/master/Core/VectorMath.cs)
   - Many useful methods for vector operations (for Vector2, Vector2Int, Vector3, Vector3Int)
   - Component modifications (clamp, min, max, etc. for individual components)
 - [SingletonBehavior](https://github.com/chillersanim/CSharp-Tools-for-Unity3D/blob/master/Core/SingletonBehaviour.cs)
   - Base class for all singleton monobehaviors
 - [Call provider](https://github.com/chillersanim/CSharp-Tools-for-Unity3D/blob/master/Core/CallProvider.cs)
   - Provides centralized callback functionality that allows subscription based callbacks
   - Update, FixedUpdate, OnDrawGizmos, EditorOnlyUpdate
 - [MeshUtil](https://github.com/chillersanim/CSharp-Tools-for-Unity3D/blob/master/Core/MeshUtil.cs)
   - Provides some mesh operations
   - IsConvex, IsCuboid
 - [UnityObjectUtil](https://github.com/chillersanim/CSharp-Tools-for-Unity3D/blob/master/Core/UnityObjectUtil.cs)
   - Some usefull methods for unity object modifications
   - GetOrAddComponent, GetOrCreateChild, GetOrCreateHierarchy, MakeObjectNameUnique, etc.
 - [Object pooling](https://github.com/chillersanim/CSharp-Tools-for-Unity3D/tree/master/Core/Pooling)  
   - Reuse objects and lists, locally and globally, with cleanup utilites.
   - Smart list polling allows for getting lists of at least a required size.
 - [CuttingEars Triangulation](https://github.com/chillersanim/CSharp-Tools-for-Unity3D/blob/master/Core/CuttingEar.cs)
   - Triangulates polygons using the cutting ears algorithm
   - Optimized variants for convex and concave polygons

**Collections** [[Wiki](https://github.com/chillersanim/CSharp-Tools-for-Unity3D/wiki/Collections)] 
 - [Octree implementation for 3D points](https://github.com/chillersanim/CSharp-Tools-for-Unity3D/blob/master/Collections/Spatial3DTree.cs)
   - Search for items in Sphere, AABB, Custom shape
   - Auto resize to fit items, no matter the size
 - [Simple 3D collection](https://github.com/chillersanim/CSharp-Tools-for-Unity3D/blob/master/Collections/Simple3DCollection.cs)
   - Similar to the 3D octree implementation, but optimized for small datasets.
   - Simple implementation, little garbage
 - [Graph](https://github.com/chillersanim/CSharp-Tools-for-Unity3D/blob/master/Collections/Graph.cs)
   - Graph implementation that allows for graph queries on uni- and bidirectional graphs.
   - FindShortestPath, FindShortestPath (restricted), FindShortestPathToNearest, FindConnectedMatches
 - [AVL Tree for generic key value pairs](https://github.com/chillersanim/CSharp-Tools-for-Unity3D/blob/master/Collections/AvlTree.cs)
   - Allows common tree operations
   - Add, Remove, Contains, TryGet, etc.
 - [Observable collection mappers](https://github.com/chillersanim/CSharp-Tools-for-Unity3D/blob/master/Collections/ObservableCollectionMapper.cs)
   - Map items of a source collection and get a mapped collection that reflects changes in the source
 - [Observable filtered collection](https://github.com/chillersanim/CSharp-Tools-for-Unity3D/blob/master/Collections/ObservableFilteredCollection.cs)
   - Filter a source collection and get a filtered collection that reflects changes in the source   

**Others**  
 - [Polylines](https://github.com/chillersanim/CSharp-Tools-for-Unity3D/tree/master/Core/Polyline)
   - Polyline with fast linear interpolation 
   - Simple line
   - Interface and extension method for generic polylines
 - [Pipeline framework](https://github.com/chillersanim/CSharp-Tools-for-Unity3D/tree/master/Pipeline) [[Wiki](https://github.com/chillersanim/CSharp-Tools-for-Unity3D/blob/master/Collections/Simple3DCollection.cs)]
   - Create a custom pipeline with predefined or custom nodes.
   - Execute pipeline in a customizable manner
 - [Mesh data](https://github.com/chillersanim/CSharp-Tools-for-Unity3D/blob/master/Modeling/MeshData.cs) [WIP]
   - Experimental mesh builder
   - Stores mesh data (Vertices, Edges, Polygons) in an optimized manner that allows for quick update and data operations.
   - *Work in progress, probably contains bugs, prone to changes.*
 
----

The files are provided AS IS.  
Some components have been tested using Unity Tests.  
I do not guaratee that the code is bug free, so use with care and test your stuff.  
If you find a bug, please let me know by opening an issue.  
Thank you!
