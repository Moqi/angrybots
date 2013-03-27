public static class RoarExtensions
{
	// This is apapted from http://stackoverflow.com/questions/6442123/in-c-how-do-i-convert-a-xmlnode-to-string-with-indentation-without-looping
	// Note that it currently outputs the string as UTF-16 rather than the roar default of UTF-8
	// But since its only for debugging that shouldn't matter too much.
	public static string DebugAsString(this System.Xml.XmlNode node)
	{
		using (var sw = new System.IO.StringWriter())
		{
			using (var xw = new System.Xml.XmlTextWriter(sw))
			{
				xw.Formatting = System.Xml.Formatting.Indented;
				xw.Indentation = 4;
				node.WriteTo(xw);
			}
			return sw.ToString();
		}
	}
	
	/**
	 * This extension allows us to do things like
	 *
	 *     n.SelectSingleNode( "./blah/text()" ).GetValueOrDefault(null)
	 * 
	 * in the case where the selected node may be null, while keeping the code concise.
	 */
	public static string GetValueOrDefault( this System.Xml.XmlNode n, string default_value )
	{
		if(n==null) return default_value;
		if(n.Value==null) return default_value;
		return n.Value;
	}
	
	/**
	 * This extension allows us to do things like
	 *
	 *     n.SelectSingleNode( "./blah" ).GetAttributeOrDefaulr("foo",null)
	 * 
	 * in the case where the selected node may be null, while keeping the code concise.
	 * It also allows us to default to null,. rather than empty string when the attribute is not present.
	 */
	public static string GetAttributeOrDefault( this System.Xml.XmlNode n, string attribute, string default_value )
	{
		System.Xml.XmlElement e = n as System.Xml.XmlElement;
		if(e==null) return default_value;
		
		if( ! e.HasAttribute(attribute) ) return default_value;
		return e.GetAttribute(attribute);
	}


	/**
	 * This extension allows us to do things like
	 *
	 *     n.SelectSingleNode( "./blah" ).GetInnerTextOrDefault("something")
	 *
	 * in the case where the selected node may be null, while still keeping the code consice.
	 */
	public static string GetInnerTextOrDefault( this System.Xml.XmlNode n, string default_value )
	{
		if(n==null) return default_value;
		if(n.InnerText==null) return default_value;
		return n.InnerText;
	}
	
	// TODO: This is not really an extension, more a helper and so should be moved...
	public static System.Xml.XmlElement CreateXmlElement( string tag, string content )
	{
		System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
		System.Xml.XmlElement element = doc.CreateElement(tag);
		doc.AppendChild(element);
		element.AppendChild(doc.CreateTextNode(content));
		return doc.DocumentElement;
	}
	
	public static System.Xml.XmlElement CreateXmlElement(string xml)
	{
		System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
		doc.LoadXml(xml);
		return doc.DocumentElement;
	}
	
	
}
