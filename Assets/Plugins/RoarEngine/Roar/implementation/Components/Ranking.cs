using System.Collections;
using System.Collections.Generic;
using Roar.Components;
using UnityEngine;

namespace Roar.implementation.Components
{
	public class Ranking : IRanking
	{
		protected string boardId;
		protected int page = 1;

		protected IDataStore dataStore;
		protected ILogger logger;

		public Ranking(string boardId, IDataStore dataStore, ILogger logger)
		{
			this.boardId = boardId;
			this.dataStore = dataStore;
			this.logger = logger;
		}

		public int Page
		{
			set { page = value; }
		}

		public void Fetch(Roar.Callback< IDictionary<string,DomainObjects.LeaderboardData> > callback)
		{
			Hashtable data = new Hashtable();
			data.Add("board_id", boardId);
			data.Add("page", page.ToString());
			dataStore.ranking.Fetch(callback, data);
		}

		public bool HasDataFromServer { get { return dataStore.ranking.HasDataFromServer; } }

		public IList<DomainObjects.LeaderboardData> List()
		{
			return dataStore.ranking.List();
		}

		// Returns the ranking Hashtable associated with attribute `ikey`
		public DomainObjects.LeaderboardData GetEntry( string ikey )
		{
			return dataStore.ranking.Get(ikey);
		}
	}
}
