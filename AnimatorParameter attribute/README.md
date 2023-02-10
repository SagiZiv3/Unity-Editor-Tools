# AnimatorParam attribute

## Demo

Example code:

```csharp
internal sealed class Demo : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField, AnimatorParameter(nameof(animator), AnimatorControllerParameterType.Trigger)]
    private string animatorTrigger;
    [SerializeField, AnimatorParameter(nameof(animator), AnimatorControllerParameterType.Bool)]
    private int animatorBool;
    [SerializeField, AnimatorParameter(nameof(animator), AnimatorControllerParameterType.Int)]
    private int animatorInt;
}
```

In Unity's Inspector:

![image](https://user-images.githubusercontent.com/73391391/218213191-bff642d0-5e97-4542-9da8-75a40fceafdf.png)

The animator controller:

![image](https://user-images.githubusercontent.com/73391391/218213685-d880e6be-3b20-4fe0-adfd-4234ff054492.png)

## Description:

Attribute that streamlines the process of selecting an animator parameter from the inspector.

Previous solutions included using a constant at the top of the class, but this was not scalable as it required all
animators to use the same specific name. Alternatively, a serialized `string` parameter was used for the parameter's name,
but this method was prone to errors due to letter casing and other string-related issues. It also had a lower level of
efficiency as it required string comparison when accessing the property.

With this attribute, you have the option to use either a `string` or int `field` to access the property's name or hash-code,
respectively.

## How to use?

In your scripts, add the `AnimatorParameterAttribute` to the fields that represents a parameter in the animator.

The attribute takes the following arguments:

* `animatorName` - The name of the animator field from which the properties are retrieved (it is recommended to use
  the `nameof` expression to avoid mistakes).
* `parameterType` - An `AnimatorControllerParameterType` type argument that determines which type of property this field
  represents.

## Future plans:

* Create a menu item that locates all fields decorated with this attribute and logs to the console any invalid
  fields.  
  For instance, after removing a parameter from the `AnimatorController`, all fields that were previously
  referencing it will either become the first parameter of the same type or will become invalid if there are no
  parameters of that type remaining.

## Tested environments:

* Unity 2020.3
* Unity 2021.3