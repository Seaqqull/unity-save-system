# About Save-System package (`com.seaqqull.save-system`)

## Package contents

The following table describes the package folder structure:

| **Location**           | **Description**                                                         |
| ---------------------- | ----------------------------------------------------------------------- |
| _Documentation~_       | Contains the documentation for the Unity package.                       |
| _Editor_               | Contains utilities for Editor windows and drawers.                      |
| _Runtime_              | Contains core C# APIs for integrating Save-System into your Unity scene.|

<a name="Installation"></a>

## Installation

### Local Installation
You can [add the local](https://docs.unity3d.com/Manual/upm-ui-local.html)
`com.seaqqull.save-system` package (from the repository that you just cloned) to your
project by:

1. Navigating to the menu `Window` -> `Package Manager`.
2. In the package manager window click on the `+` button on the top left of the packages list.
3. Select `Add package from disk...`
4. Navigate into the `com.seaqqull.save-system` folder.
5. Select the `package.json` file.

### Github via Package Manager
1. Navigate to the menu `Window` -> `Package Manager`.
2. In the package manager window click on the `+` button on the top left of the packages list.
3. Select `Add package from git URL...`
4. Enter "https://github.com/Seaqqull/unity-save-system.git?path=/UnitySaveSystem/Assets/com.seaqqull.save-system".
5. Click `Add` button.

## Setup

`Save-System` can be configured in two ways (Note: Custom import/export can only be configured using second way):
- Initialization file
- Creation of GameObject with script


In case if none of steps were performed, or error occurred during initialization via one of the defined steps. `Save-System` will be initialized with the given values:
```json
{
    "DatabasePath": "SaveSystem/",
    "DatabaseFile": "save.dat",
    "ImportType": "Binary",
    "ExportType": "Binary"
}
```

### Via initialization file

Only usable, if first way wasn't done.
1. Create `SaveSystemSetup.json` file inside the directory `Resources/Seaqqull.Save-System/`.
2. Populate file with a given structure:
```json
{
    "DatabasePath": "File/Path/",
    "DatabaseFile": "fileName.format",
    "ImportType": [Index or Name of type],
    "ExportType": [Index or Name of type]
}
```
- `DatabasePath` - contains file folders path (Note: folders are relative to `Application.persistentDataPath`).
- `DatabaseFile` - contains file name with its format (separated by `.`).
- For `ImportType` and `ExportType` fields next values can be assigned:
    * `0` or `Binary`
    * `1` or `Json`

### Via creation of GameObject with script

1. Create `GameObject` inside the firstly-loaded game scene (Usually it is a scene with `Build Index` = 0).
2. Add `SaveManager` script.
3. Check `Dont Destroy On Load`.
4. Assign right values for the fields described inside the [Via initialization file](#Via-initialization-file) section.


#### Custom import/export handlers
- When `Import Type` or `Export Type` is set as `Custom`, you can assign `Import Provider` or `Export Provider` respectively.
    * `Import Provider` is of type `SnapshotsImporter`
    * `Export Provider` is of type `SnapshotsExporter`

- When both `Import Type` and `Export Type` set as `Custom`. It is possible to assign `Processing Provider` which is used to get both `Importer` and `Exporter`.
    * `Processing Provider` is of type `SnapshotsProvider`

 To assign a custom provider/importer/exporter it is required:
 1. Create a dedicated class, inherited from the given class. Depending on the purpose:
    * Importing: `SnapshotsImporter`
    * Exporting: `SnapshotsExporter`
    * Both: `SnapshotsProvider`
 2. Create instance of that class (all them are inherited from ScriptableObject).
 3. Assign to a given field.
