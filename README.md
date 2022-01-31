# Master-Thesis-Visualization-of-Human-Behaviour-in-VR
This project contains the code and example data used in my master thesis **"Visualization of Human Behaviour in VR"**. The resulting logging and analysis tool can be used to track things like VR head and controller position, head orientation, input actions, speech, sentiment, transform positions and custom events. The data can then be analysed through a replay functionality, line graph, point cloud and heat map. For a full list and description of this tools features see the master thesis' (included in github as pdf) implementation section.

## Setup & References
Since the repository contains example logging data with **.wav** files you should install **git large file storage** (https://git-lfs.github.com/) and use git lfs clone if you do not want to authenticate for each large file (see https://stackoverflow.com/questions/42429028/git-lfs-asking-for-passphrase-for-every-tracked-file).

The project will start with errors since some of the packages used in the project do not allow for upload on github. Start unity not in safe mode and add the following packages.

### Assets that need to be added:
- **Csv Serialize v1.0:** https://assetstore.unity.com/packages/tools/integration/csv-serialize-135763
 - in the "CSVSerializer.cs" insert into the method "SetValue":
```
else if (fieldinfo.FieldType == typeof(Guid))
{
	fieldinfo.SetValue(v, Guid.Parse(value));
}
else if (fieldinfo.FieldType == typeof(DateTime))
{
	if (!DateTime.TryParseExact(value, "MM/dd/yyyy HH:mm:ss.fff", CultureInfo.CurrentCulture, DateTimeStyles.RoundtripKind, out var result))
	{
		result = DateTime.Parse(value, CultureInfo.CurrentCulture, DateTimeStyles.RoundtripKind);
	}
	fieldinfo.SetValue(v, result);
}
```
 - and insert at the top of the class
```
using System.Globalization;
```
- **Oculus Integration v31.0:** https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022
- **TextMeshPro v3.0.6** https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.0/license/LICENSE.html
 - this package is already loaded but is missing essential assets. A prompt offering to import these assets will be shown if a scene with a TextMesh Pro element in it is started (e.g. Master-Thesis-Visualization-of-Human-Behaviour-in-VR/Test_Masterthesis_Project/Assets/LogAndAnalysisTool/Scenes/ExpertInterviewDemo.unity) or if a TextMesh Pro element is added to a scene. Import the TMP essentials when the prompt shows up.

### Assets that need to be added for the example scene used for the expert interview:
- **Free Fantasy Medieval Houses and Props Pack v1.3:** https://assetstore.unity.com/packages/3d/environments/fantasy/free-fantasy-medieval-houses-and-props-pack-167010
- **Polygonal Foliage Asset Package v1.1:** https://assetstore.unity.com/packages/3d/environments/polygonal-foliage-asset-package-133037

### Already integrated assets:
- **Heatmap edited implementation from:** https://unitycodemonkey.com/video.php?v=mZzZXfySeFQ
- **OpenXR Plugin v1.2.3** https://docs.unity3d.com/Packages/com.unity.xr.openxr@1.3/license/Third%20Party%20Notices.html
- **XR Interaction Toolkit v1.0.0-pre.5** https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@1.0/license/Third%20Party%20Notices.html

### Loaded when project is opened:
- **OpenXR Plugin v1.2.3** https://docs.unity3d.com/Packages/com.unity.xr.openxr@1.3/license/Third%20Party%20Notices.html
- **XR Interaction Toolkit v1.0.0-pre.5** https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@1.0/license/Third%20Party%20Notices.html
- **XR Plugin Management v4.1.0** https://docs.unity3d.com/Packages/com.unity.xr.management@4.1/license/LICENSE.html
- **Python for Unity v4.0.0-exp.5** https://docs.unity3d.com/Packages/com.unity.scripting.python@4.0/license/Third%20Party%20Notices.html
- If you have problems with the **Python for Unity** asset (https://docs.unity3d.com/Packages/com.unity.scripting.python@4.0/manual/installation.html), e.g. python installation exception, it **can be removed**

## Test_Masterthesis_Project

### Logging and Analysis Tool Integration
With the unity project and the added assets you can build a unity package (https://docs.unity3d.com/2021.2/Documentation/Manual/AssetPackagesCreate.html). If you just want the tool and if you do not have a use for the expert interview scene you can select just the following assets:
- **Csv Serialize**
- **Heatmap**
- **LogAndAnalysisTool**
- **Oculus**
- **SavWav**
- **TextMeshPro**

You can integrate this package into your own project. Be aware that you still need the packages from **"Loaded when project is opened"** in your new project (Open XR Plugin and XR Interaction Toolkit). **Warning !!!** The interaction tool kit is at the time this was written a pre-release package. You have to enable the pre-release packages visibility in the package manager (click gear sign in package manager -> advanced project settings -> check Enable Pre-release Packages) or import by pressing the "+" sign in the unity package manager and importing with the git url "com.unity.xr.interaction.toolkit" (see https://forum.unity.com/threads/where-is-the-xr-interaction-toolkit-package-preview-packages-gone-from-2020-1.891880/).

You can also try to include and export with dependencies:
- **InputSystem.inputsettings.asset**
- **Resources**
- **Samples**
- **XR**

Unfortunately this did not work for me and still required to install the packages from **"Loaded when project is opened"**.

### General Overview
![tool_diagram](https://user-images.githubusercontent.com/14915789/150980919-68e95f9d-d531-41e7-a54b-25244a2da045.png)

The logging and analysis tool components are in the LogAndAnalysisTool "Assets" folder:
 - the ".SpeechToText" directory contains the .net speech to text application
 - the "Prefabs" directory contains the logging and analysis tool prefab (Tool.prefab)
 - the "Python" directory contains the python server for sentiment analysis and NLP

### Tool Prefab
![tool_prefab_diagram](https://user-images.githubusercontent.com/14915789/150981174-2c871b54-8fa2-41aa-a602-8a43a52c14b7.png)




## VRAnalysisTool_LogData
- the "VRAnalysisTool_LogData" directory contains the mock data used for the expert study
	- usage: copy the directory to the persitent path (https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html) of your unity project
	- For the unity project Test_Masterthesis_Project it would be inside "%userprofile%\AppData\LocalLow\<companyname>\<productname>" on windows
	- open the "ExpertInterviewDemo.unity" scene
	- make sure the "Tool.prefab" in the scene is set to "Analysis" mode
	- start the scene
