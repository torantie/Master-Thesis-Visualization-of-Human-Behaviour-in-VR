# Master-Thesis-Visualization-of-Human-Behaviour-in-VR
This project contains the code and example data used in my master thesis "Visualization of Human Behaviour in VR".

## Setup & References
Since the repository contains example logging data with .wav files you should install **git large file storage** (https://git-lfs.github.com/) and use git lfs clone if you do not want to authenticate for each large file (see https://stackoverflow.com/questions/42429028/git-lfs-asking-for-passphrase-for-every-tracked-file).

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
- **Oculus Integration v31.0:** https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022

### Assets that need to be added for the example scene used for the expert interview:
- **Free Fantasy Medieval Houses and Props Pack v1.3:** https://assetstore.unity.com/packages/3d/environments/fantasy/free-fantasy-medieval-houses-and-props-pack-167010
- **Polygonal Foliage Asset Package v1.1:** https://assetstore.unity.com/packages/3d/environments/polygonal-foliage-asset-package-133037

### Already integrated assets:
- **Heatmap edited implementation from:** https://unitycodemonkey.com/video.php?v=mZzZXfySeFQ
- **OpenXR Plugin v1.2.3** https://docs.unity3d.com/Packages/com.unity.xr.openxr@1.3/license/Third%20Party%20Notices.html

### Loaded when project is opened:
- **OpenXR Plugin v1.2.3** https://docs.unity3d.com/Packages/com.unity.xr.openxr@1.3/license/Third%20Party%20Notices.html
- **XR Interaction Toolkit v1.0.0-pre.5** https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@1.0/license/Third%20Party%20Notices.html
- **XR Plugin Management v4.1.0** https://docs.unity3d.com/Packages/com.unity.xr.management@4.1/license/LICENSE.html
- **TextMeshPro v3.0.6** https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.0/license/LICENSE.html
- **Python for Unity v4.0.0-exp.5** https://docs.unity3d.com/Packages/com.unity.scripting.python@4.0/license/Third%20Party%20Notices.html
- If you have problems with the **Python for Unity** asset (https://docs.unity3d.com/Packages/com.unity.scripting.python@4.0/manual/installation.html), e.g. python installation exception, it **can be removed**

## Test_Masterthesis_Project

![tool_diagram](https://user-images.githubusercontent.com/14915789/150980919-68e95f9d-d531-41e7-a54b-25244a2da045.png)

The logging and analysis tool components are in the LogAndAnalysisTool "Assets" folder:
 - the ".SpeechToText" directory contains the .net speech to text application
 - the "Prefabs" directory contains the logging and analysis tool prefab (Tool.prefab)
 - the "Python" directory contains the python server for sentiment analysis and NLP

### Tool prefab
![tool_prefab_diagram](https://user-images.githubusercontent.com/14915789/150981174-2c871b54-8fa2-41aa-a602-8a43a52c14b7.png)




## VRAnalysisTool_LogData
- the "VRAnalysisTool_LogData" directory contains the mock data used for the expert study
	- usage: copy the directory to the persitent path (https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html) of your unity project
	- For the unity project Test_Masterthesis_Project it would be inside "%userprofile%\AppData\LocalLow\<companyname>\<productname>" on windows
	- open the "ExpertInterviewDemo.unity" scene
	- make sure the "Tool.prefab" in the scene is set to "Analysis" mode
	- start the scene
