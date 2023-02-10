# Start a new virtual-desktop (Source https://github.com/MScholtes/PSVirtualDesktop)

Import-Module VirtualDesktop

function Request-NamedDesktop {
  <#
		.SYNOPSIS
			Retrieves or creates (if non-existing) the virtual desktop with the given name.
		.INPUTS
			The desktop name can be piped into this function.
		.OUTPUTS
			A virtual desktop with the given name.
		.EXAMPLE
			Request-NamedDesktop "My Secret Desktop"
		.EXAMPLE
			"My Secret Desktop" | Request-NamedDesktop | Switch-Desktop
		.NOTES
			The function assumes that the PSVirtualDesktop module [0] is installed.
			[0]: https://github.com/MScholtes/PSVirtualDesktop
	#>
  param(
  <#
			The name of the virtual desktop to retrieve or create (if non-existing)
		#>
    [Parameter(Mandatory=$true, ValueFromPipeline=$true)]
    [ValidateNotNullOrEmpty()]
    [string]$name
  )

  $desktop = Get-DesktopList | Where-Object Name -eq $name | Select-Object -First 1
  # The if condition below is susceptible to a TOCTOU bug (https://en.wikipedia.org/wiki/Time-of-check_to_time-of-use).
  # But this is probably okay since virtual desktops aren't something that is created every second.
  if ($desktop) {
    Write-Output "Found an existing virtual-desktop"
    Get-Desktop -Index $desktop.Number
  } else {
    Write-Output "Creating new virtual-desktop"
    $desktop = New-Desktop
    $desktop | Set-DesktopName -Name $name
    $desktop
  }
}
# Get the path to the current directory
$UNITY_PROJECT_PATH = (Get-Location).ToString()
Write-Output "Path: $UNITY_PROJECT_PATH"
$SCRIPT_DIRECTORY = Get-Location

# Remove the trailing slash at the end
$SCRIPT_DIRECTORY = $SCRIPT_DIRECTORY.ToString().TrimEnd('\')

Write-Output "Current directory: $SCRIPT_DIRECTORY"

# Get the project version
$UNITY_VERSION = Get-Content -TotalCount 1 "$SCRIPT_DIRECTORY\ProjectSettings\ProjectVersion.txt"
# Extract the version if the string has spaces (e.g, "m_EditorVersion: 2020.3.40f1")
# Remove everything up to and including the first space character,and assign the result to $UNITY_VERSION.
$UNITY_VERSION = $UNITY_VERSION -replace "^[^ ]+ "
Write-Output "Unity version: $UNITY_VERSION"

$EDITORS_DIRECTORY_PATH = "PASTE YOUR PATH HERE"

# Set the path to the Unity editor executable
$UNITY_EXE_PATH = "$EDITORS_DIRECTORY_PATH\$UNITY_VERSION\Editor\Unity.exe"

Write-Output "Unity executable: $UNITY_EXE_PATH"

# Extract the directory name, to get the path to the solution file (it is named after it).
$folder = Split-Path -Leaf $SCRIPT_DIRECTORY
$SOLUTION_FILE = "$SCRIPT_DIRECTORY\$folder.sln"

Write-Output "Solution path: $SOLUTION_FILE"

# Check for common errors
# Check if the Unity project path exists
if (-not [string]::IsNullOrEmpty($UNITY_PROJECT_PATH)) {
  if (-not (Test-Path "$UNITY_PROJECT_PATH")) {
    Write-Output "Could not find Unity project at '$UNITY_PROJECT_PATH'"
    exit 1
  }
} else {
  Write-Output "Unity project path is empty"
  exit 1
}

# Check if the Unity executable path exists
if (-not [string]::IsNullOrEmpty($UNITY_EXE_PATH)) {
  if (-not (Test-Path "$UNITY_EXE_PATH")) {
    Write-Output "Could not find Unity editor at '$UNITY_EXE_PATH'"
    exit 1
  }
} else {
  Write-Output "Unity executable path is empty"
  exit 1
}

# Check if the solution file exists
if (-not [string]::IsNullOrEmpty($SOLUTION_FILE)) {
  if (-not (Test-Path "$SOLUTION_FILE")) {
    Write-Output "Could not find solution file '$SOLUTION_FILE'"
    exit 1
  }
} else {
  Write-Output "Solution file is empty"
  exit 1
}

# Create a new virtual desktop for this project

# Get the parent directory's name
$PROJECT_NAME = Split-Path -Leaf (Split-Path -Parent $SCRIPT_DIRECTORY)
Write-Output "Project name: $PROJECT_NAME"
# Create/find the virtual desktop
$desktop = Request-NamedDesktop -Name "Unity_$PROJECT_NAME"
# Move this console window to the new desktop and switch to it.
$desktop | Move-Window (Get-ConsoleHandle) | Switch-Desktop

# Execute the programs
# Open the project's directoy in File-Explorer
explorer .

# Open the solution file to launch Rider.
Start-Process "$SOLUTION_FILE"

# Launch the Unity project in a separate process
 Start-Process "$UNITY_EXE_PATH" -ArgumentList "-projectPath", "$UNITY_PROJECT_PATH" -NoNewWindow


