For a new project:
 - drag the Tool.prefab from the prefabs folder into your scene
 - if you want to record data select logging at the top level of the prefab
	- in the scripts folder is a DemoLogging.cs script showing how to start logging or log custom events
 - if you want to analyze data select analysis at the top level of the prefab
 	- other XR cameras will be disabled when this mode is selected. The XR camera of the analysis manager will be used.

Logging setup:
 - HMD Logger: pull in the camera transform you want to track
 - Gaze Logger: TODO
 - Left Controller Logger: pull in the left controller transform you want to track
 - Right Controller Logger: pull in the right controller transform you want to track
 - Transform Logger: Lists all objects with a TrackableTransform.cs script attached that is in the unity scene.
 - Audio Logger: The maximum length of a saved audio clip can be configured.
 - InputActionEventLogger: Add an input action subscription. Select the input action from the action map you are using that you want to track (e.g. Teleport Select).
 Select the transform thats executing the action to track the position when an input action is triggered.
 - CustomEventLogger: Nothing to configure.

AnalysisManager:
 - The AnalysisManager.cs provides a button in the unity interface to caculate the position of audio events for the currently selected study
 if audio events were calculated with the speech to text tool (see post processing)


Post Processing:
 - python server: start the WebServer.py for the NLP and Sentiment calculation. The unity application will send requests with audio event data on startup to the server.
 The server responds with the sentiment and NLP calculations. Just the sentiment events are saved.
 - .net console application: start the console application by running Program.cs. Insert the directory path of the study you want to calculate audio events for.