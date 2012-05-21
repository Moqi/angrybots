
public abstract class IXMLNodeFactory
{
	public static IXMLNodeFactory instance;
	
	public abstract IXMLNode Create();
	public abstract IXMLNode Create(string xml);
	public abstract IXMLNode Create(string name, string text);
}

public class XMLNodeFactory : IXMLNodeFactory
{
	
	public override IXMLNode Create()
	{
		return new XMLNode();
	}
	
	public override IXMLNode Create(string xml)
	{
		XMLNode.XMLParser parser = new XMLNode.XMLParser();
		return parser.Parse(xml);
	}
	
	public override IXMLNode Create(string name, string text)
	{
		XMLNode node = new XMLNode(name, text);
		return node;
	}
}

