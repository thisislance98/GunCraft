using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

using MoPhoGames.USpeak.Codec;

public class USpeakCodecManagerWindow : EditorWindow
{
	[MenuItem( "Window/USpeak/Codec Manager" )]
	static void Init()
	{
		USpeakCodecManagerWindow window = EditorWindow.GetWindow<USpeakCodecManagerWindow>();
		window.load();
	}

	private USpeakCodecManager mgr;

	private List<ICodec> codecList;
	private List<int> selectedCodec;

	private Type[] codecs;
	private string[] codecNames;

	void load()
	{
		mgr = (USpeakCodecManager)Resources.Load( "CodecManager" );

		if( mgr != null )
		{
			codecList = new List<ICodec>();
			selectedCodec = new List<int>();

			codecs = GetCodecs();
			codecNames = GetCodecNames( codecs );

			for( int i = 0; i < mgr.CodecNames.Length; i++ )
			{
				codecList.Add( (ICodec)System.Activator.CreateInstance( System.Type.GetType( mgr.CodecNames[ i ] ) ) );
				selectedCodec.Add( codecs.ToList().IndexOf( codecList[ i ].GetType() ) );
			}
		}
	}

	void OnGUI()
	{
		if( mgr == null )
		{
			EditorGUILayout.HelpBox( "Missing 'CodecManager' resource. USpeak package may be corrupt, try reimporting", MessageType.Error );
			return;
		}

		EditorGUILayout.HelpBox( "To add a custom codec, click the 'Add' button and choose your codec from the dropdown, then click 'Save' to save your changes", MessageType.Info );

		if( selectedCodec.Count >= 64 )
		{
			EditorGUILayout.HelpBox( "Uh oh - looks like you've hit the maximum codec limit of 64 codecs! You'll need to get rid of some if you wish to add more", MessageType.Warning );
		}

		int remAt = -1;

		for( int i = 0; i < selectedCodec.Count; i++ )
		{
			GUILayout.BeginHorizontal( GUILayout.ExpandWidth( true ) );

			selectedCodec[ i ] = EditorGUILayout.Popup( selectedCodec[ i ], codecNames, GUILayout.ExpandWidth( true ) );
			if( GUILayout.Button( "-", GUILayout.Width( 50f ) ) )
			{
				remAt = i;
			}

			GUILayout.EndHorizontal();
		}

		if( remAt >= 0 )
			selectedCodec.RemoveAt( remAt );

		if( GUILayout.Button( "Add", GUILayout.Width( 50f ) ) )
		{
			if( selectedCodec.Count < 64 )
			{
				if( selectedCodec.Count > 0 )
				{
					selectedCodec.Add( Mathf.Min( codecs.Length - 1, selectedCodec[ selectedCodec.Count - 1 ] + 1 ) );
				}
				else
				{
					selectedCodec.Add( 0 );
				}
			}
		}

		GUILayout.FlexibleSpace();

		if( GUILayout.Button( "Save" ) )
		{
			mgr.CodecNames = new string[ selectedCodec.Count ];
			mgr.FriendlyNames = new string[ selectedCodec.Count ];
			for( int i = 0; i < mgr.CodecNames.Length; i++ )
			{
				mgr.CodecNames[ i ] = codecs[ selectedCodec[ i ] ].AssemblyQualifiedName;
				mgr.FriendlyNames[ i ] = codecs[ selectedCodec[ i ] ].Name;
			}

			EditorUtility.SetDirty( mgr );
		}
	}

	string[] GetCodecNames( Type[] types )
	{
		string[] names = new string[ types.Length ];
		for( int i = 0; i < types.Length; i++ )
		{
			names[ i ] = types[ i ].ToString();
		}

		return names;
	}

	Type[] GetCodecs()
	{
		var type = typeof( ICodec );
		var types = AppDomain.CurrentDomain.GetAssemblies().ToList()
			.SelectMany( s => s.GetTypes() )
			.Where( p => type.IsAssignableFrom( p ) && p != type );

		return types.ToArray();
	}
}