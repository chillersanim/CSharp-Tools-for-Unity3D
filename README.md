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
This isn't the full table of content, but rather an overview.
For the full table of content, check the [wiki](https://github.com/chillersanim/CSharp-Tools-for-Unity3D/wiki).

**Collections**  
 - AvlTree
 - Various collection mappers
 - Collection filter
 - Graph
 - Point collections
 - Simplified point, bounds and sphere collections

**Components**    
 - CallProvider
 - SingletonBehavior

**Core**     
 - MainThreadDispatch
 - Interpolations
 - Matrices (3x3, 4x4, MxN) single and double precision
 - Vectors (2, 3, 4, N) double precision
 - Polylines, Polygons, Shapes, Volumes and Surfaces
 - Utilities

**Pipeline**    
 - PipelineGraph
 - Pipeline nodes
 - Filters
 - Specialized nodes for common tasks

**Pooling**  
 - GlobalListPool
 - GlobalPool
 - ListPool
 - Pool
 - SpecializedPool

**Text**  
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
