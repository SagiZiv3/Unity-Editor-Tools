# Project Launcher

## Description:

A simple Windows PowerShell script that launches the Unity project that this script is located in.

Upon execution, the script will perform the following actions:

1. Create a new virtual desktop named `Unity_<ProjectName>`. If the virtual desktop already exists, it will switch to it.
2. Open the project's solution file, .sln, with the default application (presumed to be your preferred IDE).
3. Start the Unity executable file.
4. Display the project's directory in the file explorer.

## Dependencies:

This scripts depends on the [PSVirtualDesktop](https://github.com/MScholtes/PSVirtualDesktop) package to open the new
virtual desktop.

To install it, start Windows PowerShell as an administrator, and enter the following
command: `Install-Module VirtualDesktop`.

## Requirements:

All of your Unity editor installations must be stored in a single directory and organized into individual
subdirectories, each labeled with the corresponding editor version. For example:

![image](https://user-images.githubusercontent.com/73391391/218124497-982699f4-437d-44b3-a48e-d6a2de4a0ad1.png)

If you are using Unity Hub to manage all the Unity editors, it should be done automatically.

## How to use?

### First setup:

First, place the script in the directory of your Unity project, which is where the `.sln` file can be found.

Next, you will need to specify the path to the directory containing all of your Unity editor installations.

To find this path, you can either check in Unity Hub preferences:

<img alt="image" src="https://user-images.githubusercontent.com/73391391/218125710-7a28d395-75ca-4c38-bfa1-97eb82233321.png" width="960"/>

Or manually find it by using the "Open file location" context menu in windows:

<img alt="Find installation path" height="540" src="https://user-images.githubusercontent.com/73391391/218144750-03a118a5-f1e1-42a5-9d2d-4dcb94aed812.gif" width="960"/>

After you have retrieved the path, open the script in a text editor application (e.g., Notepad) and paste the path to
line 60:
`$EDITORS_DIRECTORY_PATH = "PASTE YOUR PATH HERE"`

### Usage:

You can manually execute the script by right-clicking it and selecting "Run with PowerShell".

Or, the easier option, create a shortcut that will execute the script:

1. Right click on an empty space on the Desktop (or in the file-explorer).
2. Select New→Shortcut.
3. Paste this
   line: `C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -executionpolicy remotesigned -File <PATH TO PS1 SCRIPT>` (
   Replace `<PATH TO PS1 SCRIPT>` with the absolute path to the script file).
4. Click "Next".
5. Give the shortcut a name.
6. Click "Done".

If you want, you can drag the shortcut to the taskbar to make it easily accessible.

## Future plans:

* Automatically place each program's window on a specific monitor in a multi-monitor setup. It would be more convenient
  to have the IDE open on the secondary monitor and Unity displayed on the primary monitor in full-screen mode.

## Tested environments:

* Windows 10 - Version 22H2