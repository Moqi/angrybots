using System.Collections.Generic;
using Roar.Components;
using UnityEngine;

namespace Roar.implementation.Components
{
	public class Achievements : IAchievements
	{
		protected IDataStore dataStore;
		protected ILogger logger;

		public Achievements (IDataStore dataStore, ILogger logger)
		{
			this.dataStore = dataStore;
			this.logger = logger;
		}

		public void Fetch (Roar.Callback< IDictionary<string,Foo> > callback)
		{
			dataStore.achievements.Fetch (callback);
		}

		public bool HasDataFromServer { get { return dataStore.achievements.HasDataFromServer; } }

		public IList<Foo> List ()
		{
			return dataStore.achievements.List ();
		}


		// Returns the achievement Hashtable associated with attribute `ikey`
		public Foo GetAchievement (string ikey)
		{
			return dataStore.achievements.Get (ikey);
		}

	}
}
