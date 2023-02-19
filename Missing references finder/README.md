# Missing References Finder

## Demo

The example object:

![image](https://user-images.githubusercontent.com/73391391/219901724-3b5bbda2-1bee-423f-842c-f81aa331b3bf.png)

The result in the window:

![image](https://user-images.githubusercontent.com/73391391/219901794-7e8ae451-f87f-47aa-a527-12ac921bed21.png)

The script detected two missing references - one to the animator variable and another in the `UnityEvent`.

If we were to fix the issue in the `UnityEvent`, we will get the next issue:

![image](https://user-images.githubusercontent.com/73391391/219901923-394eac61-bcc1-401f-90e8-9e200ef0ff24.png)

The code detected that the third item in the `UnityEvent` doesn't call a valid function.

## Description:

The Missing References Finder is a tool designed for Unity developers to help locate any missing or unassigned
references in their project. It can search for missing references in every script instance on every object, including
and it can also searching inside `UnityEvent`s and detect when a callback doesn't have a target object or when a
referenced method doesn't exist due to deletion, renaming, or a change to a non-public function.

By detecting missing references before they cause problems at runtime, the Missing References Finder can save developers
a lot of debugging time and frustration. Unity does not provide warnings for missing references, and in the
case of UnityEvents, missing references are silently ignored, making it difficult to track down the source of the issue.

Using the Missing References Finder, developers can quickly and easily locate any missing references and resolve them
before they cause issues. This tool can help streamline the development process and ensure that the project is
functioning properly.

## How to use?

To start the search for missing references, open the editor window by navigating to "Utilities" and selecting "Open
missing references window" or "Find missing references." Once the window is open, you'll see the following options:

![image](https://user-images.githubusercontent.com/73391391/219902898-e4718035-cf79-42f1-a830-71cbfdadccc1.png)

1. A "Search" button, which will initiate the search and change to "Refresh" after the initial search has been
   performed.
1. An "Include prefabs" toggle, which should add prefab assets to the search (although this feature may not work
   properly).
1. A list of namespaces to be ignored. Types belonging to any of these namespaces will be excluded from the search.
1. An "Errors" list, which will appear after a search has been performed and will list all the errors that were found.
   From this list, you can easily navigate to the referenced object that generated the error.

## Known bugs/limitations:

* Validation is not performed for namespaces added to the ignored list - any string can be added to the list without
  being validated by the code.
* When searching for missing references on prefabs, the code only shows results for loaded prefab assets.
* Missing support for prefab stages - search results will include objects from the main stage (the root scene) even when
  you are in a prefab stage.

## Future plans:

* Refactor the code - While the code currently works, it's messy and not very scalable. By applying the skills and
  knowledge I have gained since writing it, I want to take it to the next level and improve its readability,
  maintainability, and performance.
* Show all errors for a `UnityEvent` - currently, the code only shows errors for each `UnityEvent` instance, which might
  require multiple refreshes of the search to fix all issues.
* Add support for deep search - Currently, the code only searches at the top level and doesn't look inside arrays,
  lists, or objects. By adding deep search support, we can ensure that all missing references are found, no matter how
  deep they are located.
* Add support for the `SerializeReference` attribute - This will allow us to not only detect when a reference is null
  but also if the referenced instance has been deleted or renamed.
* Improve error messages - Some of the error messages aren't very clear or rely solely on the exception message from
  Unity. By improving these messages, we can make it easier for users to understand what is wrong and how to fix it.
* Perform targeted searches - search directly within the hierarchy of specific objects.
* Error finders selection - select which type of errors you want to find to speed up the search.

## Tested environments:

* Unity 2018.3
* Unity 2019.3
* Unity 2020.3
* Unity 2021.3
