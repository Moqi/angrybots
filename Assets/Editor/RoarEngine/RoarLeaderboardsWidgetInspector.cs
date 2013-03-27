using UnityEngine;
using UnityEditor;
using System.Collections;


public class RoarLeaderboardsWidgetInspector : RoarUIWidgetInspector
{
	private SerializedProperty whenToFetch;
	private SerializedProperty howOftenToFetch;
	
	private SerializedProperty leaderboardItemBounds;
	private SerializedProperty leaderboardItemSpacing;
	private SerializedProperty leaderboardEntryStyle;
	
	protected override void OnEnable ()
	{
		base.OnEnable ();
		
		whenToFetch = serializedObject.FindProperty("whenToFetch");
		howOftenToFetch = serializedObject.FindProperty("howOftenToFetch");

		leaderboardItemBounds = serializedObject.FindProperty("leaderboardItemBounds");
		leaderboardItemSpacing = serializedObject.FindProperty("leaderboardItemSpacing");
		leaderboardEntryStyle = serializedObject.FindProperty("leaderboardEntryStyle");
	}
	
	protected override void DrawGUI()
	{
		base.DrawGUI();

		// data fetching
		Comment("How often to fetch player statistics from the server.");
		EditorGUILayout.PropertyField(whenToFetch);
		if (whenToFetch.enumValueIndex == 2)
			EditorGUILayout.PropertyField(howOftenToFetch, new GUIContent("How Often (seconds)"));
		
		// rendering properties
		Comment("Leaderboard item presentation.");
		EditorGUILayout.PropertyField(leaderboardItemBounds);
		EditorGUILayout.PropertyField(leaderboardItemSpacing);
		EditorGUILayout.PropertyField(leaderboardEntryStyle);
	}
}
