using UnityEditor;
using UnityEngine;

public class EditorAutoSaveWindow : EditorWindow {
	
	private EditorAutoSavePreferences preferences;
	private bool saveOnExit = true;
	
	[UnityEditor.MenuItem("Edit/Project Settings/Editor AutoSave")]
	public static void ShowEditorAutoSaveWindow()
	{		
		EditorAutoSaveWindow window = GetWindow<EditorAutoSaveWindow>(true);
		window.minSize = new Vector2(300, 250);
		window.Show();
	}
	
	void OnEnable()
	{
		title = "Editor AutoSave Preferences";
		preferences = EditorAutoSave.prefs;
		saveOnExit = true;
	}
	
	void OnGUI()
	{
		preferences.enabled = EditorGUILayout.Toggle(new GUIContent("Editor AutoSave enabled", "Enable/Disable Editor AutoSave"), preferences.enabled);
		
		var oldEnabled = GUI.enabled;
		GUI.enabled = preferences.enabled;
		
		preferences.autoSaveOnRun = EditorGUILayout.Toggle(new GUIContent("AutoSave on Editor Run", "Enable/Disable AutoSaves when running the game in the Editor."), preferences.autoSaveOnRun);
		preferences.autoSaveOnIntervals = EditorGUILayout.Toggle(new GUIContent("AutoSave on Intervals", "Enable/Disable AutoSaves every " + preferences.autoSaveIntervals + " seconds."), preferences.autoSaveOnIntervals);
		preferences.autoSaveIntervals = EditorGUILayout.IntSlider(new GUIContent("Interval Length", "Minimum frequency to AutoSave in seconds."), (int)preferences.autoSaveIntervals, (int)EditorAutoSave.minIntervalTime, 1800);
		
		preferences.saveScene = EditorGUILayout.Toggle(new GUIContent("AutoSave Saves Scene", "Enable/Disable AutoSaves saving the current scene."), preferences.saveScene);
		preferences.saveProject = EditorGUILayout.Toggle(new GUIContent("AutoSave Saves Project", "Enable/Disable AutoSaves saving the entire project (prefabs, materials, etc.)."), preferences.saveProject);
		preferences.logSaves = EditorGUILayout.Toggle(new GUIContent("Log AutoSaves", "Enable/Disable AutoSaves outputting a log message on each AutoSave."), preferences.logSaves);
		
		GUI.enabled = oldEnabled;
		
		EditorGUILayout.BeginHorizontal();
		
		if(GUILayout.Button("Cancel"))
		{
			saveOnExit = false;
			Close();
		}
		
		if(GUILayout.Button("Save Settings"))
			Close();
		
		EditorGUILayout.EndHorizontal();
	}
	
	void Save()
	{
		preferences.Save();
	}
	
	void OnDestroy()
	{
		if(saveOnExit)
			Save();
	}
}
