using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Xml.Serialization;

[System.Serializable]
public class EditorAutoSavePreferences {

	public bool enabled, // Whether or not to AutoSave at all
				autoSaveOnRun,  // Whether or not AutoSave is triggered when the game is run.
				autoSaveOnIntervals, // Whether or not AutoSave is triggered every at least once every interval
				saveScene, // Whether or not AutoSave saves the current scene
				saveProject, // Whether or not AutoSave saves the project (prefabs and other assets)
				logSaves; // Whether or not to write a log message on saves
	
	public float autoSaveIntervals; // AutoSave interval in seconds
	
	static private string prefsPath = "./EditorAutoSavePreferences.xml"; 
	
	public EditorAutoSavePreferences()
	{
		RestoreDefaults();
	}
	
	public void RestoreDefaults()
	{
		enabled = autoSaveOnRun = autoSaveOnIntervals = saveScene = saveProject = true;
		logSaves = false;
		autoSaveIntervals = 300f;
	}
	
	public void Save()
	{
		autoSaveIntervals = Mathf.Max(autoSaveIntervals, EditorAutoSave.minIntervalTime);
		
		System.IO.Stream fileStream = new System.IO.FileStream(prefsPath, System.IO.FileMode.Create);
		
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(EditorAutoSavePreferences));
		xmlSerializer.Serialize(fileStream, this);
		fileStream.Close();
	}
		
	static public EditorAutoSavePreferences Load()
	{
		System.IO.FileInfo fileInfo = new System.IO.FileInfo(prefsPath);
		if(!fileInfo.Exists)
			return new EditorAutoSavePreferences();
		
		System.IO.Stream fileStream = new System.IO.FileStream(prefsPath, System.IO.FileMode.Open);
		
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(EditorAutoSavePreferences));
		EditorAutoSavePreferences prefs = (EditorAutoSavePreferences)xmlSerializer.Deserialize(fileStream);
		fileStream.Close();
		
		prefs.autoSaveIntervals = Mathf.Max(prefs.autoSaveIntervals, EditorAutoSave.minIntervalTime);
		
		return prefs;
	}
}
