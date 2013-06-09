using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Roar;
using Roar.DomainObjects;

public class RoarLeaderboardsWidget : RoarUIWidget
{
	public delegate void RoarLeaderboardsWidgetHandler();
	public static event RoarLeaderboardsWidgetHandler OnLeaderboardsFetchedStarted;
	public static event RoarLeaderboardsWidgetHandler OnLeaderboardsFetchedComplete;
	
	public delegate void RoarLeaderboardsWidgetSelectedHandler(string leaderboardKey, string leaderboardName);
	public static event RoarLeaderboardsWidgetSelectedHandler OnLeaderboardSelected;
	
	public enum WhenToFetch { OnEnable, Once, Occassionally, Manual };
	public WhenToFetch whenToFetch = WhenToFetch.OnEnable;
	public float howOftenToFetch = 60;

	public Rect leaderboardItemBounds = new Rect(0, 0, 450, 30);
	public float leaderboardItemSpacing = 4;
	public string leaderboardEntryStyle = "DefaultButton";
	
	private bool isFetching;
	private float whenLastFetched;
	private Roar.Components.ILeaderboards boards;	
	private IList<LeaderboardInfo> leaderboards;
	private LeaderboardData activeLeaderboard;
		
	protected override void Awake()
	{
		base.Awake();
		ScrollViewContentWidth = leaderboardItemBounds.width;
	}
	
	void Reset()
	{
		bounds = new Rect(0,100,512,386);
		displayName = "Leaderboard";
		horizontalAlignment = AlignmentHorizontal.Left;
		verticalAlignment = AlignmentVertical.Center;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		
		boards = roar.Leaderboards;
		
		//Do we need to fetch the list of boards?
		if (whenToFetch == WhenToFetch.OnEnable 
		|| (whenToFetch == WhenToFetch.Once && !boards.HasBoardList)
		|| (whenToFetch == WhenToFetch.Occassionally && (whenLastFetched == 0 || Time.realtimeSinceStartup - whenLastFetched >= howOftenToFetch))
		)
		{
			FetchBoardList();
		}
	}
	
	public void FetchBoardList()
	{
		networkActionInProgress = true;

		isFetching = true;
		if (OnLeaderboardsFetchedStarted != null)
			OnLeaderboardsFetchedStarted();
		boards.FetchBoardList(OnRoarFetchLeaderboardsComplete);
	}
	
	void OnRoarFetchLeaderboardsComplete(Roar.CallbackInfo<Roar.Components.ILeaderboards> cache)
	{
		networkActionInProgress = false;
		whenLastFetched = Time.realtimeSinceStartup;
		isFetching = false;
		leaderboards = boards.BoardList();
		
		// set default labels
		foreach (LeaderboardInfo leaderboard in leaderboards)
		{
			if (string.IsNullOrEmpty(leaderboard.label))
				leaderboard.label = string.Format("Leaderboard{0} - {1}", leaderboard.board_id, leaderboard.ikey);
		}
		
		ScrollViewContentHeight = leaderboards.Count * (leaderboardItemBounds.height + leaderboardItemSpacing);
		
		if (OnLeaderboardsFetchedComplete != null)
			OnLeaderboardsFetchedComplete();
	}
	
	protected override void DrawGUI(int windowId)
	{
		
		if (isFetching)
		{
			GUI.Label(new Rect(0,0,ContentWidth,ContentHeight), "Fetching leaderboard data...", "StatusNormal");
			ScrollViewContentHeight = contentBounds.height;
		}
		else
		{
			if (!boards.HasBoardList || leaderboards == null || leaderboards.Count == 0)
			{
				GUI.Label(new Rect(0,0,ContentWidth,ContentHeight), "No leaderboards to display", "StatusNormal");
				ScrollViewContentHeight = contentBounds.height;
			}
			else
			{
				ScrollViewContentHeight = Mathf.Max(contentBounds.height, leaderboards.Count * (leaderboardItemBounds.height + leaderboardItemSpacing));
				Rect entry = leaderboardItemBounds;
				foreach (LeaderboardInfo leaderboard in leaderboards)
				{
					if (GUI.Button(entry, leaderboard.label, leaderboardEntryStyle))
					{
						if (OnLeaderboardSelected != null)
							OnLeaderboardSelected(leaderboard.board_id, leaderboard.ikey);
					}
					entry.y += entry.height + leaderboardItemSpacing;
				}
			}
		}
		
		
	}
}
