# AnimationCurve Builder

## Demo

Creating a Gaussian curve:

![image](https://user-images.githubusercontent.com/73391391/218320329-78f58d6c-a51b-4a6d-a582-70496d4452d9.png)

Created curve:

<img alt="image" src="https://user-images.githubusercontent.com/73391391/218320390-cf79fc16-2371-46a3-8820-5d5a965b6ad8.png" width="960"/>

Creating a Sine wave curve:

![image](https://user-images.githubusercontent.com/73391391/218320515-47eba422-249a-4a61-9e10-9b1b03982e75.png)

Created curve:

<img alt="image" src="https://user-images.githubusercontent.com/73391391/218320558-47e38c8a-e01f-4891-89ba-7316d1228358.png" width="960"/>

## Description:

A convenient tool that enables you to effortlessly set an AnimationCurve based on a mathematical formula.

A versatile animation tool that simplifies the process of creating customized AnimationCurves.
By utilizing pre-defined mathematical formulas and giving you the ability to add your own, this tool makes it easy to
create complex or simple AnimationCurves with ease.

## How to use?

### Generating a curve

1. Open the Editor window under "Utilities→Animation Curve Builder".
2. Drag your script instance from the inspector to the "Script instance" field.
3. Select the AnimationCurve field you want to initialize.
4. Select the curve type you want.
5. Fill the other fields:
    1. Step size: the distance between each point on the x-axis. More points = smoother curve, but also a more complex
       curve.
    2. Min value: The start x-axis value.
    3. Max value: The end x-axis value.
    4. Precision: Determines the level of accuracy for the x-axis values in the AnimationCurve. It defines
       the number of decimal places to keep in the calculation to prevent calculations errors caused by floating-point
       precision.
6. Fill the fields for the selected curve type.
7. Click "Generate curve".

### Creating new curve types

To create a new curve type simply create a new class that implements the `ICurveType` interface with the following
methods:

* `DisplayName` - A readonly string that will be displayed in the curve types dropdown menu.
* `OnGUI()` - Draws all the GUI elements relevant for this curve type.
* `Evaluate(float value)` - Accepts the value on the x-axis of the AnimationCurve and returns the corresponding value on
  the y-axis.
* `IsValidValue(float value)` - Accepts a value on the x-axis of the AnimationCurve and returns `true` if this value can
  be evaluated for this curve type.  
  For example, a square-root curve type will return `false` for negative values.

## Future plans:

* Improve UI. Spacing and UI layout requires some improvement, especially when displaying the custom CurveType UI.
* Implement a CurveType that allows for a free-form formula input and converts the entered string into a mathematical
  representation of the curve.
* Implement a CurveType that is based on points and generates a curve that passes through all specified points in the
  list.

### Future references:

* https://easings.net/en
* https://gist.github.com/cjddmut/d789b9eb78216998e95c

## Tested environments:

* Unity 2020.3
* Unity 2021.3