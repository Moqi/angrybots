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
	public float rankingItemSpacing;
	public string rankingEntryPlayerRankStyle = "LeaderboardRankingPlayerRank";
	public string rankingEntryPlayerNameStyle = "LeaderboardRankingPlayerName";
	public string rankingEntryPlayerScoreStyle = "LeaderboardRankingPlayerScore";
	
	public string previousButtonLabel = "Previous page";
	public string previousButtonStyle = "LeaderboardRankingPrev";
	public string nextButtonLabel = "Next page";
	public string nextButtonStyle = "LeaderboardRankingNext";
	
	public string customDataFormat = "{0}:{1}";
	public string rankFormat = "{0}";
	public string rankStyle = "LeaderboardRankingRank";
	public string valueFormat = "{0}";
	public string valueStyle = "LeaderboardRankingValue";
	
	
	

	//public string rankingNavigatePageValueStyle = "LabelPageValue";
	//public string rankingNavigateLeftButtonStyle = "ButtonNavigatePageLeft";
	//public string rankingNavigateRightButtonStyle = "ButtonNavigatePageRight";
	
	public string leaderboardId = string.Empty;
	public int page = 1;
	
	private Roar.Components.ILeaderboards boards;	
	private bool isFetching;
	private IList<LeaderboardEntry> leaderboard;
	
	protected override void Awake()
	{
		base.Awake();
		ScrollViewContentWidth = rankingItemBounds.width;
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
	
	void OnLeaderboardSelected(string leaderboardId)
	{
		this.leaderboardId = leaderboardId;
		this.page = 1;
		FetchIfRequired();
	}
	
	void FetchIfRequired()
	{
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
			Debug.Log("leaderboardId not set!");
			return;
		}
		isFetching = true;
		boards.FetchBoard( leaderboardId, page, OnRoarFetchLeaderboardComplete );
	}
	
	void OnRoarFetchLeaderboardComplete(Roar.CallbackInfo<Roar.Components.ILeaderboards> info)
	{
		//TODO: Handle errors!
		leaderboard = info.data.GetLeaderboard(leaderboardId, page);
		isFetching = false;
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
			if (leaderboard == null || (leaderboard.Count == 0 && page == 1) )
			{
				GUI.Label(new Rect(0,0,ContentWidth,ContentHeight), "No ranking data.", "StatusNormal");
				ScrollViewContentHeight = contentBounds.height;
			}
			else
			{
				ScrollViewContentHeight = Mathf.Max(contentBounds.height, (leaderboard.Count+1) * (rankingItemBounds.height + rankingItemSpacing));
				//Render some navigation widgets:
				Rect entryRect = rankingItemBounds;
				GUI.BeginGroup(entryRect);
				
				//We do this last so we dont break immediate mode GUI rendering.
				bool requires_refetch = false;
				
				if( page==1 ) { GUI.enabled = false; }
				if( GUI.Button(new Rect(0,0,entryRect.width/2,entryRect.height), previousButtonLabel, previousButtonStyle) )
				{
					page = page - 1;
					requires_refetch = true;
				}
				GUI.enabled = true;
				
				if( leaderboard.Count == 0 ) { GUI.enabled = false; }
				if( GUI.Button(new Rect(entryRect.width/2,0,entryRect.width/2,entryRect.height), nextButtonLabel, nextButtonStyle) )
				{
					page = page +1;
					requires_refetch = true;
				}
				GUI.enabled = true;
				GUI.EndGroup();
				entryRect.y += entryRect.height + rankingItemSpacing;

				
				foreach (LeaderboardEntry leaderboardEntry in leaderboard)
				{
					string prop_string = string.Join(
						"\n",
						leaderboardEntry.properties.Select( p => ( string.Format( customDataFormat, p.ikey, p.value ) ) ).ToArray()
						);
					GUI.Label(entryRect, prop_string, rankingEntryPlayerRankStyle);
					GUI.Label(entryRect, string.Format( rankFormat, leaderboardEntry.rank), rankStyle );
					GUI.Label(entryRect, string.Format( valueFormat, leaderboardEntry.value), valueStyle );
					entryRect.y += entryRect.height + rankingItemSpacing;
				}
				//useScrollView = utilizeScrollView && ((entry.y + entry.height) > contentBounds.height);
				
				if(requires_refetch) FetchIfRequired();

			}
		}
	}
	
	/*
	void GUIPageNavigator(Rect rect)
	{
		GUIStyle navigateButtonStyle;
		float w = rect.width;
		//float h = rect.height;

		GUI.BeginGroup(rect);
		rect.x = 0;
		rect.y = 0;
		
		navigateButtonStyle = skin.FindStyle(rankingNavigateLeftButtonStyle);
		rect.width = navigateButtonStyle.fixedWidth;
		rect.height = navigateButtonStyle.fixedHeight;
		if (activeLeaderboard.HasPrevious)
		{
			if (GUI.Button(rect, string.Empty, rankingNavigateLeftButtonStyle))
			{
				FetchRankings(activeLeaderboard, activeLeaderboard.page + 1);
			}
		}
		
		rect.width = w;
		if (activeLeaderboard.HasPrevious || activeLeaderboard.HasNext)
			GUI.Label(rect, activeLeaderboard.page.ToString(), rankingNavigatePageValueStyle);

		navigateButtonStyle = skin.FindStyle(rankingNavigateRightButtonStyle);
		rect.width = navigateButtonStyle.fixedWidth;
		rect.height = navigateButtonStyle.fixedHeight;
		rect.x = w - rect.width;
		if (activeLeaderboard.HasNext)
		{
			if (GUI.Button(rect, string.Empty, rankingNavigateRightButtonStyle))
			{
				FetchRankings(activeLeaderboard, activeLeaderboard.page - 1);
			}
		}
		
		GUI.EndGroup();
	}
	*/
}
