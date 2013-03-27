using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Roar.DomainObjects;

namespace Roar.DataConversion
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
		T Build( System.Xml.XmlElement n);
	}


	public interface IXCRMParser
	{
		List<DomainObjects.Cost> ParseCostList (System.Xml.XmlElement n);
		List<DomainObjects.Modifier> ParseModifierList (System.Xml.XmlElement n);
		List<DomainObjects.Requirement> ParseRequirementList (System.Xml.XmlElement n);
		List<DomainObjects.ItemStat> ParseItemStatList (System.Xml.XmlElement n);

		List<string> ParseTagList( System.Xml.XmlElement n);
	}

	public class XCRMParser : IXCRMParser
	{
		public IXCRMParser crm = null;
		
		public XCRMParser()
		{
			crm = this;
		}

		public DomainObjects.Modifier ParseAModifier( System.Xml.XmlElement n )
		{
			DomainObjects.Modifier retval;
			switch( n.Name )
			{
			case "remove_items":
			{
				DomainObjects.Modifiers.RemoveItems remove_item_mod = new DomainObjects.Modifiers.RemoveItems();
				remove_item_mod.ikey = n.GetAttribute("ikey");
				if ( n.HasAttribute("count") )
				{
					if( ! System.Int32.TryParse( n.GetAttribute("count"), out remove_item_mod.count ) )
					{
						throw new InvalidXMLElementException("Unable to parse remove_items count to integer");
					}
				}
				retval = remove_item_mod;
				break;
			}
			case "grant_stat":
			{
				DomainObjects.Modifiers.GrantStat grant_stat_mod = new DomainObjects.Modifiers.GrantStat();
				grant_stat_mod.ikey = n.GetAttribute("ikey");
				grant_stat_mod.type = n.GetAttribute("type");
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
				grant_stat_mod.type = n.GetAttribute("type");
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
				DomainObjects.Modifiers.GrantItem grant_state_item = new DomainObjects.Modifiers.GrantItem();
				grant_state_item.ikey = n.GetAttribute("ikey");
				retval = grant_state_item;
				break;
			}
			case "grant_xp":
			{
				DomainObjects.Modifiers.GrantXp grant_xp = new DomainObjects.Modifiers.GrantXp();
				if(! System.Int32.TryParse(n.GetAttribute("value"), out grant_xp.value))
				{
					throw new InvalidXMLElementException("Unable to parse grant_xp value to integer");
				}
				retval = grant_xp;
				break;
			}
			case "grant_xp_range":
			{
				DomainObjects.Modifiers.GrantXpRange grant_xp_range = new DomainObjects.Modifiers.GrantXpRange();
				if(! System.Int32.TryParse(n.GetAttribute("min"), out grant_xp_range.min))
				{
					throw new InvalidXMLElementException("Unable to parse grant_xp_range min to integer");
				}
				if(! System.Int32.TryParse(n.GetAttribute("max"), out grant_xp_range.max))
				{
					throw new InvalidXMLElementException("Unable to parse grant_xp_range max to integer");
				}
				retval = grant_xp_range;
				break;
			}
			case "random_choice":
			{
				DomainObjects.Modifiers.RandomChoice random_choice = new DomainObjects.Modifiers.RandomChoice();
				random_choice.choices = new List<DomainObjects.Modifiers.RandomChoice.ChoiceEntry>();
				foreach ( System.Xml.XmlNode nn in n)
				{
					if( nn.NodeType != System.Xml.XmlNodeType.Element ) continue;
					
					if(nn.Name == "choice")
					{
						DomainObjects.Modifiers.RandomChoice.ChoiceEntry entry = new DomainObjects.Modifiers.RandomChoice.ChoiceEntry();
						if(! System.Int32.TryParse((nn as System.Xml.XmlElement).GetAttribute("weight"), out entry.weight))
						{
								throw new InvalidXMLElementException("Unable to parse choice weight to integer.");
						}
						foreach(System.Xml.XmlNode nnn_n in nn)
						{
							if(nnn_n.NodeType != System.Xml.XmlNodeType.Element ) continue;
							System.Xml.XmlElement nnn = nnn_n as System.Xml.XmlElement;
							
							switch(nnn.Name)
							{
							case "modifier":
								entry.modifiers = crm.ParseModifierList(nnn);
								break;
							case "requirement":
								entry.requirements = crm.ParseRequirementList(nnn);
								break;
							default:
								throw new InvalidXMLElementException("Invalid choice element for Random Choice : "+nnn.Name);
							}
						}
						random_choice.choices.Add(entry);
					}
					else
					{
						throw new InvalidXMLElementException("Invalid Random Choice node : "+nn.Name);
					}
				}
				retval = random_choice;
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
				retval = new DomainObjects.Modifiers.Nothing();
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
				foreach (System.Xml.XmlNode nn_n in n)
				{
					if( nn_n.NodeType != System.Xml.XmlNodeType.Element ) continue;
					System.Xml.XmlElement nn = nn_n as System.Xml.XmlElement;
					
					switch( nn.Name )
					{
					case "if":
						m.if_ = crm.ParseRequirementList(nn);
						break;
					case "then":
						m.then_ = crm.ParseModifierList(nn);
						break;
					case "else":
						m.else_ = crm.ParseModifierList(nn);
						break;
					default:
						throw new InvalidXMLElementException("Invalid if-then-else node : "+nn.Name);
					}
				}
				retval = m;
				break;
			}
			default:
				throw new InvalidXMLElementException("Invalid modifier type : "+n.Name);

			}
			return retval;
		}

		public List<DomainObjects.Modifier> ParseModifierList (System.Xml.XmlElement n)
		{
			List<DomainObjects.Modifier> modifier_list = new List<DomainObjects.Modifier> ();
			if (n == null)
			{
				return modifier_list;
			}
			foreach( System.Xml.XmlNode nn in n )
			{
				if( nn.NodeType != System.Xml.XmlNodeType.Element ) continue;
				modifier_list.Add( ParseAModifier(nn as System.Xml.XmlElement) );
			}
			return modifier_list;
		}

		public DomainObjects.Cost ParseACost( System.Xml.XmlElement n )
		{
			DomainObjects.Cost retval;
			//TODO: Implement the rest!
			switch( n.Name )
			{
			case "stat_cost":
				{
					DomainObjects.Costs.Stat stat = new DomainObjects.Costs.Stat();
					stat.ikey = n.GetAttribute("ikey");
					stat.type = n.GetAttribute("type");
					if(! System.Int32.TryParse(n.GetAttribute("value"), out stat.value))
					{
						throw new InvalidXMLElementException("Unable to parse value to integer.");
					}
					retval = stat;
				}
				break;
				
			case "stat_change":
				{
					DomainObjects.Costs.Stat stat = new DomainObjects.Costs.Stat();
					stat.ikey = n.GetAttribute("ikey");
					stat.type = n.GetAttribute("type");
					if(! System.Int32.TryParse(n.GetAttribute("value"), out stat.value))
					{
						throw new InvalidXMLElementException("Unable to parse value to integer.");
					}
					if(stat.value <0)
						stat.value = stat.value *-1;
					
					retval = stat;
				}
				break;
				
			case "item_cost":
				DomainObjects.Costs.Item item = new DomainObjects.Costs.Item();
				item.ikey = n.GetAttribute("ikey");
				if(! System.Int32.TryParse(n.GetAttribute("number_required"), out item.number_required))
				{
					throw new InvalidXMLElementException("Unable to parse number_required to integer.");
				}
				retval = item;
				break;
			default:
				throw new UnexpectedXMLElementException("Invalid cost type, \""+n.Name+"\"");
			}

			retval.ok = (n.GetAttribute("ok")=="true");
			retval.reason = n.GetAttribute("reason");
			return retval;
		}


		
		public List<DomainObjects.Cost> ParseCostList ( System.Xml.XmlElement n)
		{
			List<DomainObjects.Cost> cost_list = new List<DomainObjects.Cost> ();
			if (n == null)
			{
				return cost_list;
			}
			foreach ( System.Xml.XmlNode nn in n)
			{
				if( nn.NodeType != System.Xml.XmlNodeType.Element ) continue;
				cost_list.Add ( ParseACost(nn as System.Xml.XmlElement) );
			}
			return cost_list;
		}

		public DomainObjects.Requirement ParseARequirement( System.Xml.XmlElement n )
		{
			DomainObjects.Requirement retval;

			switch( n.Name )
			{
			case "friends_requirement":
				DomainObjects.Requirements.Friends friends = new DomainObjects.Requirements.Friends();
				if(! System.Int32.TryParse(n.GetAttribute("required"), out friends.required))
				{
					throw new InvalidXMLElementException("Unable to parse required to integer");
				}
				retval = friends;
				break;
			case "and":
				DomainObjects.Requirements.And and = new DomainObjects.Requirements.And();
				retval = and;
				break;
			case "true_requirement":
				retval = new DomainObjects.Requirements.True();
				break;
			case "false_requirement":
				retval = new DomainObjects.Requirements.False();
				break;
			case "level_requirement":
				DomainObjects.Requirements.Level level_req = new DomainObjects.Requirements.Level();
				if( ! System.Int32.TryParse( n.GetAttribute("level"), out level_req.level) )
				{
					throw new InvalidXMLElementException("Unable to parse level restriction to integer");
				} ;
				retval = level_req;
				break;
			case "item_requirement":
				DomainObjects.Requirements.Item item_req = new DomainObjects.Requirements.Item();
				item_req.ikey = n.GetAttribute("ikey");
				if( ! System.Int32.TryParse( n.GetAttribute("number_required"), out item_req.number_required ) )
				{
					throw new InvalidXMLElementException("Unable to parse item count to integer");
				}
				retval = item_req;
				break;
			case "stat_requirement":
				DomainObjects.Requirements.Stat stat_req = new DomainObjects.Requirements.Stat();
				stat_req.ikey = n.GetAttribute("ikey");
				stat_req.type = n.GetAttribute("type");
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

			retval.ok = (n.GetAttribute("ok")=="true");
			retval.reason = n.GetAttributeOrDefault("reason",null);
			return retval;
		}

		
		public List<DomainObjects.Requirement> ParseRequirementList (System.Xml.XmlElement n)
		{
			List<DomainObjects.Requirement> req_list = new List<DomainObjects.Requirement> ();
			if (n == null)
			{
				return req_list;
			}
			foreach (System.Xml.XmlNode nn in n)
			{
				if( nn.NodeType != System.Xml.XmlNodeType.Element ) continue;
				req_list.Add ( ParseARequirement(nn as System.Xml.XmlElement) );
			}
			return req_list;
		}
		
		public DomainObjects.ItemStat ParseAnItemStat (System.Xml.XmlElement n)
		{
			DomainObjects.ItemStat retval = null;
			string ikey = n.GetAttribute("ikey");;
			int value = 0;
			System.Int32.TryParse( n.GetAttribute("value"), out value );
			
			switch (n.Name)
			{
			case "regen_stat":
				DomainObjects.ItemStats.RegenStat regen_stat = new DomainObjects.ItemStats.RegenStat(ikey, value);
				if (n.HasAttribute("every"))
				{
					if (! System.Int32.TryParse(n.GetAttribute("every"), out regen_stat.every))
					{
						throw new InvalidXMLElementException("Unable to parse the \"every\" attribute for RegenStat");
					}
				}
				retval = regen_stat;
				break;
			case "regen_stat_limited":
				DomainObjects.ItemStats.RegenStatLimited regen_stat_limited = new DomainObjects.ItemStats.RegenStatLimited(ikey, value);
				if (n.HasAttribute("repeat"))
				{
					if (! System.Int32.TryParse(n.GetAttribute("repeat"), out regen_stat_limited.repeat))
					{
						throw new InvalidXMLElementException("Unable to parse the repeat attribute for RegenStatLimited");
					}
				}
				if (n.HasAttribute("times_used"))
				{
					if (! System.Int32.TryParse(n.GetAttribute("times_used"), out regen_stat_limited.times_used))
					{
						throw new InvalidXMLElementException("Unable to parse the times_used attribute for RegenStatLimited");
					}
				}
				retval = regen_stat_limited;
				break;
			case "grant_stat":
				retval = new DomainObjects.ItemStats.GrantStat(ikey, value);
				break;
			case "equip_attribute":
				retval = new DomainObjects.ItemStats.EquipAttribute(ikey, value);
				break;
			case "collect_stat":
				DomainObjects.ItemStats.CollectStat collect_stat = new DomainObjects.ItemStats.CollectStat(ikey, value);
				if (n.HasAttribute("every"))
				{
					if (! System.Int32.TryParse(n.GetAttribute("every"), out collect_stat.every))
					{
						throw new InvalidXMLElementException("Unable to parse the \"every\" attribute for CollectStat");
					}
				}
				if (n.HasAttribute("window"))
				{
					if (! System.Int32.TryParse(n.GetAttribute("window"), out collect_stat.window))
					{
						throw new InvalidXMLElementException("Unable to parse the window attribute for CollectStat");
					}
				}
				if (n.HasAttribute("collect_at"))
				{
					if (! System.Int32.TryParse(n.GetAttribute("collect_at"), out collect_stat.collect_at))
					{
						throw new InvalidXMLElementException("Unable to parse the collect_at attribute for CollectStat");
					}
				}
				retval = collect_stat;
				break;
			default:
				retval = new DomainObjects.ItemStats.UnknownStat(ikey, value);
				break;
			}
			return retval;
		}

		public List<DomainObjects.ItemStat> ParseItemStatList (System.Xml.XmlElement n)
		{
			List<DomainObjects.ItemStat> item_stat_list = new List<DomainObjects.ItemStat>();
			if (n == null)
			{
				return item_stat_list;
			}
			foreach(System.Xml.XmlNode nn in n)
			{
				if( nn.NodeType != System.Xml.XmlNodeType.Element ) continue;
				item_stat_list.Add(ParseAnItemStat(nn as System.Xml.XmlElement));
			}
			return item_stat_list;
		}

		public List<string> ParseTagList( System.Xml.XmlElement n)
		{
			List<string> tags = new List<string>();
			if (n == null)
			{
				return tags;
			}
			System.Xml.XmlNodeList tag_nodes = n.SelectNodes("./tag");
			foreach (System.Xml.XmlNode tag_node in tag_nodes)
			{
				System.Xml.XmlElement tag_element = tag_node as System.Xml.XmlElement;
				tags.Add(tag_element.GetAttribute("value"));
			}
			return tags;
		}


	}

	public class XmlToShopEntry : IXmlToObject<DomainObjects.ShopEntry>
	{
		public IXCRMParser CrmParser_;

		public XmlToShopEntry ()
		{
			CrmParser_ = new XCRMParser ();
		}

		public ShopEntry Build( System.Xml.XmlElement n)
		{
			ShopEntry retval = new ShopEntry ();
			
			retval.ikey = n.GetAttributeOrDefault("ikey",null);
			retval.label = n.GetAttribute("label");
			retval.description = n.GetAttribute("description");
			
			foreach( System.Xml.XmlAttribute a in n.Attributes )
			{
				if(a.Name=="ikey") continue;
				if(a.Name=="label") continue;
				if(a.Name=="description") continue;
				throw new UnexpectedXMLElementException("unexpected attribute, \""+a.Name+"\", on ShopEntry");
			}

			if (retval.ikey==null) throw new MissingXMLElementException("missing attribute, \"ikey\", on ShopEntry");

			retval.costs = new List<Cost>();
			retval.modifiers = new List<Modifier>();

			foreach (System.Xml.XmlNode nn_n in n) {
				if(nn_n.NodeType!=System.Xml.XmlNodeType.Element) continue;
				System.Xml.XmlElement nn = nn_n as System.Xml.XmlElement;
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


	//TODO: Remove me!
	public class XmlToFoo : IXmlToObject<Foo>
	{
		public Foo Build(System.Xml.XmlElement n)
		{
			return new Foo();
		}
	}

}
