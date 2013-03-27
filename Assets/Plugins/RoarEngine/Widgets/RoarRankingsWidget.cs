using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Roar;
using Roar.DomainObjects;

public class RoarRankingsWidget : RoarUIWidget
{
	public enum WhenToFetch { OnEnable, Once, Occassionally, Manual };
	public WhenToFetch whenToFetch = WhenToFetch.OnEnable;
	public float howOftenToFetch = 60;
	
	public Rect rankingItemBounds = new Rect(0, 0, 450, 50);
	public float rankingItemSpacing = 4;
	public string rankingEntryPlayerRankStyle = "LeaderboardRankingPlayerRank";
	public string rankingEntryPlayerNameStyle = "LeaderboardRankingPlayerName";
	public string rankingEntryPlayerScoreStyle = "LeaderboardRankingPlayerScore";
	
	public string previousButtonLabel = "Previous page";
	public string previousButtonStyle = "DefaultPillboxStyle";
	public string nextButtonLabel = "Next page";
	public string nextButtonStyle = "DefaultPillboxStyle";
	
	public string customDataFormat = "{0}:{1}";
	public string rankFormat = "{0}";
	public string rankStyle = "LeaderboardRankingRank";
	public string valueFormat = "{0}";
	public string valueStyle = "LeaderboardRankingValue";
	
	public float valueWidth =100;
	public float rankColumnWidth = 40;
	public float interColumnSeparators =40;
	public float divideHeight = 20;
	public float sectionHeight = 50;
	public float topSeparation = 5;
	public float footerHeight = 60;
	public float buttonHeight = 30;
	
	public string leaderboardId = string.Empty;
	public string leaderboardName = "";
	public int page = 1;
	
	private Roar.Components.ILeaderboards boards;	
	private bool isFetching;
	private IList<LeaderboardEntry> leaderboard;
	
	protected override void Awake()
	{
		base.Awake();
		ScrollViewContentWidth = rankingItemBounds.width;
	}
	void Reset()
	{
		bounds = new Rect(512,100,512,386);
		subheaderDarkWidth = 150;
		displayName = "Rankings";
	}
	
	protected override void OnEnable()
	{
		base.OnEnable();
		RoarLeaderboardsWidget.OnLeaderboardsFetchedStarted += OnLeaderboardsFetchedStarted;
		RoarLeaderboardsWidget.OnLeaderboardsFetchedComplete += OnLeaderboardsFetchedComplete;
		RoarLeaderboardsWidget.OnLeaderboardSelected += OnLeaderboardSelected;
		
		boards = roar.Leaderboards;
		
		FetchIfRequired();
	}
	
	protected override void OnDisable()
	{
		RoarLeaderboardsWidget.OnLeaderboardsFetchedStarted -= OnLeaderboardsFetchedStarted;
		RoarLeaderboardsWidget.OnLeaderboardsFetchedComplete -= OnLeaderboardsFetchedComplete;
		RoarLeaderboardsWidget.OnLeaderboardSelected -= OnLeaderboardSelected;
	}
	
	void OnLeaderboardsFetchedStarted()
	{}
	
	void OnLeaderboardsFetchedComplete()
	{
		FetchIfRequired();
	}
	
	void OnLeaderboardSelected(string leaderboardId, string name)
	{
		this.leaderboardName = name;
		this.leaderboardId = leaderboardId;
		this.page = 1;
		FetchIfRequired();
	}
	
	void FetchIfRequired()
	{
		subheaderName =leaderboardName;

		if (!string.IsNullOrEmpty(leaderboardId))
		{
			leaderboard = boards.GetLeaderboard(leaderboardId,page);
		}
		
		if (leaderboard == null)
		{
			Fetch();
		}
	}
	
	public void Fetch()
	{
		if (string.IsNullOrEmpty(leaderboardId))
		{
			if (Debug.isDebugBuild)
				Debug.Log("leaderboardId not set!");
			return;
		}
		isFetching = true;
		networkActionInProgress = true;
		boards.FetchBoard( leaderboardId, page, OnRoarFetchLeaderboardComplete );
	}
	
	void OnRoarFetchLeaderboardComplete(Roar.CallbackInfo<Roar.Components.ILeaderboards> info)
	{
		//TODO: Handle errors!
		leaderboard = info.data.GetLeaderboard(leaderboardId, page);
		isFetching = false;
		networkActionInProgress = false;
	}
	
	
	protected override void DrawGUI(int windowId)
	{
		
		if (isFetching)
		{
			GUI.Label(new Rect(0,0,ContentWidth,ContentHeight), "Fetching leaderboard ranking data...", "StatusNormal");
			ScrollViewContentHeight = contentBounds.height;
		}
		else
		{

			GUI.Box(new Rect(0, 0, contentBounds.width, divideHeight), new GUIContent(""), "DefaultSeparationBar");
			Vector2 rankW = GUI.skin.FindStyle("DefaultSeparationBarText").CalcSize(new GUIContent("Rank"));
			GUI.Label(new Rect(interColumnSeparators - rankW.x/2, 0, valueWidth, divideHeight), "RANK", "DefaultSeparationBarText");

			GUI.Label(new Rect(interColumnSeparators*2 +rankW.x, 0, valueWidth, divideHeight), "NAME", "DefaultSeparationBarText");

			float heightSoFar = divideHeight;

			bool requires_refetch = false;

			if(leaderboard != null)
			foreach (LeaderboardEntry item in leaderboard)
			{
				Vector2 rank = GUI.skin.FindStyle("DefaultLightContentText").CalcSize(new GUIContent(item.rank.ToString()));
				Vector2 labSize = GUI.skin.FindStyle("DefaultLightContentText").CalcSize(new GUIContent(item.player_id));

				string prop_string = string.Join(
						"\n",
						item.properties.Select( p => ( string.Format( customDataFormat, p.ikey, p.value ) ) ).ToArray()
						);

				GUI.Box(new Rect(0, heightSoFar, contentBounds.width, sectionHeight), new GUIContent(""), "DefaultHorizontalSection");
				float ySoFar = heightSoFar + topSeparation;

				GUI.Box(new Rect(interColumnSeparators, ySoFar, rank.x, sectionHeight), item.rank.ToString(), "DefaultHeavyContentText");
				GUI.Box(new Rect(2*interColumnSeparators + rank.x, ySoFar, labSize.x, sectionHeight), prop_string, "DefaultHeavyContentText");

				GUI.Label (new Rect(3*interColumnSeparators + rank.x+labSize.x, heightSoFar, valueWidth, labSize.y),  item.value.ToString(), "DefaultHeavyContentText");
				
				if(GUI.Button(new Rect(4*interColumnSeparators + rank.x + labSize.x+valueWidth, ySoFar, valueWidth, buttonHeight), "View Prof", "DefaultButton"))
				{
					GameObject.Find("ProfileViewerWidget").SendMessage("ViewProfile", item.player_id, SendMessageOptions.RequireReceiver);
				}
				heightSoFar += sectionHeight + topSeparation;
			}

			if(leaderboard != null)
			{
				GUI.Box(new Rect(0, heightSoFar, contentBounds.width, footerHeight), new GUIContent(""), "DefaultFooterStyle");

				GUILayout.BeginArea(new Rect(0, heightSoFar, contentBounds.width, footerHeight));
				GUILayout.BeginVertical();
				GUILayout.FlexibleSpace();
				
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				
				if( page==1 ) { GUI.enabled = false; }
				if( GUILayout.Button(previousButtonLabel, previousButtonStyle) )
				{
					page = page - 1;
					requires_refetch = true;
				}
				GUI.enabled = true;
				
				GUILayout.Space(interColumnSeparators);
				if( leaderboard.Count == 0 ) { GUI.enabled = false; }
				if( GUILayout.Button( nextButtonLabel, nextButtonStyle) )
				{
					page = page +1;
					requires_refetch = true;
				}
				GUI.enabled = true;
				GUILayout.Space(interColumnSeparators);
				GUILayout.EndHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.EndVertical();
				GUILayout.EndArea();
				heightSoFar += footerHeight;
			}
			
			if(alwaysShowVerticalScrollBar)
				if(heightSoFar < contentBounds.height)
					ScrollViewContentHeight = contentBounds.height;
				else
					ScrollViewContentHeight = heightSoFar;
			else
				ScrollViewContentHeight = heightSoFar;

			if(requires_refetch) FetchIfRequired();
		}
		
	}

}
