using UnityEngine;
using UnityEditor;
using System.Collections;

public class RoarObjectFactory : Editor
{
	[MenuItem("GameObject/Create Other/Roar/System Object", false, 2000)]
	public static void CreateRoarSceneObject()
	{
		if (ExistingComponentTypeExists(typeof(DefaultRoar)))
		{
			EditorUtility.DisplayDialog("Sorry!", "A DefaultRoar component is already located in this scene.", "OK");
		}
		else
		{
			GameObject go = RoarObjectFactory.CreateGameObjectInScene("Roar");
			go.AddComponent<DefaultRoar>();
			go.transform.parent = null;
			
			Selection.activeGameObject = go;
		}
	}

	[MenuItem("GameObject/Create Other/Roar/Stats Widget", false, 2001)]
	public static void CreateRoarStatsWidgetObject()
	{
		if (!ExistingComponentTypeExists(typeof(DefaultRoar)))
		{
			if (EditorUtility.DisplayDialog("Sorry!", "A DefaultRoar system component cannot be found in this scene. Add one now?", "OK", "Later"))
			{
				CreateRoarSceneObject();
				_CreateRoarStatsWidgetObject();
			}
		}
		else
		{
			_CreateRoarStatsWidgetObject();
		}
	}
	private static void _CreateRoarStatsWidgetObject()
	{
		GameObject go = RoarObjectFactory.CreateGameObjectInScene("RoarStatsWidget");
		go.AddComponent<RoarStatsWidget>();
	
		DefaultRoar defaultRoar = GameObject.FindObjectOfType(typeof(DefaultRoar)) as DefaultRoar;
		go.transform.parent = defaultRoar.transform;		
		
		Selection.activeGameObject = go;
	}
	
	[MenuItem("GameObject/Create Other/Roar/Leaderboard Widget", false, 2002)]
	public static void CreateRoarLeaderboardsWidgetObject()
	{
		if (!ExistingComponentTypeExists(typeof(DefaultRoar)))
		{
			if (EditorUtility.DisplayDialog("Sorry!", "A DefaultRoar system component cannot be found in this scene. Add one now?", "OK", "Later"))
			{
				CreateRoarSceneObject();
				_CreateRoarLeaderboardsWidgetObject();
			}
		}
		else
		{
			_CreateRoarLeaderboardsWidgetObject();
		}
	}
	
	private static void _CreateRoarLeaderboardsWidgetObject()
	{
		GameObject go = RoarObjectFactory.CreateGameObjectInScene("RoarLeaderboardsWidget");
		go.AddComponent<RoarLeaderboardsWidget>();
	
		DefaultRoar defaultRoar = GameObject.FindObjectOfType(typeof(DefaultRoar)) as DefaultRoar;
		go.transform.parent = defaultRoar.transform;		
		
		Selection.activeGameObject = go;
	}
	
	[MenuItem("GameObject/Create Other/Roar/Rankings Widget", false, 2003)]
	public static void CreateRoarRankingsWidgetObject()
	{
		if (!ExistingComponentTypeExists(typeof(DefaultRoar)))
		{
			if (EditorUtility.DisplayDialog("Sorry!", "A DefaultRoar system component cannot be found in this scene. Add one now?", "OK", "Later"))
			{
				CreateRoarSceneObject();
				_CreateRoarRankingsWidgetObject();
			}
		}
		else
		{
			_CreateRoarRankingsWidgetObject();
		}
	}
	
	private static void _CreateRoarRankingsWidgetObject()
	{
		GameObject go = RoarObjectFactory.CreateGameObjectInScene("RoarRankingsWidget");
		go.AddComponent<RoarRankingsWidget>();
	
		DefaultRoar defaultRoar = GameObject.FindObjectOfType(typeof(DefaultRoar)) as DefaultRoar;
		go.transform.parent = defaultRoar.transform;		
		
		Selection.activeGameObject = go;
	}
	
	[MenuItem("GameObject/Create Other/Roar/Shop Widget", false, 2004)]
	public static void CreateRoarShopWidgetObject()
	{
		if (!ExistingComponentTypeExists(typeof(DefaultRoar)))
		{
			if (EditorUtility.DisplayDialog("Sorry!", "A DefaultRoar system component cannot be found in this scene. Add one now?", "OK", "Later"))
			{
				CreateRoarSceneObject();
				_CreateRoarShopWidgetObject();
			}
		}
		else
		{
			_CreateRoarShopWidgetObject();
		}
	}
	
	private static void _CreateRoarShopWidgetObject()
	{
		GameObject go = RoarObjectFactory.CreateGameObjectInScene("RoarShopWidget");
		go.AddComponent<RoarShopWidget>();
	
		DefaultRoar defaultRoar = GameObject.FindObjectOfType(typeof(DefaultRoar)) as DefaultRoar;
		go.transform.parent = defaultRoar.transform;		
		
		Selection.activeGameObject = go;
	}
	
	[MenuItem("GameObject/Create Other/Roar/Friends List Widget", false, 2005)]
	public static void CreateRoarFriendsListWidgetObject()
	{
		if (!ExistingComponentTypeExists(typeof(DefaultRoar)))
		{
			if (EditorUtility.DisplayDialog("Sorry!", "A DefaultRoar system component cannot be found in this scene. Add one now?", "OK", "Later"))
			{
				CreateRoarSceneObject();
				_CreateRoarFriendsListWidgetObject();
			}
		}
		else
		{
			_CreateRoarFriendsListWidgetObject();
		}
	}
	
	private static void _CreateRoarFriendsListWidgetObject()
	{
		GameObject go = RoarObjectFactory.CreateGameObjectInScene("RoarFriendsWidget");
		go.AddComponent<RoarFriendsListWidget>();
	
		DefaultRoar defaultRoar = GameObject.FindObjectOfType(typeof(DefaultRoar)) as DefaultRoar;
		go.transform.parent = defaultRoar.transform;		
		
		Selection.activeGameObject = go;
	}
	
	[MenuItem("GameObject/Create Other/Roar/Inventory Widget", false, 2006)]
	public static void CreateRoarInventoryWidgetObject()
	{
		if (!ExistingComponentTypeExists(typeof(DefaultRoar)))
		{
			if (EditorUtility.DisplayDialog("Sorry!", "A DefaultRoar system component cannot be found in this scene. Add one now?", "OK", "Later"))
			{
				CreateRoarSceneObject();
				_CreateRoarInventoryWidgetObject();
			}
		}
		else
		{
			_CreateRoarInventoryWidgetObject();
		}
	}
	
	private static void _CreateRoarInventoryWidgetObject()
	{
		GameObject go = RoarObjectFactory.CreateGameObjectInScene("RoarInventoryWidget");
		go.AddComponent<RoarInventoryWidget>();
	
		DefaultRoar defaultRoar = GameObject.FindObjectOfType(typeof(DefaultRoar)) as DefaultRoar;
		go.transform.parent = defaultRoar.transform;		
		
		Selection.activeGameObject = go;
	}
	
	[MenuItem("GameObject/Create Other/Roar/Login Widget", false, 2007)]
	public static void CreateRoarLoginWidgetObject()
	{
		if (!ExistingComponentTypeExists(typeof(DefaultRoar)))
		{
			if (EditorUtility.DisplayDialog("Sorry!", "A DefaultRoar system component cannot be found in this scene. Add one now?", "OK", "Later"))
			{
				CreateRoarSceneObject();
				_CreateRoarLoginWidgetObject();
			}
		}
		else
		{
			_CreateRoarLoginWidgetObject();
		}
	}
	
	private static void _CreateRoarLoginWidgetObject()
	{
		GameObject go = RoarObjectFactory.CreateGameObjectInScene("RoarLoginWidget");
		go.AddComponent<RoarLoginWidget>();
	
		DefaultRoar defaultRoar = GameObject.FindObjectOfType(typeof(DefaultRoar)) as DefaultRoar;
		go.transform.parent = defaultRoar.transform;		
		
		Selection.activeGameObject = go;
	}

	[MenuItem("GameObject/Create Other/Roar/Tasks Widget", false, 2008)]
	public static void CreateRoarTasksWidgetObject()
	{
		if (!ExistingComponentTypeExists(typeof(DefaultRoar)))
		{
			if (EditorUtility.DisplayDialog("Sorry!", "A DefaultRoar system component cannot be found in this scene. Add one now?", "OK", "Later"))
			{
				CreateRoarSceneObject();
				_CreateRoarTasksWidgetObject();
			}
		}
		else
		{
			_CreateRoarTasksWidgetObject();
		}
	}
	
	private static void _CreateRoarTasksWidgetObject()
	{
		GameObject go = RoarObjectFactory.CreateGameObjectInScene("RoarTasksWidget");
		go.AddComponent<RoarTasksWidget>();
	
		DefaultRoar defaultRoar = GameObject.FindObjectOfType(typeof(DefaultRoar)) as DefaultRoar;
		go.transform.parent = defaultRoar.transform;		
		
		Selection.activeGameObject = go;
	}
	
	[MenuItem("GameObject/Create Other/Roar/Facebook Shop Widget", false, 2009)]
	public static void CreateRoarFacebookShopWidgetObject()
	{
		if (!ExistingComponentTypeExists(typeof(DefaultRoar)))
		{
			if (EditorUtility.DisplayDialog("Sorry!", "A DefaultRoar system component cannot be found in this scene. Add one now?", "OK", "Later"))
			{
				CreateRoarSceneObject();
				_CreateRoarFacebookShopWidgetObject();
			}
		}
		else
		{
			_CreateRoarFacebookShopWidgetObject();
		}
	}

	private static void _CreateRoarFacebookShopWidgetObject()
	{
		GameObject go = RoarObjectFactory.CreateGameObjectInScene("RoarFacebookShopWidget");
		go.AddComponent<RoarFacebookShopWidget>();

		DefaultRoar defaultRoar = GameObject.FindObjectOfType(typeof(DefaultRoar)) as DefaultRoar;
		go.transform.parent = defaultRoar.transform;		

		Selection.activeGameObject = go;
	}

	[MenuItem("GameObject/Create Other/Roar/Facebook Bind Widget", false, 2010)]
	public static void CreateRoarFacebookBindWidgetObject()
	{
		if (!ExistingComponentTypeExists(typeof(DefaultRoar)))
		{
			if (EditorUtility.DisplayDialog("Sorry!", "A DefaultRoar system component cannot be found in this scene. Add one now?", "OK", "Later"))
			{
				CreateRoarSceneObject();
				_CreateRoarFacebookBindWidgetObject();
			}
		}
		else
		{
			_CreateRoarFacebookBindWidgetObject();
		}
	}

	private static void _CreateRoarFacebookBindWidgetObject()
	{
		GameObject go = RoarObjectFactory.CreateGameObjectInScene("RoarFacebookBindWidget");
		go.AddComponent<RoarFacebookBindWidget>();

		DefaultRoar defaultRoar = GameObject.FindObjectOfType(typeof(DefaultRoar)) as DefaultRoar;
		go.transform.parent = defaultRoar.transform;		

		Selection.activeGameObject = go;
	}
	
	[MenuItem("GameObject/Create Other/Roar/Widget Bar", false, 2011)]
	public static void CreateRoarWidgetBarObject()
	{
		if (!ExistingComponentTypeExists(typeof(DefaultRoar)))
		{
			if (EditorUtility.DisplayDialog("Sorry!", "A DefaultRoar system component cannot be found in this scene. Add one now?", "OK", "Later"))
			{
				CreateRoarSceneObject();
				_CreateRoarWidgetBarObject();
			}
		}
		else
		{
			_CreateRoarWidgetBarObject();
		}
	}

	private static void _CreateRoarWidgetBarObject()
	{
		GameObject go = RoarObjectFactory.CreateGameObjectInScene("RoarWidgetBar");
		go.AddComponent<RoarWidgetBar>();

		DefaultRoar defaultRoar = GameObject.FindObjectOfType(typeof(DefaultRoar)) as DefaultRoar;
		go.transform.parent = defaultRoar.transform;		

		Selection.activeGameObject = go;
	}
	
	[MenuItem("GameObject/Create Other/Roar/Profile Viewer", false, 2012)]
	public static void CreateRoarProfileViewerObject()
	{
		if (!ExistingComponentTypeExists(typeof(DefaultRoar)))
		{
			if (EditorUtility.DisplayDialog("Sorry!", "A DefaultRoar system component cannot be found in this scene. Add one now?", "OK", "Later"))
			{
				CreateRoarSceneObject();
				_CreateRoarProfileViewerObject();
			}
		}
		else
		{
			_CreateRoarProfileViewerObject();
		}
	}

	private static void _CreateRoarProfileViewerObject()
	{
		GameObject go = RoarObjectFactory.CreateGameObjectInScene("ProfileViewerWidget");
		go.AddComponent<RoarProfileViewerWidget>();

		Selection.activeGameObject = go;
	}
	
	[MenuItem("GameObject/Create Other/Roar/Player Picker", false, 2013)]
	public static void CreateRoarPlayerPickerObject()
	{
		if (!ExistingComponentTypeExists(typeof(DefaultRoar)))
		{
			if (EditorUtility.DisplayDialog("Sorry!", "A DefaultRoar system component cannot be found in this scene. Add one now?", "OK", "Later"))
			{
				CreateRoarSceneObject();
				_CreateRoarPlayerPickerObject();
			}
		}
		else
		{
			_CreateRoarPlayerPickerObject();
		}
	}

	private static void _CreateRoarPlayerPickerObject()
	{
		GameObject go = RoarObjectFactory.CreateGameObjectInScene("PlayerSelectionWidget");
		go.AddComponent<RoarPlayerSelectionWidget>();

		Selection.activeGameObject = go;
	}

	public static bool ExistingComponentTypeExists(System.Type type)
	{
		Component c = FindObjectOfType(type) as Component;
		return c != null;
	}
	
	public static GameObject CreateGameObjectInScene(string name)
	{
		string realName = name;
		int counter = 0;
		while (GameObject.Find(realName) != null)
		{
			realName = name + counter++;
		}
		
		GameObject go = new GameObject(realName);
		if (Selection.activeGameObject != null)
		{
			string assetPath = AssetDatabase.GetAssetPath(Selection.activeGameObject);
			if (assetPath.Length == 0) go.transform.parent = Selection.activeGameObject.transform;
		}
		go.transform.localPosition = Vector3.zero;
		go.transform.localRotation = Quaternion.identity;
		go.transform.localScale = Vector3.one;
		return go;
	}
	
}
