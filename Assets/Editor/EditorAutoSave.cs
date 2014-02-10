using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class EditorAutoSave {
	
	static private double lastSave = 0f;
	static public EditorAutoSavePreferences prefs;
	static public float minIntervalTime = 60f;

    static EditorAutoSave ()
    {
		EditorApplication.update += Update;
		EditorApplication.playmodeStateChanged += PlaymodeStateChanged;
		lastSave = EditorApplication.timeSinceStartup;
		LoadPrefs();
    }

    static void Update ()
    {
		if(!prefs.enabled)
			return;
		
		// make sure the editor is not playing or about to be
		if (EditorApplication.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isPaused)
			return;
		
		
		if(prefs.autoSaveOnIntervals)
			if(lastSave + prefs.autoSaveIntervals < EditorApplication.timeSinceStartup)
				AutoSave();
    }
	
	static void PlaymodeStateChanged()
	{
		if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying && prefs.autoSaveOnRun)
			AutoSave();
	}
	
	static void LoadPrefs()
	{
		prefs = EditorAutoSavePreferences.Load();
	}
	
	static void AutoSave()
	{
		if(!prefs.enabled || (!prefs.saveScene && !prefs.saveProject))
			return;

		if(prefs.saveScene && EditorApplication.currentScene != "")
			EditorApplication.SaveScene();
		
		if(prefs.saveProject)
			EditorApplication.SaveAssets();
			
		lastSave = EditorApplication.timeSinceStartup;
		
		if(prefs.logSaves)
			Debug.Log("Editor Auto Saving ...");
	}
}
