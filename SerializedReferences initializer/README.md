# SerializedReferences Initializer

## Demo

```csharp
// ISing.cs
internal interface ISing
{
    void Sing();
    void StopSinging();
}

// Demo.cs
internal sealed class Demo : MonoBehaviour
{
    [SerializeReference, ShowInitializationMenu]
    private ISing type;
}

// StreetPerformer.cs
[GenerateWrapperFor(typeof(ISing))]
[AddComponentMenu("Singing related/")]
internal sealed class StreetPerformer : MonoBehaviour, ISing
{
    [SerializeField] private GameObject street;
    public void Sing()
    {
        Debug.Log($"Singing on {street.name}");
    }
    public void StopSinging()
    {
        Debug.Log($"Stop singing on {street.name}");
    }
}

// SingingDog.cs
namespace EditorTools.SerializedReferenceInitializer.Example
{
    [CreateAssetMenu(menuName = "SerializedReference initializer/Demo/Singing Dog")]
    [GenerateWrapperFor(typeof(ISing))]
    internal sealed class SingingDog : ScriptableObject, ISing
    {
        [SerializeField] private int age;
        public void Sing()
        {
            Debug.Log("Barkkk");
        }
        public void StopSinging()
        {
            Debug.Log("...");
        }
    }
}

// OperaSinger.cs
[CustomMenuPath("Singing related/Opera")]
internal sealed class OperaSinger : ISing
{
    public void Sing()
    {
        throw new System.NotImplementedException();
    }
    public void StopSinging()
    {
        throw new System.NotImplementedException();
    }
}

```

As you can see, we have an interface and various implementations of it - a normal class, a `MonoBehaviour` and
a `ScriptableObject`.

In the inspector:

![Animation](https://user-images.githubusercontent.com/73391391/219223278-408db810-af50-485b-926b-f291399f2867.gif)

## Description:

The `ShowInitializationMenuAttribute` attribute enables you to initialize the reference for fields that have been marked
with the `SerializeReference` attribute.

When you apply this attribute to a field with the `SerializeReference` attribute, a dropdown menu will appear in the
Unity
inspector for the decorated field. This menu displays all the types that implement the type of the decorated field (
including the field's type itself, if it is instantiatable).

The main benefit of this tool is that you can assign a reference to Unity objects, such as `MonoBehaviour` and
`ScriptableObject`, to a `SerializeReference` field. This means that you are no longer limited to only non-Unity object
classes, or only `MonoBehaviour` or `ScriptableObject` classes.

For Unity object implementations, the dropdown menu will display an option to create a new instance, in addition to all
relevant instances that can be assigned to the field (along with corresponding icons).

## How to use?

To serialize a field in your Unity class, add the `ShowInitializationMenuAttribute` and `SerializeReference` attributes
to the field you want to serialize.

For example:

```csharp
[SerializeReference, ShowInitializationMenu]
private IInterface example;
```

The `SerializeReference` attribute instructs Unity to serialize the field as a reference, rather than a value. You can
find more information on this attribute [here](https://docs.unity3d.com/ScriptReference/SerializeReference.html).

Using the `ShowInitializationMenuAttribute` attribute ensures that Unity will display the custom property drawer.

By default, the dropdown menu does not show types that inherit from `UnityEngine.Object`. If you want this type to
appear in the dropdown menu, add the `GenerateWrapperForAttribute` attribute to your type declaration and pass it the
type that it will wrap.

Here's an example:

```csharp
[GenerateWrapperFor(typeof(IInterface))]
public class UnityObjectExample : MonoBehaviour, IInterface{}
```

This attribute performs some background magic, which allows you to assign a Unity object to a `SerializeReference`
field (well, technically it creates the illusion that you can do so).

If you want to modify a type's path in the dropdown, you can use either the built-in attribute `AddComponentMenu` or the
custom attribute `CustomMenuPathAttribute`. Both attributes work the same way and produce the same result. The
`CustomMenuPathAttribute` was created because you might see a warning when using the `AddComponentMenu` attribute on a
type that does not inherit from `UnityEngine.Object`.

You can use this approach with interfaces and classes (as long as they do not inherit from `UnityEngine.Object`),
but generic types like `IGenericInterface<int>` won't work because Unity does not support serialization of generic
types.

That's all! ☺

## Known bugs/limitations:

* Serialized interfaces that are implemented by Unity objects, must be in their own file with the same name as the
  interface's name. This is because the magic-code needs a path to the file containing the serialized interface, and
  unfortunately we don't have an API to find the file path for a given type.
  So, for example, if you serialize an interface named `IInterface` it must be in a file named `IInterface.cs` and must
  not contain any other type declaration [see the explanation about the magic code](#the-magic) for more info.
* Drag-and-Drop functionality only works on individual components and not on list/arrays. So to add an item/items,
  you'll need to manually click the plus icon and assign them one-by-one.
* When creating a new `ScriptableObject` instance from the dropdown, it requires 2 undo-operations to cancel the
  modification.
  This is because after the `ScriptableObject` was created and named, the code re-select the object we were previously
  on which adds a new undo operation of "Selection changed".

## Future plans:

* Better modification detection for serialized interfaces that are implemented by Unity objects. Currently we use the
  file's last modified date to know if it has changed, but it has false-positives like when adding/modifying a
  comment in the file.
* Improve dropdown menu - currently the items aren't properly sorted after the first level.
* Add support for renamed classes - A limitation of `SerializeReference` is if the type is renamed, the reference is
  lost. If this tool will add the `MovedFrom` attribute automatically to renamed types, it will prevent it.

## Tested environments:

* Unity 2020.3
* Unity 2021.3

## References:

* https://github.com/mackysoft/Unity-SerializeReferenceExtensions/ - A great library that achieves similar results, but
  lacks the support for a `UnityEngine.Object` references.

## The magic ✨

<details>
<summary>Reveal</summary>

In order to enable support for Unity objects in `SerializeReference` fields, the code locates all types with the
attribute `GenerateWrapperForAttribute` and generates a new class that implements the specified interface, with a
reference to the instance of the class. This new class delegates all the interface's methods and properties to that
instance.

To prevent compilation errors in the generated class when the interface is modified, the code listens to the
`OnPostprocessAllAssets` callback and deletes all generated classes that wrap that interface.

I use the `AssemblyReloadEvents.afterAssemblyReload` callback to listen for successful compilation and generate classes
for all types that don't have a generated class when it is invoked.

It's important to note that since the `OnPostprocessAllAssets` callback is invoked prior to code compilation, reflection
cannot be used to determine if changes were made. Therefore, the code is currently unable to differentiate between when
the interface was actually modified or only when the file contents were modified, such as when adding a comment.

### Example:

For the `StreetPerformer` class:

```csharp
[GenerateWrapperFor(typeof(ISing))]
[AddComponentMenu("Singing related/")]
internal sealed class StreetPerformer : MonoBehaviour, ISing
{
    [SerializeField] private GameObject street;
    public void Sing()
    {
        Debug.Log($"Singing on {street.name}");
    }
    public void StopSinging()
    {
        Debug.Log($"Stop singing on {street.name}");
    }
}
```

The generated class is:

```csharp
using UnityEngine;
using System;
using EditorTools.SerializedReferenceInitializer.Attributes;

[SerializableAttribute()]
[AutoGeneratedWrapperAttribute(typeof(EditorTools.SerializedReferenceInitializer.Example.Implementations.StreetPerformer))]
internal sealed class StreetPerformer_ISing_Wrapper : EditorTools.SerializedReferenceInitializer.Example.ISing
{
    
    [SerializeField()]
    private EditorTools.SerializedReferenceInitializer.Example.Implementations.StreetPerformer _instance;
    
    private StreetPerformer_ISing_Wrapper(EditorTools.SerializedReferenceInitializer.Example.Implementations.StreetPerformer instance)
    {
        this._instance = instance;
    }
    
    public void Sing()
    {
        this._instance.Sing();
    }
    
    public void StopSinging()
    {
        this._instance.StopSinging();
    }
}
```

</details>