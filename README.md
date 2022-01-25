# Master-Thesis-Visualization-of-Human-Behaviour-in-VR

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
