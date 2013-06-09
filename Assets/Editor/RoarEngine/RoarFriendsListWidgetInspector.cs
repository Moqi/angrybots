using UnityEngine;
using UnityEditor;
using System.Collections;


public class RoarFriendsListWidgetInspector : RoarUIWidgetInspector
{
	
	private SerializedProperty entryBounds;
	private SerializedProperty entrySpacing;
	private SerializedProperty playerIdFormatString;
	private SerializedProperty playerIdFormat;
	private SerializedProperty nameFormatString;
	private SerializedProperty nameFormat;
	private SerializedProperty levelFormatString;
	private SerializedProperty levelFormat;
	
	protected override void OnEnable ()
	{
		base.OnEnable ();
		
		entryBounds = serializedObject.FindProperty("entryBounds");
		entrySpacing = serializedObject.FindProperty("entrySpacing");
		playerIdFormat = serializedObject.FindProperty("playerIdFormat");
		playerIdFormatString = serializedObject.FindProperty("playerIdFormatString");
		nameFormat = serializedObject.FindProperty("nameFormat");
		nameFormatString = serializedObject.FindProperty("nameFormatString");
		levelFormat = serializedObject.FindProperty("levelFormat");
		levelFormatString = serializedObject.FindProperty("levelFormatString");
	}
	
	protected override void DrawGUI()
	{
		base.DrawGUI();
		
		// rendering properties
		Comment("List Spacing");
		EditorGUILayout.PropertyField(entryBounds);
		EditorGUILayout.PropertyField(entrySpacing);
		Comment("Presentation");
		EditorGUILayout.PropertyField(playerIdFormat);
		EditorGUILayout.PropertyField(playerIdFormatString);
		EditorGUILayout.PropertyField(nameFormat);
		EditorGUILayout.PropertyField(nameFormatString);
		EditorGUILayout.PropertyField(levelFormat);
		EditorGUILayout.PropertyField(levelFormatString);
	}
}
