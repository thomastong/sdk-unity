using DC = Roar.implementation.DataConversion;
using System.Collections;
using System.Collections.Generic;

//TODO: Remove this!
public class Foo : Roar.DomainObjects.IDomainObject
{
	//TODO: Fix thio
	public string value;
	public bool MatchesKey(string s)
	{
		return true;
	}
}

namespace Roar.implementation
{
	public class ItemCache : DataModel<Foo>
	{
		public ItemCache (string name, string url, string node, ArrayList conditions, DC.IXmlToObject<Foo> xmlParser, IRequestSender api, Roar.ILogger logger)
		: base(name,url,node,conditions,xmlParser,api,logger)
		{
		}

		/**
	    * Fetches details about `items` array and adds to item Cache Model
	    */
		public bool AddToCache ( IList<string> items, Roar.RequestCallback cb=null)
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
				if ( ! Has( items [i] ) )
					batch.Add (items [i] );

			return batch;
		}
	}


}

