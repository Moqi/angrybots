using UnityEngine;
using UnityEditor;
using System.Collections;


public class RoarInventoryWidgetInspector : RoarUIWidgetInspector
{
	private SerializedProperty whenToFetch;
	private SerializedProperty howOftenToFetch;
	
	private SerializedProperty inventoryLabelFormat;
	private SerializedProperty inventoryDescriptionFormat;
	private SerializedProperty inventoryTypeFormat;
	private SerializedProperty inventoryConsumeButtonStyle;
	private SerializedProperty inventoryMaxLabelWidth;
	private SerializedProperty inventoryMaxDescriptionWidth;
	private SerializedProperty inventoryMaxTypeWidth;
	private SerializedProperty inventoryRowHeight;
	
	protected override void OnEnable ()
	{
		base.OnEnable ();
		
		whenToFetch = serializedObject.FindProperty("whenToFetch");
		howOftenToFetch = serializedObject.FindProperty("howOftenToFetch");

		inventoryLabelFormat = serializedObject.FindProperty("labelFormat");
		inventoryDescriptionFormat = serializedObject.FindProperty("descriptionFormat");
		inventoryTypeFormat = serializedObject.FindProperty("typeFormat");
		inventoryConsumeButtonStyle = serializedObject.FindProperty("consumeButtonFormat");
		inventoryMaxLabelWidth = serializedObject.FindProperty("maxLabelWidth");
		inventoryMaxDescriptionWidth = serializedObject.FindProperty("maxDescriptionFormatWidth");
		inventoryMaxTypeWidth = serializedObject.FindProperty("maxTypeWidth");
		inventoryRowHeight = serializedObject.FindProperty("rowHeight");
	}
	
	protected override void DrawGUI()
	{
		base.DrawGUI();

		// data fetching
		Comment("How often to fetch Inventory from the server.");
		EditorGUILayout.PropertyField(whenToFetch);
		if (whenToFetch.enumValueIndex == 2)
			EditorGUILayout.PropertyField(howOftenToFetch, new GUIContent("How Often (seconds)"));
		
		// rendering properties
		Comment("Inventory presentation.");
		EditorGUILayout.PropertyField(inventoryLabelFormat);
		EditorGUILayout.PropertyField(inventoryDescriptionFormat);
		EditorGUILayout.PropertyField(inventoryTypeFormat);
		EditorGUILayout.PropertyField(inventoryConsumeButtonStyle);
		EditorGUILayout.PropertyField(inventoryMaxLabelWidth);
		EditorGUILayout.PropertyField(inventoryMaxDescriptionWidth);
		EditorGUILayout.PropertyField(inventoryMaxTypeWidth);
		EditorGUILayout.PropertyField(inventoryRowHeight);
	}
}
