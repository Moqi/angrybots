using UnityEngine;
using UnityEditor;
using System.Collections;


public class RoarShopWidgetInspector : RoarUIWidgetInspector
{
	private SerializedProperty whenToFetch;
	private SerializedProperty howOftenToFetch;
	
	private SerializedProperty shopItemLabelStyle;
	private SerializedProperty shopItemDescriptionStyle;
	private SerializedProperty shopItemCostStyle;
	private SerializedProperty shopItemBuyButtonStyle;
	private SerializedProperty shopItemBounds;
	private SerializedProperty buyButtonBounds;
	private SerializedProperty shopItemSpacing;
	
	protected override void OnEnable ()
	{
		base.OnEnable ();
		
		whenToFetch = serializedObject.FindProperty("whenToFetch");
		howOftenToFetch = serializedObject.FindProperty("howOftenToFetch");
		
		shopItemLabelStyle = serializedObject.FindProperty("shopItemLabelStyle");
		shopItemDescriptionStyle = serializedObject.FindProperty("shopItemDescriptionStyle");
		shopItemCostStyle = serializedObject.FindProperty("shopItemCostStyle");
		shopItemBuyButtonStyle = serializedObject.FindProperty("shopItemBuyButtonStyle");
		shopItemBounds = serializedObject.FindProperty("shopItemBounds");
		buyButtonBounds = serializedObject.FindProperty("buyButtonBounds");
		shopItemSpacing = serializedObject.FindProperty("shopItemSpacing");
	}
	
	protected override void DrawGUI()
	{
		base.DrawGUI();

		// data fetching
		Comment("How often to fetch player statistics from the server.");
		EditorGUILayout.PropertyField(whenToFetch);
		if (whenToFetch.enumValueIndex == 2)
			EditorGUILayout.PropertyField(howOftenToFetch, new GUIContent("How Often (seconds)"));
		
		// shop items
		Comment("Shop item presentation.");
		EditorGUILayout.PropertyField(shopItemLabelStyle);
		EditorGUILayout.PropertyField(shopItemDescriptionStyle);
		EditorGUILayout.PropertyField(shopItemCostStyle);
		EditorGUILayout.PropertyField(shopItemBuyButtonStyle);
		EditorGUILayout.PropertyField(shopItemBounds);
		EditorGUI.indentLevel = 1;
		EditorGUILayout.PropertyField(buyButtonBounds);
		EditorGUI.indentLevel = 0;
		EditorGUILayout.PropertyField(shopItemSpacing);
	}
}
