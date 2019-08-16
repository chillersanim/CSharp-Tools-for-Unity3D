# Unity Tools

Some c# scripts that can be useful for developing in Unity3D.  
I made them for myself, and decided to publish them.
See the wiki for more information.

This collection is a work in progress and prone to changes.

----

Currently in C# Tools for Unity available:  

**Core**  
 - Math3D 
   - Many useful methods for common 3D math operations
   - Nearest point, intersection, etc.
 - VectorMath
   - Many useful methods for vector operations (for Vector2, Vector2Int, Vector3, Vector3Int)
   - Component modifications (clamp, min, max, etc. for individual components)
 - SingletonBehavior
   - Base class for all singleton monobehaviors
 - Call provider
   - Provides centralized callback functionality that allows subscription based callbacks
   - Update, FixedUpdate, OnDrawGizmos, EditorOnlyUpdate
 - MeshUtil
   - Provides some mesh operations
   - IsConvex, IsCuboid
 - UnityObjectUtil
   - Some usefull methods for unity object modifications
   - GetOrAddComponent, GetOrCreateChild, GetOrCreateHierarchy, MakeObjectNameUnique, etc.

**Collections**  
 - Octree implementation for 3D points
   - Search for items in Sphere, AABB, Custom shape
   - Auto resize to fit items, no matter the size
 - AVL Tree for generic key value pairs
   - Allows common tree operations
   - Add, Remove, Contains, TryGet, etc.
 - Collection mappers
   - Filter a source collection and get a filtered collection that reflects changes in the source
   - Map items of a source collection and get a mapped collection that reflects changes in the source

**Others**  
 - Polylines
   - Polyline with fast linear interpolation 
   - Simple line
   - Interface and extension method for generic polylines
 - Pipeline framework
   - Create a custom pipeline with predefined or custom nodes.
   - Execute pipeline in a customizable manner
   
**Editor**  
 - DuplicateSelector
   - Selects duplicate renderers in the scene, usefull for finding forgotten copies.
 
----

The files are provided AS IS.  
I do not guaratee that the code is bug free, so use with care and test your stuff.  
If you find a bug, please let me know by opening an issue.  
Thank you!
