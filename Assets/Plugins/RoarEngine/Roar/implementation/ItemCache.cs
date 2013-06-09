using DC = Roar.DataConversion;
using System.Collections;
using System.Collections.Generic;

//TODO: Remove this!
public class Foo
{
	//TODO: Fix thio
	public string value;
}

namespace Roar.implementation
{
	public interface IItemCache : IDataModel<DomainObjects.ItemPrototype,Roar.WebObjects.Items.ViewResponse>
	{
		bool AddToCache ( IList<string> items, Roar.Callback<IDictionary<string, DomainObjects.ItemPrototype> > cb);
		IList<string> ItemsNotInCache (IList<string> items);
	}
	
	public class ItemCache : DataModel<DomainObjects.ItemPrototype,Roar.WebObjects.Items.ViewResponse>, IItemCache
	{
		public ItemCache (string name, IDomGetter<Roar.WebObjects.Items.ViewResponse> getter, IDomToCache<Roar.WebObjects.Items.ViewResponse,Roar.DomainObjects.ItemPrototype> converter, Roar.ILogger logger)
		: base(name, getter, converter, logger)
		{
		}

		/**
	    * Fetches details about `items` array and adds to item Cache Model
	    */
		public bool AddToCache ( IList<string> items, Roar.Callback<IDictionary<string, DomainObjects.ItemPrototype> > cb)
		{
			IList<string> batch = ItemsNotInCache (items);

			// Make the call if there are new items to fetch,
			// passing the `batch` list and persisting the Model data (adding)
			// Returns `true` if items are to be added, `false` if nothing to add
			if (batch.Count > 0) {
				var keysAsJSON = Roar.Json.ArrayToJSON (batch);
				Hashtable args = new Hashtable ();
				args ["item_ikeys"] = keysAsJSON;
				Fetch (cb, args, true);
				return true;
			} else
				return false;
		}

		/**
	   * Takes an array of items and returns an new array of any that are
	   * NOT currently in cache.
	   */
		public IList<string> ItemsNotInCache (IList<string> items)
		{
			// First build a list of "new" items to add to cache
			if (!HasDataFromServer) {
				return new List<string>( items );
			}

			List<string> batch = new List<string>();
			
			for (int i=0; i<items.Count; i++)
				if ( RawGet( items [i] ) == null )
					batch.Add (items [i] );

			return batch;
		}
	}


}

