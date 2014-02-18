/* Copyright (c) 2012 MoPho' Games
 * All Rights Reserved
 * 
 * Please see the included 'LICENSE.TXT' for usage rights
 * If this asset was downloaded from the Unity Asset Store,
 * you may instead refer to the Unity Asset Store Customer EULA
 * If the asset was NOT purchased or downloaded from the Unity
 * Asset Store and no such 'LICENSE.TXT' is present, you may
 * assume that the software has been pirated.
 * */

using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(USpeaker))]
public class USpeakInspector : Editor
{
	public override void OnInspectorGUI()
	{
		//CheckUndo();

		USpeaker tg = (USpeaker)target;

		EditorGUI.BeginChangeCheck();
		int newCodec = EditorGUILayout.Popup( "Codec", tg.Codec, USpeakCodecManager.Instance.FriendlyNames );
		if( EditorGUI.EndChangeCheck() )
		{
			Undo.RegisterSceneUndo( "Changed Codec" );
			EditorUtility.SetDirty( tg );
			tg.Codec = newCodec;
		}

		EditorGUI.BeginChangeCheck();
		SpeakerMode newSpeakerMode = (SpeakerMode)EditorGUILayout.EnumPopup( "Speaker Mode", tg.SpeakerMode );
		if( EditorGUI.EndChangeCheck() )
		{
			Undo.RegisterSceneUndo( "Changed Speaker Mode" );
			EditorUtility.SetDirty( tg );
			tg.SpeakerMode = newSpeakerMode;
		}

		EditorGUI.BeginChangeCheck();
		BandMode newBandMode = (BandMode)EditorGUILayout.Popup( "Bandwidth Mode", (int)tg.BandwidthMode, new string[] { "Narrow - 8 kHz", "Wide - 16 kHz", "Ultrawide - 32 kHz" } );
		if( (int)tg.BandwidthMode > 2 )
			tg.BandwidthMode = BandMode.Narrow;
		if( EditorGUI.EndChangeCheck() )
		{
			Undo.RegisterSceneUndo( "Changed Bandwidth Mode" );
			EditorUtility.SetDirty( tg );
			tg.BandwidthMode = newBandMode;
		}

		EditorGUI.BeginChangeCheck();
		SendBehavior newSendBehavior = (SendBehavior)EditorGUILayout.Popup( "Sending Mode", (int)tg.SendingMode, new string[] { "Send while recording", "Record then send" } );
		if( EditorGUI.EndChangeCheck() )
		{
			Undo.RegisterSceneUndo( "Changed Sending Mode" );
			EditorUtility.SetDirty( tg );
			tg.SendingMode = newSendBehavior;
		}

		EditorGUI.BeginChangeCheck();
		ThreeDMode new3DMode = (ThreeDMode)EditorGUILayout.Popup( "3D Mode", (int)tg._3DMode, new string[] { "None", "Speaker Pan", "Full 3D" } );
		if( EditorGUI.EndChangeCheck() )
		{
			Undo.RegisterSceneUndo( "Changed 3D Mode" );
			EditorUtility.SetDirty( tg );

			tg._3DMode = new3DMode;
		}

		EditorGUI.BeginChangeCheck();
		bool newUseVad = EditorGUILayout.Toggle( "Volume Activated", tg.UseVAD );
		if( EditorGUI.EndChangeCheck() )
		{
			Undo.RegisterSceneUndo( "Changed Volume Activated" );
			EditorUtility.SetDirty( tg );
			tg.UseVAD = newUseVad;
		}

		EditorGUI.BeginChangeCheck();
		bool newDebugPlayback = EditorGUILayout.Toggle( "Debug Playback", tg.DebugPlayback );
		if( EditorGUI.EndChangeCheck() )
		{
			Undo.RegisterSceneUndo( "Changed Debug Playback" );
			EditorUtility.SetDirty( tg );
			tg.DebugPlayback = newDebugPlayback;
		}

		string sendrate = tg.SendRate + "/sec";
		if( tg.SendRate < 1.0f )
		{
			sendrate = "1 per " + ( 1.0f / tg.SendRate ) + " secs";
		}

		EditorGUI.BeginChangeCheck();
		float newSendRate = EditorGUILayout.FloatField( "Send Rate (" + sendrate + ")", tg.SendRate );
		if( newSendRate <= 0 )
			newSendRate = Mathf.Epsilon;

		if( EditorGUI.EndChangeCheck() )
		{
			Undo.RegisterSceneUndo( "Changed Send Rate" );
			EditorUtility.SetDirty( tg );
			tg.SendRate = newSendRate;
		}

		EditorGUI.BeginChangeCheck();
		bool newAskPermission = EditorGUILayout.Toggle( "Ask for Mic Permission", tg.AskPermission );
		if( EditorGUI.EndChangeCheck() )
		{
			Undo.RegisterSceneUndo( "Changed Ask For Mic Permission" );
			EditorUtility.SetDirty( tg );
			tg.AskPermission = newAskPermission;
		}

		EditorGUI.BeginChangeCheck();
		float newSpeakerVolume = EditorGUILayout.Slider( "Speaker Volume", tg.SpeakerVolume, 0f, 1f );
		if( EditorGUI.EndChangeCheck() )
		{
			Undo.RegisterSceneUndo( "Changed Speaker Volume" );
			EditorUtility.SetDirty( tg );

			tg.SpeakerVolume = newSpeakerVolume;
		}

		EditorGUI.BeginChangeCheck();
		float newVolumeThreshold = EditorGUILayout.Slider( "Volume Threshold", tg.VolumeThreshold, 0.0001f, 0.2f );
		if( EditorGUI.EndChangeCheck() )
		{
			Undo.RegisterSceneUndo( "Changed Volume Threshold" );
			EditorUtility.SetDirty( tg );

			tg.VolumeThreshold = newVolumeThreshold;
		}

		tg.GetInputHandler();

		tg.DrawTalkControllerUI();

		//if( GUI.changed )
		//{
		//    EditorUtility.SetDirty( target );

		//    guiChanged = true;
		//}
	}
}