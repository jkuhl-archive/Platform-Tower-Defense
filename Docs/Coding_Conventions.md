# Coding Conventions

## General Practices
- All code should be self documenting (as in you know exactly what it's doing from a quick glance). More complex blocks of code should have well placed comments for clarity
- The majority of the code recommendations from Rider / Visual Studio are valid and should be followed
- Rider has a built in 'Auto Cleanup' feature that should be utilized when you are polishing up finalized code, hit 'Ctrl+Alt+Shift+L' to bring up the dialog box for this feature and make sure to check the 'Code cleanup' box

## Variables
- Unchanging variables that need to be accessed from more than one location in the code should be added to a static 'Constants' class, see "Scripts/Data/PlayerConstants.cs" for an example
- Variables and methods should be private and use limited accessors such as get methods unless full public access is required
- The ```[SerializeField]``` tag can be used to make private variables visible within Unity, use this instead of making the variable public unless absolutely neccessary
- Variable names should be human readable whenever possible
  
## Methods
- All methods in a class other than Unity's built in methods (Update(), Start(), etc.) should be sorted alphabetically with private methods at the top, public methods below, and static methods at the bottom
- All methods should have a well written docstring, eg:
```
/// <summary>
///     Gets the GameObject for the given UI element by name
///     Only able to access child GameObjects of this object
/// </summary>
/// <param name="objectName"> Name of the UI element that we want to get </param>
/// <returns> GameObject for the UI element, or null if it cannot be found </returns>
public GameObject GetUiObjectByName(string objectName)
{...}
```