using DC = Roar.implementation.DataConversion;
using System.Collections;

namespace Roar.implementation
{

public class DataStore
{
  public DataStore(IRoarInternal roar_internal)
  {
    roar_internal_ = roar_internal;
    Properties_   = new DataModel("properties", "user/view", "attribute", null, roar_internal_, new DC.XmlToPropertyHashtable());
    Inventory_    = new DataModel("inventory", "items/list", "item", null, roar_internal_, new DC.XmlToInventoryItemHashtable());
    Shop_         = new DataModel("shop", "shop/list", "shopitem", null, roar_internal_, new DC.XmlToShopItemHashtable());
    Actions_      = new DataModel("tasks", "tasks/list", "task", null, roar_internal_, new DC.XmlToTaskHashtable());
    Gifts_        = new DataModel("gifts", "mail/what_can_i_send", "mailable", null, roar_internal_, null);
    Badges_       = new DataModel("achievements", "user/achievements", "achievement", null, roar_internal_, null);
    Leaderboards_ = new DataModel("leaderboards", "leaderboards/list", "board", null, roar_internal_, null);
    Friends_      = new DataModel("friends", "friends/list", "friend", null, roar_internal_, null);
    Cache_        = new DataModel("cache", "items/view", "item", null, roar_internal_, new DC.XMLToItemHashtable() );
  }

  public void clear(bool x)
  {
    Properties_.clear(x);
    Inventory_.clear(x);
    Shop_.clear(x);
    Actions_.clear(x);
    Gifts_.clear(x);
    Badges_.clear(x);
    Leaderboards_.clear(x);
    Friends_.clear(x);
    Cache_.clear(x);
  }


  public IRoarInternal roar_internal_;
  public DataModel Properties_;
  public DataModel Inventory_;
  public DataModel Shop_;
  public DataModel Actions_;
  public DataModel Gifts_;
  public DataModel Badges_;
  public DataModel Leaderboards_;
  public DataModel Friends_;
  public DataModel Cache_;

  /**
    * Fetches details about `items` array and adds to item Cache Model
    * 
    * @todo move this out of this class.
    */
  public bool addToCache( ArrayList items, Roar.Callback cb=null )
  {
    ArrayList batch = itemsNotInCache( items );

    // Make the call if there are new items to fetch,
    // passing the `batch` list and persisting the Model data (adding)  
    // Returns `true` if items are to be added, `false` if nothing to add
    if (batch.Count>0)
    {
      var keysAsJSON = Roar.Json.ArrayToJSON(batch) as string;
      Hashtable args = new Hashtable();
      args["item_ikeys"] = keysAsJSON;
      Cache_.fetch( cb, args, true);
      return true;
    }
    else return false;
  }


  /**
   * Takes an array of items and returns an new array of any that are 
   * NOT currently in cache.
   * 
   * @todo move this out of this class.
   */
  public ArrayList itemsNotInCache( ArrayList items )
  {
    // First build a list of "new" items to add to cache
    if (!Cache_.hasDataFromServer){ return items.Clone() as ArrayList; }

    var batch = new ArrayList();
    for (int i=0;i<items.Count;i++)
      if (!Cache_.has( (items[i] as string) )) batch.Add( items[i] );

    return batch;
  }
}

}