using UnityEngine;
using UnityEditor;
using System.Collections;


public class RoarRankingsWidgetInspector : RoarUIWidgetInspector
{
	private SerializedProperty whenToFetch;
	private SerializedProperty howOftenToFetch;
	
	private SerializedProperty rankingItemBounds;
	private SerializedProperty rankingItemSpacing;
	private SerializedProperty rankingEntryPlayerRankStyle;
	private SerializedProperty rankingEntryPlayerNameStyle;
	private SerializedProperty rankingEntryPlayerScoreStyle;
	
	private SerializedProperty customDataFormat;
	private SerializedProperty rankFormat;
	private SerializedProperty rankStyle;
	private SerializedProperty valueFormat;
	private SerializedProperty valueStyle;


	private SerializedProperty previousButtonLabel;
	private SerializedProperty previousButtonStyle;
	private SerializedProperty nextButtonLabel;
	private SerializedProperty nextButtonStyle;

	
	private SerializedProperty leaderboardId;
	private SerializedProperty page;
	
	protected override void OnEnable ()
	{
		base.OnEnable ();
		
		whenToFetch = serializedObject.FindProperty("whenToFetch");
		howOftenToFetch = serializedObject.FindProperty("howOftenToFetch");

		rankingItemBounds = serializedObject.FindProperty("rankingItemBounds");
		rankingItemSpacing = serializedObject.FindProperty("rankingItemSpacing");
		rankingEntryPlayerRankStyle = serializedObject.FindProperty("rankingEntryPlayerRankStyle");
		rankingEntryPlayerNameStyle = serializedObject.FindProperty("rankingEntryPlayerNameStyle");
		rankingEntryPlayerScoreStyle = serializedObject.FindProperty("rankingEntryPlayerScoreStyle");
		
		customDataFormat = serializedObject.FindProperty("customDataFormat");
		rankFormat = serializedObject.FindProperty("rankFormat");
		rankStyle = serializedObject.FindProperty("rankStyle");
		valueFormat = serializedObject.FindProperty("valueFormat");
		valueStyle = serializedObject.FindProperty("valueStyle");

		previousButtonLabel = serializedObject.FindProperty("previousButtonLabel");
		previousButtonStyle = serializedObject.FindProperty("previousButtonStyle");
		nextButtonLabel = serializedObject.FindProperty("nextButtonLabel");
		nextButtonStyle = serializedObject.FindProperty("nextButtonStyle");
		
		leaderboardId = serializedObject.FindProperty("leaderboardId");
		page = serializedObject.FindProperty("page");
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
		Comment("Leaderboard ranking item presentation.");
		EditorGUILayout.PropertyField(rankingItemBounds);
		EditorGUILayout.PropertyField(rankingItemSpacing);
		EditorGUILayout.PropertyField(rankingEntryPlayerRankStyle);
		EditorGUILayout.PropertyField(rankingEntryPlayerNameStyle);
		EditorGUILayout.PropertyField(rankingEntryPlayerScoreStyle);
		
		EditorGUILayout.PropertyField(customDataFormat);
		EditorGUILayout.PropertyField(rankFormat);
		EditorGUILayout.PropertyField(rankStyle);
		EditorGUILayout.PropertyField(valueFormat);
		EditorGUILayout.PropertyField(valueStyle);
		
		Comment ("Paging controls");
		
		EditorGUILayout.PropertyField(previousButtonLabel);
		EditorGUILayout.PropertyField(previousButtonStyle);
		EditorGUILayout.PropertyField(nextButtonLabel);
		EditorGUILayout.PropertyField(nextButtonStyle);
		
		// 
		Comment("Specific leaderboard data.");
		EditorGUILayout.PropertyField(leaderboardId);
		EditorGUILayout.PropertyField(page);
	}
}
