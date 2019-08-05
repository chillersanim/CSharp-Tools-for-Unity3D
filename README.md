# Unity Collections
### What is the Unity Collections repo  
The Unity Collection repo contains some useful collections I've made for myself and wanted to share.  
Here is an overview over the supported collections.

### Spatial3DTree  
A generic collection that offers an efficient point lookup in 3D space using a custom octree implementation.  

The Spatial3DTree implements a self-regulating [Octree](https://en.wikipedia.org/wiki/Octree).  
When items gets added, it grows the octree if needed while keeping the existing data intact.  
On the other hand, when items are removed, the octree shrinks as much as possible.  
As many objects move around in Unity, moving objects in the Spatial3DTree is also supported.

To find items in the Spatial3DTree, one uses specialized enumerators.  
These enumerators differ in the shape they are using for inclusion/exclusion testing.

**Enumerators**
  - Spatial3DTreeEnumerator  
    *Enumerates over a Spatial3DTree and returns all items.*
  - SphereCastEnumerator          
    *Enumerates over a Spatial3DTree and returns all items that are within a sphere.*  
  - AabbCastEnumerator  
    *Enumerates over a Spatial3DTree and returns all items that are within an axis aligned bounding box*  
  - ShapeCastEnumerator           
    *Enumerates over a Spatial3DTree and returns all items that are within an user defined shape*
  - InverseSphereCastEnumerator   
    *Enumerates over a Spatial3DTree and returns all items that are outside a sphere*  
  - InverseAabbCastEnumerator  
    *Enumerates over a Spatial3DTree and returns all items that are outside an axis aligned bounding box*  
  - InverseShapeCastEnumerator           
    *Enumerates over a Spatial3DTree and returns all items that are outside an user defined shape*  

**Custom filter**  
In order to customize the filter, you can overwrite a few base classes.  
Depending on your use case, different implementations make sense.
  - IShape  
    *Implement this to define the shape.  
    Needs to be used with a ShapeCastEnumerator or InverseShapeCastEnumerator to get the items.*
  - Spatial3DTreeInclusionEnumeratorBase  
    *Base class for custom Spatial3DTree enumerators that look for items inside a shape.*  
  - Spatial3DTreeExclusionEnumeratorBase  
    *Base class for custom Spatial3DTree enumerators that look for items outside a shape*
    
### ObservableCollectionMapper  
The observable collection mapper is used to get an automatically mapped list from a base list.  
For that, a base list and a mapping function needs to be provided.

If the base list implements the INotifyCollectionChanged interface, changes to the base list are automatically propagated.  
The observable collection mapper can be used like any normal readonly list and implements the INotifyCollectionChanged to inform about changes.  

The observable collection mapper is useful, if you need to repeatedly access the mapped items, without wanting to repeatedly map these items.  

**Usage example**  
Assuming you had a bunch of numbers and wanted to repeatedly print their length, without using ToString() unnecessary often.  
This can be easily achieved by using the observable collection mapper.

``` csharp
var numbers = new ObservableCollection<int>();
var mapping = new ObservableCollectionMapper<int, string>(numbers, n => n.ToString());

numbers.Add(0);
numbers.Add(10);

foreach (var text in mapping)
{
    Console.Write($"{text}, {text.Length}; ");
}

numbers.Add(100);
numbers.Add(1000);

Console.WriteLine();
foreach (var text in mapping)
{
    Console.Write($"{text}, {text.Length}; ");
}

// Output:
// 0, 1; 10, 2;
// 0, 1; 10, 2; 100, 3; 1000, 4;
```

This is a rather simple (and silly) example.  
But it should give an understanding of what the observable collection mapper can do.

**Limitations**  
Changes to item properties used in the mapping aren't propagated to the output.  
This is not currently supported.

### ObservableFilteredCollection  
The observable filtered collection is used to get an automatically filtered list from a base list.  
It works gives access to the same items as in the base list, but only those that match the filter.  
For that a base list and a filter function needs to be provided.  

If the base list implements the INotifyCollectionChanged interface, changes to the base list are automatically propagated.  
The observable filter collection can be used like any normal readonly list and implements the INotifyCollectionChanged to inform about changes.  

The observable filtered collection is useful, if you need to repeatedly access the filtered items, without wanting to repeatedly do the filtering.

**Usage example**  
Assuming you have a bunch of numbers and want to only print even numbers.  
This can be easily achieved by using the observable filtered collection.

``` csharp
var numbers = new ObservableCollection<int>();
var mapping = new ObservableFilteredCollection<int>(numbers, n => n % 2 == 0);

numbers.Add(0);
numbers.Add(1);
numbers.Add(2);

foreach (var text in mapping)
{
    Console.Write($"{text}, ");
}

numbers.Add(5);
numbers.Add(1000);
numbers.Add(1001);

Console.WriteLine();
foreach (var text in mapping)
{
    Console.Write($"{text}, ");
}

// Output:
// 0, 2
// 0, 2, 1000
```

This is a rather simple example.  
But it should give an understanding of what the observable filtered collection can do.
