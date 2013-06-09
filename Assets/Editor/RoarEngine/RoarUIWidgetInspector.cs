using UnityEngine;
using UnityEditor;
using System.Collections;

public abstract class RoarUIWidgetInspector : RoarInspector
{
	protected SerializedProperty customGUISkin;
	protected SerializedProperty depth;
	protected SerializedProperty bounds;
	protected SerializedProperty contentBounds;
	protected SerializedProperty color;
	protected SerializedProperty boundType;
	protected SerializedProperty apearance;
	protected SerializedProperty windowInfo;	
	protected SerializedProperty draggableWindowFullScreen;
	protected SerializedProperty draggableWindowBounds;
	protected SerializedProperty horizontalAlignment;
	protected SerializedProperty verticalAlignment;
	protected SerializedProperty horizontalOffset;
	protected SerializedProperty verticalOffset;
	protected SerializedProperty useScrollView;
	protected SerializedProperty initialContentWidth;
	protected SerializedProperty initialContentHeight;
	protected SerializedProperty alwaysShowHorizontalScrollBar;
	protected SerializedProperty alwaysShowVerticalScrollBar;
	protected SerializedProperty autoEnableOnLogIn;
	protected SerializedProperty autoDisableOnLogout;
	
	protected override void OnEnable()
	{
		base.OnEnable ();
		
		customGUISkin = serializedObject.FindProperty("skin");
		depth = serializedObject.FindProperty("depth");
		bounds = serializedObject.FindProperty("bounds");
		contentBounds = serializedObject.FindProperty("contentBounds");
		boundType = serializedObject.FindProperty("boundType");
		apearance = (serializedObject.FindProperty("apearance"));
		windowInfo = (serializedObject.FindProperty("windowInfo"));
		horizontalAlignment = serializedObject.FindProperty("horizontalAlignment");
		verticalAlignment = serializedObject.FindProperty("verticalAlignment");
		horizontalOffset = serializedObject.FindProperty("horizontalOffset");
		verticalOffset = serializedObject.FindProperty("verticalOffset");
		useScrollView = serializedObject.FindProperty("useScrollView");
		initialContentWidth = serializedObject.FindProperty("initialContentWidth");
		initialContentHeight = serializedObject.FindProperty("initialContentHeight");
		alwaysShowHorizontalScrollBar = serializedObject.FindProperty("alwaysShowHorizontalScrollBar");
		alwaysShowVerticalScrollBar = serializedObject.FindProperty("alwaysShowVerticalScrollBar");
		autoEnableOnLogIn = serializedObject.FindProperty("autoEnableOnLogIn");
		autoDisableOnLogout = serializedObject.FindProperty("autoDisableOnLogout");
	}

	protected override void DrawGUI()
	{
		// customGUISkin
		EditorGUILayout.Space();
		Comment("You can specify custom skin for the user interface. Otherwise, the default skin will be used.");
		EditorGUILayout.PropertyField(customGUISkin);
		
		// rendering properties
		Comment("Widget rendering properties.");
		EditorGUILayout.PropertyField(depth, new GUIContent("Draw Order"));		
		EditorGUILayout.PropertyField(horizontalAlignment);
		EditorGUILayout.PropertyField(horizontalOffset);
		EditorGUILayout.PropertyField(verticalAlignment);
		EditorGUILayout.PropertyField(verticalOffset);
		EditorGUILayout.PropertyField(autoEnableOnLogIn);
		EditorGUILayout.PropertyField(autoDisableOnLogout);
		
		// boundary properties
		Comment("Draw boundary properties.");
		//EditorGUILayout.PropertyField(boundType, new GUIContent("Type"));
		EditorGUILayout.PropertyField(bounds, new GUIContent("Render Bounds"));
		if (bounds.rectValue.width <= 0 || bounds.rectValue.height <= 0)
		{
			EditorGUILayout.HelpBox("Since the render bounds width and/or height is 0, nothing will be visible.", MessageType.Warning);
		}
		EditorGUILayout.PropertyField(contentBounds, new GUIContent("Content Bounds"));
		if (contentBounds.rectValue.width <= 0 || contentBounds.rectValue.height <= 0)
		{
			EditorGUILayout.HelpBox("Since the content bounds width and/or height is 0, nothing will be visible.", MessageType.Warning);
		}
		EditorGUILayout.Space();
		EditorGUILayout.PropertyField(windowInfo, true, null);

		EditorGUILayout.PropertyField(apearance, true, null);
		EditorGUILayout.PropertyField(useScrollView, new GUIContent("Enable Scrolling"));	
		if (useScrollView.boolValue)
		{
//			EditorGUILayout.PropertyField(initialContentWidth);
//			EditorGUILayout.PropertyField(initialContentHeight);
			EditorGUILayout.PropertyField(alwaysShowHorizontalScrollBar, new GUIContent("Always Show Horiz. Bar"));
			EditorGUILayout.PropertyField(alwaysShowVerticalScrollBar, new GUIContent("Always Show Vert. Bar"));
		}
	}
}
