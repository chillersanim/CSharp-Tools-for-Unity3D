# C# tools for Unity3D

C# tools for Unity is a class library that provides tools that solves common coding problems in Unity.  
It provides hand-optimized data structures, algorithms, components and utilities that often come in handy.  
While it offers a few mono behaviors, its main focus is on providing coding tools.  

## Links  
Check the [wiki](https://github.com/chillersanim/CSharp-Tools-for-Unity3D/wiki) for detailed information.  
Discuss ideas and ask questions in [this forum](https://forum.unity.com/threads/open-source-c-library-for-unity-developers.731399/).

## Important  
This collection is a work in progress and prone to changes.  
New versions might contain breaking changes and require you to adapt your code.

## Table of content  

**Collections**  
*Various collections and collection modifiers.* </summary>
 - AvlTree
 - Various collection mappers
 - Collection filter
 - Graph
 - Point collections
 - Simplified point, bounds and sphere collections

**Components**  
*Some mono behaviors that provide commonly required functionality.* 
 - CallProvider
 - MainThreadDispatch
 - SingletonBehavior

**Core**  
*The core of the library, provides a wide range of commonly used tools.  
The classes below are categorized in different sub-folders, however they all share the same namespace: "Unity_Tools.Core"*

  - Common/  
  *Contains classes that don't belong anywhere else.*  
    - CallController
    - Interval
  
  - Interfaces/  
  *Contains all interfaces of the core namespace.*
    - IBounds3DCollection
    - IFilter
    - IInterpolation
    - IPoint3DCollection
    - IPolygon
    - IPolyline
    - IReusable
    - ISphere3DCollection
    - IVector
    - IVectorD
    - IVectorF
    - IVolume
  
  - Interpolation/  
  *Provides interpolation data structures that allows you to interpolate scalar data in various ways.*  
    - CubicHermiteInterpolation
    - LinearInterpolation
    - PolynomialInterpolation
    - PolynomialInterpolation2
  
  - Matrix/  
  *Some matrix and vector implementations for custom-dimensional matrices and vectors as well as double precision vectors.*  
    - Matrix3x3d
    - Matrix3x3f
    - Matrix4x4d
    - Matrix4x4f
    - MatrixD
    - MatrixF
    - MatrixFactory
    - MatrixMxNd
    - MatrixMxNf
    - Vector2d
    - Vector3d
    - Vector4d
    - VectorFactory
    - VectorNd
    - VectorNf
  
  - Primitives/  
  *3D primitives such as poly lines, polygons, shapes, volumes and surfaces.*
    - Aabb
    - CatmullRomSpline
    - Line
    - LinearPolyline
    - LineSegment
    - Polygon
    - Sphere
    - VolumeAll
    - VolumeDifference
    - VolumeEmpty
    - VolumeIntersection
    - VolumeInverse
    - VolumePlane
    - VolumeUnion
  
  - Utilities/  
  *Static utility classes providing various functionalities and extension methods.*  
    - CameraUtil
    - CollectionUtil
    - CommonUtil
    - Math3D
    - MeshUtil
    - NumericsUtil
    - PolygonUtil
    - PolylineUtil
    - UnityObjectUtil
    - VectorUtil

**Pipeline**  
*Provides a flexible pipeline that allows you to efficiently pipeline work.*  
 - IItemReciver
 - IPipelineNode
 - PipelineBase
 - PipelineEnd
 - PipelineFilter
 - PipelineGraph
 - PipelineItemFactory
 - PipelineItemWorker
 - PipelineStart
 - PipelineWorker  
 _
 - Specialized/  
 *Contains specialized pipeline nodes, these should be considered first before implementing custom nodes.*
   - AddCollider
   - AddComponent
   - FilterByAnyComponents
   - FilterByComponent
   - GameObjectCollector
   - GenerateLightmapUvs
   - RemoveComponents
   - RemovePrefabLink
   - SetLayer
   - SetMaterial
   - SetStaticFlag

**Pooling**  
*Provides local and global object pools for various types.*  
 - GlobalListPool
 - GlobalPool
 - IPool
 - ListPool
 - Pool
 - PoolBase
 - SpecializedPool

**Text**  
*Tools and classes for working with strings and text.*
 - CsvHelper
 - StreamReplacement
 
## Installation  
To use this library in your Unity project, you simply need to download this project and place it in your project's asset folder.  
Make sure that you don't place it in an Editor folder, otherwise the code won't be available when building your game.
 
## Note  
The files are provided AS IS.  
Some components have been tested using Unity Tests.  
I do not guarantee that the code is bug free, so use with care and test your stuff.  
If you find a bug, please let me know by opening an issue.  
Thank you!
