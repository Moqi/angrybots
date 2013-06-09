using System.Collections;
using System.Collections.Generic;
using Roar.Components;
using UnityEngine;

namespace Roar.implementation.Components
{
	public class Leaderboards : ILeaderboards
	{
		protected IDataStore dataStore;
		protected ILogger logger;
		IWebAPI webapi;
		public int page_size = 5;
		
		protected bool hasBoardList=false;
		protected List<Roar.DomainObjects.LeaderboardInfo> boardList;
		protected Dictionary<
			string,
			IDictionary<int, IList<Roar.DomainObjects.LeaderboardEntry> >
				> boards = new Dictionary<string, IDictionary<int, IList<Roar.DomainObjects.LeaderboardEntry>>>();
		
		public static System.Action<Roar.WebObjects.Leaderboards.ViewResponse> OnLeaderboardFetchCompleted;

		public Leaderboards (IWebAPI webapi, IDataStore dataStore, ILogger logger)
		{
			this.dataStore = dataStore;
			this.logger = logger;
			this.webapi = webapi;
		}

		public void FetchBoardList( Roar.Callback< ILeaderboards > callback )
		{
			Roar.WebObjects.Leaderboards.ListArguments args = new Roar.WebObjects.Leaderboards.ListArguments();
			webapi.leaderboards.list( args, new BoardListFetcherCallback(this,callback) );
		}
		
		class BoardListFetcherCallback : ZWebAPI.Callback<Roar.WebObjects.Leaderboards.ListResponse>
		{
			Leaderboards leaderboards;
			Roar.Callback<ILeaderboards> cb;
		
			public BoardListFetcherCallback( Leaderboards leaderboards, Roar.Callback<ILeaderboards> cb )
			{
				this.leaderboards = leaderboards;
				this.cb = cb;
			}
		
			public void OnError(Roar.RequestResult result)
			{
				//TODO: Handle the error?
			}
		
			public void OnSuccess(Roar.CallbackInfo<Roar.WebObjects.Leaderboards.ListResponse> info )
			{
				leaderboards.boardList = info.data.boards;
				leaderboards.hasBoardList = true;
				if (cb != null)
				{
					cb( new Roar.CallbackInfo<ILeaderboards>(leaderboards,WebAPI.OK,null) );
				}
			}
		}


		public void FetchBoard( string board_id, int page_number, Roar.Callback<ILeaderboards> cb )
		{
			Roar.WebObjects.Leaderboards.ViewArguments args = new Roar.WebObjects.Leaderboards.ViewArguments();
			args.board_id = board_id;
			args.page = page_number;
			args.num_results = page_size;
			webapi.leaderboards.view( args, new BoardFetcherCallback(this,board_id, cb) );
		}
		
		class BoardFetcherCallback : ZWebAPI.Callback<Roar.WebObjects.Leaderboards.ViewResponse>
		{
			Leaderboards leaderboards;
			string board_id;
			Roar.Callback<ILeaderboards> cb;

			public BoardFetcherCallback( Leaderboards leaderboards, string board_id, Roar.Callback<ILeaderboards> cb )
			{
				this.leaderboards = leaderboards;
				this.board_id = board_id;
				this.cb = cb;
			}

			public void OnError(Roar.RequestResult result)
			{
				//TODO: Handle the error?
			}

			public void OnSuccess(Roar.CallbackInfo<Roar.WebObjects.Leaderboards.ViewResponse> info )
			{
				IDictionary<int, IList< DomainObjects.LeaderboardEntry > > board = null;
				leaderboards.boards.TryGetValue(board_id, out board);
				if( board == null )
				{
					board = new Dictionary<int, IList<DomainObjects.LeaderboardEntry> >();
					leaderboards.boards[board_id] = board;
				}
				
				board[info.data.leaderboard_data.page] = info.data.leaderboard_data.entries;
				if (cb != null)
				{
					cb( new Roar.CallbackInfo<ILeaderboards>(leaderboards,WebAPI.OK,null) );
				}
				Leaderboards.OnLeaderboardFetchCompleted(info.data);
			}
	}

		public bool HasBoardList { get { return hasBoardList; } }

		public IList<DomainObjects.LeaderboardInfo> BoardList ()
		{
			return boardList;
		}
		
		//TODO: This should really support paging.
		public IList<Roar.DomainObjects.LeaderboardEntry> GetLeaderboard( string board_id, int page )
		{
			IList<Roar.DomainObjects.LeaderboardEntry> retval=null;
			IDictionary<int, IList<Roar.DomainObjects.LeaderboardEntry> > board;
			boards.TryGetValue(board_id, out board);
			if( board != null )
			{
				board.TryGetValue(page, out retval );
			}
			return retval;
		}

	}
}
