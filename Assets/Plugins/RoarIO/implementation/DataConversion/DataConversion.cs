using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roar.implementation.DataConversion
{

	public interface IXmlToHashtable
	{
		Hashtable BuildHashtable(IXMLNode n);
		string GetKey(IXMLNode n);
	};
	
	public interface ICRMParser
	{
		ArrayList ParseCostList( IXMLNode n );
		ArrayList ParseModifierList( IXMLNode n );
		ArrayList ParseRequirementList( IXMLNode n );
	}
	
	public class CRMParser : ICRMParser
	{
		protected Hashtable AttributesAsHash( IXMLNode n )
		{
			Hashtable h = new Hashtable();
			foreach( KeyValuePair<string,string> kv in n.Attributes )
			{
				h[kv.Key] = kv.Value;
			}
			return h;
		}
		
		public ArrayList ParseModifierList( IXMLNode n )
		{
			ArrayList modifier_list = new ArrayList();
			foreach( IXMLNode nn in n.Children )
			{
				Hashtable hh = AttributesAsHash(nn);
				hh["name"] = nn.Name;
				modifier_list.Add( hh );
			}
			return modifier_list;
		}
		
		public ArrayList ParseCostList( IXMLNode n )
		{
			ArrayList cost_list = new ArrayList();
			foreach( IXMLNode nn in n.Children )
			{
				Hashtable hh = AttributesAsHash(nn);
				hh["name"] = nn.Name;
				cost_list.Add( hh );
			}
			return cost_list;
		}
		
		public ArrayList ParseRequirementList( IXMLNode n )
		{
			ArrayList req_list = new ArrayList();
			foreach( IXMLNode nn in n.Children )
			{
				Hashtable hh = AttributesAsHash(nn);
				hh["name"] = nn.Name;
				req_list.Add( hh );
			}
			return req_list;
		}
		
	}
	
	public class XmlToTaskHashtable : IXmlToHashtable
	{
		public ICRMParser CrmParser_;
		
		public XmlToTaskHashtable()
		{
			CrmParser_ = new CRMParser();
		}
		
		public string GetKey( IXMLNode n )
		{
			return n.GetAttribute("ikey");
		}
		

		
		public Hashtable BuildHashtable( IXMLNode n )
		{
			Hashtable retval = new Hashtable();
			retval["ikey"] = n.GetAttribute("ikey");
			
			foreach( IXMLNode nn in n.Children )
			{
				switch( nn.Name )
				{
				case "location":
					break;
				case "mastery_level":
					break;
				case "rewards":
					retval["rewards"] = CrmParser_.ParseModifierList( nn );
					break;
				case "costs":
					retval["costs"] = CrmParser_.ParseCostList( nn );
					break;
				case "requires":
					retval["requires"] = CrmParser_.ParseRequirementList( nn );
					break;
				case "tags":
					break;
				default:
					retval[nn.Name] = nn.Text;
					break;
				}
			}
			
			return retval;
		}
		
	}

	public class XmlToInventoryItemHashtable : IXmlToHashtable
	{
		public ICRMParser CrmParser_;
		
		public XmlToInventoryItemHashtable()
		{
			CrmParser_ = new CRMParser();
		}
		
		public virtual string GetKey( IXMLNode n )
		{
			return n.GetAttribute("id");
		}
		
		public Hashtable BuildHashtable( IXMLNode n )
		{
			Hashtable retval = new Hashtable();
			foreach( KeyValuePair<string,string> kv in n.Attributes )
			{
				retval[kv.Key] = kv.Value;
			}
			
			foreach( IXMLNode nn in n.Children )
			{
				switch( nn.Name )
				{
				case "price":
					retval["price"] = CrmParser_.ParseModifierList( nn );
					break;
				default:
					retval[nn.Name] = nn.Text;
					break;
				}
			}
			
			return retval;
		}
		
	}
	
	public class XmlToShopItemHashtable : IXmlToHashtable
	{
		public ICRMParser CrmParser_;
		
		public XmlToShopItemHashtable()
		{
			CrmParser_ = new CRMParser();
		}
		
		public string GetKey( IXMLNode n )
		{
			return n.GetAttribute("ikey");
		}
		
		public Hashtable BuildHashtable( IXMLNode n )
		{
			Hashtable retval = new Hashtable();
			foreach( KeyValuePair<string,string> kv in n.Attributes )
			{
				//We move the ikey to shop_ikey
				if( kv.Key == "ikey")
				{
					retval["shop_ikey"] = kv.Value;
				}
				else
				{
					retval[kv.Key] = kv.Value;
				}
			}
			
			foreach( IXMLNode nn in n.Children )
			{
				switch( nn.Name )
				{
				case "costs":
					retval["costs"] = CrmParser_.ParseCostList( nn );
					break;
				case "modifiers":
					//We require and expect that there be only one modifier and that modifier is a grant_item
					//were we to support the more general case we'd do this:
					//    retval["modifiers"] = CrmParser_.ParseCostList( kv.Value[0] );
					
					//TODO : Add some checks for this... it could crash if we dontget what we expect!

					retval["ikey"] = nn.GetFirstChild("grant_item").GetAttribute("ikey");
					break;
				default:
					retval[nn.Name] = nn.Text;
					break;
				}
			}
			
			return retval;
		}
		
	}

	public class XmlToPropertyHashtable : IXmlToHashtable
	{
		public XmlToPropertyHashtable() {}
		public string GetKey( IXMLNode n )
		{
			string key=n.GetAttribute("ikey");
			if (key!=null) return key;
			return n.GetAttribute("name");
		}

		public Hashtable BuildHashtable( IXMLNode n )
		{
			Debug.Log("XmlToPropertyHashtable.GetKey - got me a "+n.DebugAsString());

			Hashtable retval = new Hashtable();
			foreach( KeyValuePair<string,string> kv in n.Attributes )
			{
				retval[kv.Key] = kv.Value;
			}
			return retval;
		}
	}
	
	public class XMLToItemHashtable : XmlToInventoryItemHashtable
	{
		public override string GetKey( IXMLNode n )
		{
			return n.GetAttribute("ikey");
		}
	}
}
