using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Roar.DomainObjects;

namespace Roar.implementation.DataConversion
{

	//TODO: Should the Exceptions share a common base class?

	//TODO: Not sure this is thre right place for this.
	public class UnexpectedXMLElementException : System.Exception
	{
		public UnexpectedXMLElementException() : base() {}
		public UnexpectedXMLElementException(string message): base(message) {}
		public UnexpectedXMLElementException(string message, System.Exception innerException): base(message, innerException) {}
	};

	//TODO: Not sure this is thre right place for this.
	public class MissingXMLElementException : System.Exception
	{
		public MissingXMLElementException() : base() {}
		public MissingXMLElementException(string message): base(message) {}
		public MissingXMLElementException(string message, System.Exception innerException): base(message, innerException) {}
	};

	//TODO: Not sure this is thre right place for this.
	public class InvalidXMLElementException : System.Exception
	{
		public InvalidXMLElementException() : base() {}
		public InvalidXMLElementException(string message): base(message) {}
		public InvalidXMLElementException(string message, System.Exception innerException): base(message, innerException) {}
	};

	/**
	 * The attribute conversion code in this namespace uses Native.Extract
	 * to convert string representations of booleans to native bools as
	 * it's easier to work with booleans than string representations of booleans.
	 **/
	public class Native
	{
		public static object Extract (string v)
		{
			if (v == "true") {
				return true;
			} else if (v == "false") {
				return false;
			} else {
				return v;
			}
		}
	}

	public interface IXmlToObject<T>
	{
		T Build( IXMLNode n);
		string GetKey( IXMLNode n);
	}

	public interface IXmlToHashtable
	{
		Hashtable BuildHashtable (IXMLNode n);

		string GetKey (IXMLNode n);
	};

	public interface ICRMParser
	{
		ArrayList ParseCostList (IXMLNode n);
		ArrayList ParseModifierList (IXMLNode n);
		ArrayList ParseRequirementList (IXMLNode n);
		ArrayList ParsePropertiesList (IXMLNode n);
		ArrayList ParseChildrenForAttribute (IXMLNode n, string attribute);
	}

	public class CRMParser : ICRMParser
	{
		protected Hashtable AttributesAsHash (IXMLNode n)
		{
			Hashtable h = new Hashtable ();
			foreach (KeyValuePair<string,string> kv in n.Attributes) {
				h [kv.Key] = Native.Extract (kv.Value);
			}
			return h;
		}

		public ArrayList ParseModifierList (IXMLNode n)
		{
			ArrayList modifier_list = new ArrayList ();
			foreach (IXMLNode nn in n.Children) {
				Hashtable hh = AttributesAsHash (nn);
				hh ["name"] = nn.Name;
				modifier_list.Add (hh);
			}
			return modifier_list;
		}

		public ArrayList ParseCostList (IXMLNode n)
		{
			ArrayList cost_list = new ArrayList ();
			foreach (IXMLNode nn in n.Children) {
				Hashtable hh = AttributesAsHash (nn);
				hh ["name"] = nn.Name;
				cost_list.Add (hh);
			}
			return cost_list;
		}

		public ArrayList ParseRequirementList (IXMLNode n)
		{
			ArrayList req_list = new ArrayList ();
			foreach (IXMLNode nn in n.Children) {
				Hashtable hh = AttributesAsHash (nn);
				hh ["name"] = nn.Name;
				req_list.Add (hh);
			}
			return req_list;
		}

		public ArrayList ParsePropertiesList (IXMLNode n)
		{
			ArrayList prop_list = new ArrayList ();
			foreach (IXMLNode nn in n.Children) {
				prop_list.Add (AttributesAsHash (nn));
			}
			return prop_list;
		}

		// for each child node that has a matching attribute, adds the attribute
		// to the list then returns the list
		public ArrayList ParseChildrenForAttribute (IXMLNode n, string attribute)
		{
			ArrayList list = new ArrayList ();
			foreach (IXMLNode nn in n.Children) {
				string attributeValue = nn.GetAttribute (attribute);
				if (attributeValue != null) {
					list.Add (attributeValue);
				}
			}
			return list;
		}

	}

	public interface IXCRMParser
	{
		List<DomainObjects.Cost> ParseCostList (IXMLNode n);
		List<DomainObjects.Modifier> ParseModifierList (IXMLNode n);
		List<DomainObjects.Requirement> ParseRequirementList (IXMLNode n);
		List<string> ParseTagList( IXMLNode n);
	}

	public class XCRMParser : IXCRMParser
	{
		public DomainObjects.Modifier ParseAModifier( IXMLNode n )
		{
			DomainObjects.Modifier retval;
			switch( n.Name )
			{
			case "remove_items":
			{
				DomainObjects.Modifiers.RemoveItems remove_item_mod = new DomainObjects.Modifiers.RemoveItems();
				remove_item_mod.ikey = n.GetAttribute("ikey");
				if( ! System.Int32.TryParse( n.GetAttribute("count"), out remove_item_mod.count ) )
				{
					throw new InvalidXMLElementException("Unable to parse grant_stat count to integer");
				}
				retval = remove_item_mod;
				break;
			}
			case "grant_stat":
			{
				DomainObjects.Modifiers.GrantStat grant_stat_mod = new DomainObjects.Modifiers.GrantStat();
				grant_stat_mod.ikey = n.GetAttribute("ikey");
				if( ! System.Int32.TryParse( n.GetAttribute("value"), out grant_stat_mod.value ) )
				{
					throw new InvalidXMLElementException("Unable to parse grant_stat value to integer");
				}
				retval = grant_stat_mod;
				break;
			}
			case "grant_stat_range":
			{
				DomainObjects.Modifiers.GrantStatRange grant_stat_mod = new DomainObjects.Modifiers.GrantStatRange();
				grant_stat_mod.ikey = n.GetAttribute("ikey");
				if( ! System.Int32.TryParse( n.GetAttribute("min"), out grant_stat_mod.min ) )
				{
					throw new InvalidXMLElementException("Unable to parse grant_stat_range min to integer");
				}
				if( ! System.Int32.TryParse( n.GetAttribute("max"), out grant_stat_mod.max ) )
				{
					throw new InvalidXMLElementException("Unable to parse grant_stat_range max to integer");
				}
				retval = grant_stat_mod;
				break;
			}
			case "grant_item":
			{
				DomainObjects.Modifiers.GrantItem m = new DomainObjects.Modifiers.GrantItem();
				//TODO: Fill me in!
				retval = m;
				break;
			}
			case "grant_xp":
			{
				DomainObjects.Modifiers.GrantXp m = new DomainObjects.Modifiers.GrantXp();
				//TODO: Fill me in!
				retval = m;
				break;
			}
			case "grant_xp_range":
			{
				DomainObjects.Modifiers.GrantXpRange m = new DomainObjects.Modifiers.GrantXpRange();
				//TODO: Fill me in!
				retval = m;
				break;
			}
			case "random_choice":
			{
				DomainObjects.Modifiers.RandomChoice m = new DomainObjects.Modifiers.RandomChoice();
				//TODO: Fill me in!
				retval = m;
				break;
			}
			case "multiple":
			{
				DomainObjects.Modifiers.Multiple m = new DomainObjects.Modifiers.Multiple();
				//TODO: Fill me in!
				retval = m;
				break;
			}
			case "nothing":
			{
				DomainObjects.Modifiers.Nothing m = new DomainObjects.Modifiers.Nothing();
				//TODO: Fill me in!
				retval = m;
				break;
			}
			case "named":
			{
				DomainObjects.Modifiers.NamedReference m = new DomainObjects.Modifiers.NamedReference();
				//TODO: Fill me in!
				retval = m;
				break;
			}
			case "if_then_else":
			{
				DomainObjects.Modifiers.IfThenElse m = new DomainObjects.Modifiers.IfThenElse();
				//TODO: Fill me in!
				retval = m;
				break;
			}
			default:
				throw new InvalidXMLElementException("Invalid modifier type : "+n.Name);

			}
			return retval;
		}

		public List<DomainObjects.Modifier> ParseModifierList (IXMLNode n)
		{
			List<DomainObjects.Modifier> modifier_list = new List<DomainObjects.Modifier> ();
			foreach (IXMLNode nn in n.Children)
			{
				modifier_list.Add( ParseAModifier(nn) );
			}
			return modifier_list;
		}

		public DomainObjects.Cost ParseACost( IXMLNode n )
		{
			DomainObjects.Cost retval;
			//TODO: Implement the rest!
			switch( n.Name )
			{
			case "stat_cost":
				retval = new DomainObjects.Costs.Stat();
				break;
			default:
				throw new UnexpectedXMLElementException("Invalid cost type, \""+n.Name+"\"");
			}

			retval.ok = (n.GetAttribute("ok")=="true");
			retval.reason = n.GetAttribute("reason");
			return retval;
		}

		public List<DomainObjects.Cost> ParseCostList (IXMLNode n)
		{
			List<DomainObjects.Cost> cost_list = new List<DomainObjects.Cost> ();
			foreach (IXMLNode nn in n.Children)
			{
				cost_list.Add ( ParseACost(nn) );
			}
			return cost_list;
		}

		public DomainObjects.Requirement ParseARequirement( IXMLNode n )
		{
			DomainObjects.Requirement retval;

			switch( n.Name )
			{
			case "true":
				retval = new DomainObjects.Requirements.True();
				break;
			case "false":
				retval = new DomainObjects.Requirements.False();
				break;
			case "level":
				DomainObjects.Requirements.Level level_req = new DomainObjects.Requirements.Level();
				if( ! System.Int32.TryParse( n.GetAttribute("level"), out level_req.level) )
				{
					throw new InvalidXMLElementException("Unable to parse level restriction to integer");
				} ;
				retval = level_req;
				break;
			case "item":
				DomainObjects.Requirements.Item item_req = new DomainObjects.Requirements.Item();
				item_req.ikey = n.GetAttribute("ikey");
				if( ! System.Int32.TryParse( n.GetAttribute("number_required"), out item_req.number_required ) )
				{
					throw new InvalidXMLElementException("Unable to parse item count to integer");
				}
				retval = item_req;
				break;
			case "stat":
				DomainObjects.Requirements.Stat stat_req = new DomainObjects.Requirements.Stat();
				stat_req.ikey = n.GetAttribute("ikey");
				if( ! System.Int32.TryParse( n.GetAttribute("value"), out stat_req.value ) )
				{
					throw new InvalidXMLElementException("Unable to parse value in stat requirement");
				}
				retval = stat_req;
				break;
			case "multiple":
				DomainObjects.Requirements.Multiple multiple_req = new DomainObjects.Requirements.Multiple();
				multiple_req.requirements = ParseRequirementList(n);
				retval = multiple_req;
				break;
			default:
				throw new InvalidXMLElementException("Invalid requirement type : "+n.Name);
			}

			return retval;
		}

		public List<DomainObjects.Requirement> ParseRequirementList (IXMLNode n)
		{
			List<DomainObjects.Requirement> req_list = new List<DomainObjects.Requirement> ();
			foreach (IXMLNode nn in n.Children)
			{
				req_list.Add ( ParseARequirement(nn) );
			}
			return req_list;
		}

		public List<string> ParseTagList( IXMLNode n)
		{
			List<string> tags = new List<string>();
			foreach (IXMLNode nn in n.Children)
			{
				tags.Add( nn.GetAttribute("value") );
			}
			return tags;
		}

	}


	public class XmlToTaskHashtable : IXmlToHashtable
	{
		public ICRMParser CrmParser_;

		public XmlToTaskHashtable ()
		{
			CrmParser_ = new CRMParser ();
		}

		public string GetKey (IXMLNode n)
		{
			return n.GetAttribute ("ikey");
		}

		public Hashtable BuildHashtable (IXMLNode n)
		{
			Hashtable retval = new Hashtable ();
			retval ["ikey"] = n.GetAttribute ("ikey");

			foreach (IXMLNode nn in n.Children) {
				switch (nn.Name) {
				case "location":
					break;
				case "mastery_level":
					break;
				case "rewards":
					retval ["rewards"] = CrmParser_.ParseModifierList (nn);
					break;
				case "costs":
					retval ["costs"] = CrmParser_.ParseCostList (nn);
					break;
				case "requires":
					retval ["requires"] = CrmParser_.ParseRequirementList (nn);
					break;
				case "tags":
					retval ["tags"] = CrmParser_.ParseChildrenForAttribute (nn, "value");
					break;
				case "properties":
					retval ["properties"] = CrmParser_.ParsePropertiesList (nn);
					break;
				default:
					retval [nn.Name] = nn.Text;
					break;
				}
			}

			return retval;
		}

	}

	public class XmlToInventoryItemHashtable : IXmlToHashtable
	{
		public ICRMParser CrmParser_;

		public XmlToInventoryItemHashtable ()
		{
			CrmParser_ = new CRMParser ();
		}

		public virtual string GetKey (IXMLNode n)
		{
			return n.GetAttribute ("id");
		}

		public Hashtable BuildHashtable (IXMLNode n)
		{
			Hashtable retval = new Hashtable ();
			foreach (KeyValuePair<string,string> kv in n.Attributes) {
				retval [kv.Key] = Native.Extract (kv.Value);
			}

			foreach (IXMLNode nn in n.Children) {
				switch (nn.Name) {
				case "price":
					retval ["price"] = CrmParser_.ParseModifierList (nn);
					break;
				case "tags":
					retval ["tags"] = CrmParser_.ParseChildrenForAttribute (nn, "value");
					break;
				case "properties":
					retval ["properties"] = CrmParser_.ParsePropertiesList (nn);
					break;
				default:
					retval [nn.Name] = nn.Text;
					break;
				}
			}

			return retval;
		}

	}

	public class XmlToShopEntry : IXmlToObject<DomainObjects.ShopEntry>
	{
		public IXCRMParser CrmParser_;

		public XmlToShopEntry ()
		{
			CrmParser_ = new XCRMParser ();
		}

		public string GetKey (IXMLNode n)
		{
			return n.GetAttribute ("ikey");
		}

		public ShopEntry Build(IXMLNode n)
		{
			ShopEntry retval = new ShopEntry ();
			IEnumerable<KeyValuePair<string,string>> attributes = n.Attributes;
			foreach (KeyValuePair<string,string> kv in attributes)
			{
				if (kv.Key == "ikey")
				{
					retval.ikey = kv.Value;
				}
				else if (kv.Key == "label")
				{
					retval.label = kv.Value;
				}
				else if (kv.Key == "description")
				{
					retval.description = kv.Value;
				}
				else
				{
					throw new UnexpectedXMLElementException("unexpected attribute, \""+kv.Key+"\", on ShopEntry");
				}
			}

			if (retval.ikey==null) throw new MissingXMLElementException("missing attribute, \"ikey\", on ShopEntry");
			if (retval.label==null) { retval.label=""; }
			if (retval.description==null) { retval.description=""; }

			retval.costs = new List<Cost>();
			retval.modifiers = new List<Modifier>();

			foreach (IXMLNode nn in n.Children) {
				switch (nn.Name) {
				case "costs":
					retval.costs = CrmParser_.ParseCostList (nn);
					break;
				case "modifiers":
					retval.modifiers = CrmParser_.ParseModifierList( nn );
					break;
				case "requirements":
					retval.requirements = CrmParser_.ParseRequirementList( nn );
					break;
				case "tags":
					retval.tags = CrmParser_.ParseTagList(nn);
					break;
				case "properties":
					// retval.properties = CrmParser_.ParsePropertiesList (nn);
					break;
				default:
					throw new UnexpectedXMLElementException("unexpected element, \""+nn.Name+"\", in ShopEntry");
					//retval [nn.Name] = nn.Text;
				}
			}

			return retval;
		}

	}

	public class XmlToPropertyHashtable : IXmlToHashtable
	{
		public XmlToPropertyHashtable ()
		{
		}

		public string GetKey (IXMLNode n)
		{
			string key = n.GetAttribute ("ikey");
			if (key != null)
				return key;
			return n.GetAttribute ("name");
		}

		public Hashtable BuildHashtable (IXMLNode n)
		{
			Hashtable retval = new Hashtable ();
			foreach (KeyValuePair<string,string> kv in n.Attributes) {
				retval [kv.Key] = Native.Extract (kv.Value);
			}
			return retval;
		}
	}

	public class XMLToItemHashtable : XmlToInventoryItemHashtable
	{
		public override string GetKey (IXMLNode n)
		{
			return n.GetAttribute ("ikey");
		}
	}

	public class XmlToAchievementHashtable : IXmlToHashtable
	{

		public XmlToAchievementHashtable ()
		{
		}

		public string GetKey (IXMLNode n)
		{
			return n.GetAttribute ("ikey");
		}

		public Hashtable BuildHashtable (IXMLNode n)
		{
			Hashtable retval = new Hashtable ();
			foreach (KeyValuePair<string,string> kv in n.Attributes) {
				retval [kv.Key] = Native.Extract (kv.Value);
			}
			return retval;
		}
	}

	public class XmlToLeaderboardsHashtable : IXmlToHashtable
	{

		public XmlToLeaderboardsHashtable ()
		{
		}

		public string GetKey (IXMLNode n)
		{
			return n.GetAttribute ("ikey");
		}

		public Hashtable BuildHashtable (IXMLNode n)
		{
			Hashtable retval = new Hashtable ();
			foreach (KeyValuePair<string,string> kv in n.Attributes) {
				retval [kv.Key] = Native.Extract (kv.Value);
			}
			return retval;
		}
	}

	public class XmlToRankingHashtable : IXmlToHashtable
	{

		public XmlToRankingHashtable ()
		{
		}

		public string GetKey (IXMLNode n)
		{
			return n.GetAttribute ("ikey");
		}

		public Hashtable BuildHashtable (IXMLNode n)
		{
			Hashtable retval = new Hashtable ();
			Hashtable properties = new Hashtable ();
			ArrayList entries = new ArrayList();
			foreach (KeyValuePair<string,string> kv in n.Attributes)
			{
				//Debug.Log (string.Format ("BuildHashtable: {0} => {1}", kv.Key, kv.Value));
				properties [kv.Key] = Native.Extract (kv.Value);
			}
			foreach (IXMLNode child in n.Children)
			{
				Hashtable entry = new Hashtable();
				foreach (KeyValuePair<string,string> kv in child.Attributes)
				{
					//Debug.Log (string.Format ("BuildHashtable: {0} => {1}", kv.Key, kv.Value));
					entry[kv.Key] = Native.Extract(kv.Value);

					// any custom data? need the player_name if it's there
					foreach (IXMLNode subChild in child.Children)
					{
						if (subChild.Name == "custom")
						{
							foreach (IXMLNode propertyNode in subChild.Children)
							{
								string propertyKey = string.Empty, propertyValue = string.Empty;
								foreach (KeyValuePair<string,string> kvp in propertyNode.Attributes)
								{
									if (kvp.Key == "ikey")
										propertyKey = kvp.Value;
									else if (kvp.Key == "value")
										propertyValue = kvp.Value;
								}
								if (propertyKey.Length > 0)
									entry[propertyKey] = propertyValue;
							}
						}
					}
				}
				entries.Add(entry);
			}
			retval.Add("properties", properties);
			retval.Add("entries", entries);
			return retval;
		}
	}

	public class XmlToAppstoreItemHashtable : IXmlToHashtable
	{
		public ICRMParser CrmParser_;

		public XmlToAppstoreItemHashtable ()
		{
			CrmParser_ = new CRMParser ();
		}

		public string GetKey (IXMLNode n)
		{
			return n.GetAttribute ("product_identifier");
		}

		public Hashtable BuildHashtable (IXMLNode n)
		{
			Hashtable retval = new Hashtable ();
			foreach (KeyValuePair<string,string> kv in n.Attributes) {
				retval [kv.Key] = Native.Extract (kv.Value);
			}

			foreach (IXMLNode nn in n.Children) {
				switch (nn.Name) {
				case "modifiers":
					retval ["modifiers"] = CrmParser_.ParseModifierList (nn);
					break;
				default:
					retval [nn.Name] = nn.Text;
					break;
				}
			}
			return retval;
		}
	}

	public class XmlToGiftHashtable : IXmlToHashtable
	{
		public ICRMParser CrmParser_;

		public XmlToGiftHashtable ()
		{
			CrmParser_ = new CRMParser ();
		}

		public string GetKey (IXMLNode n)
		{
			return n.GetAttribute ("id");
		}

		public Hashtable BuildHashtable (IXMLNode n)
		{
			Hashtable retval = new Hashtable ();
			foreach (KeyValuePair<string,string> kv in n.Attributes) {
				retval [kv.Key] = Native.Extract (kv.Value);
			}

			foreach (IXMLNode nn in n.Children) {
				switch (nn.Name) {
				case "costs":
					retval ["costs"] = CrmParser_.ParseCostList (nn);
					break;
				case "requirements":
					retval ["requirements"] = CrmParser_.ParseRequirementList (nn);
					break;
				default:
					retval [nn.Name] = nn.Text;
					break;
				}
			}
			return retval;
		}
	}
}
