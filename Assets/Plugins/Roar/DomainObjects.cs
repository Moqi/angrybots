using System.Collections.Generic;

namespace Roar
{
	namespace DomainObjects
	{
	
		public abstract class Cost
		{
			public bool ok;
			public string reason;
		};
		
		public abstract class Requirement
		{
		};
		
		public abstract class Modifier
		{
		};
		
		
		namespace Costs
		{
			public class Multiple : Cost
			{
				public List<Cost> costs;
			}
			
			public class Stat : Cost
			{
				//TODO: What?
				public string ikey;
			}
			
			public class Item : Cost
			{
			}
		}
		
		namespace Modifiers
		{
			public class RemoveItems : Modifier
			{
				public string ikey;
				public int count;
			}
			
			public class GrantStat : Modifier
			{
				public string ikey;
				public int value;
			}
			
			
			public class GrantStatRange : Modifier
			{
				public string ikey;
				public int min;
				public int max;
			}
			
			public class GrantXp : Modifier
			{
			}
			
			public class GrantXpRange : Modifier
			{
			}
			
			public class GrantItem : Modifier
			{
				public string ikey;
			}
			
			public class Nothing : Modifier
			{
			}
			
			public class NamedReference : Modifier
			{
			}
			
			public class Script : Modifier
			{
			}
			
			public class Multiple : Modifier
			{
				public List<Modifier> modifier;
			}
			
			public class IfThenElse : Modifier
			{
				public Requirement if_;
				public Modifier then_;
				public Modifier else_;
			}
			
			public class RandomChoice : Modifier
			{
				public struct ChoiceEntry
				{
					public double weight;
					public Modifier modifier;
					public Requirement requirement;
				}
				
				public List<ChoiceEntry> choices;
			}
			
		}
		
		namespace Requirements
		{
			public class Level : Requirement
			{
				public int level;
			}
			
			public class Item : Requirement
			{
				public string ikey;
				public int number_required;
			}
			
			public class Stat : Requirement
			{
				public string ikey;
				public int value;
			}
			
			public class Multiple : Requirement
			{
				public List<Requirement> requirements;
			}
			
			public class True : Requirement
			{
			}
			
			public class False : Requirement
			{
			}
			
			public class Friends : Requirement
			{
			}
		}
		
		
		public interface IDomainObject
		{
			bool MatchesKey( string key );
		}
		
		public class ShopEntry : IDomainObject
		{
			public string as_json() { return "{}"; }
			public bool MatchesKey( string key ) { return false; } //Should check id, ikey, shop_ikey etc.
			
			public string ikey;
			public string label;
			public string description;
			
			public IList<Cost> costs;
			public IList<Modifier> modifiers;
			public IList<Requirement> requirements;
			
			public IList<string> tags;
		};
	}
}