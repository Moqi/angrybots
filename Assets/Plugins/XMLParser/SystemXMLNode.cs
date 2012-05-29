/*
Copyright (c) 2012, Run With Robots
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
    * Neither the name of the roar.io library nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY RUN WITH ROBOTS ''AS IS'' AND ANY
EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL MICHAEL ANDERSON BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class SystemXMLNode : IXMLNode
{
	protected System.Xml.XmlNode node;

	public SystemXMLNode(System.Xml.XmlElement element)
	{
		this.node = element;
	}

	public SystemXMLNode(System.Xml.XmlDocument document)
	{
		this.node = document;
	}

	public string GetAttribute(string key)
	{
		if(this.node.Attributes == null) { return null; }

		XmlAttribute attribute = this.node.Attributes[key];
		return attribute == null ? null : attribute.Value;
	}

	public IEnumerable<KeyValuePair<string, string>> Attributes
	{
		get
		{
			if(this.node.Attributes == null)
			{
				yield break;
			}
			else
			{
				foreach(XmlAttribute x in this.node.Attributes)
				{
					yield return new KeyValuePair<string, string>(x.Name, x.Value);
				}
			}
		}
	}

	public IEnumerable< IXMLNode > Children
	{
		get
		{
			foreach(System.Xml.XmlNode x in this.node.ChildNodes)
			{
				// we're only interested in elements
				if(x.NodeType == XmlNodeType.Element)
				{
					yield return new SystemXMLNode(x as System.Xml.XmlElement);
				}
			}
		}
	}

	public IXMLNode GetFirstChild( string key )
	{
		System.Xml.XmlNode firstChild = this.node.FirstChild;
		// the first child may not be an element
		while(firstChild.NodeType != XmlNodeType.Element && firstChild.NextSibling != null)
		{
			firstChild = firstChild.NextSibling;
		}
		return firstChild == null ? null : new SystemXMLNode(firstChild as System.Xml.XmlElement);
	}

	public string Text
	{
		get
		{
			if(this.node.Value == null)
			{
				System.Xml.XmlNode childNode = this.node.FirstChild;
				if(childNode != null)
				{
					if(childNode.NodeType == XmlNodeType.Text)
					{
						return childNode.InnerText;
					}
					else if(childNode.NodeType == XmlNodeType.CDATA)
					{
						return childNode.InnerText;
					}
				}
				else
				{
					return this.node.InnerText;
				}
			}
			return "";
		}
		set
		{
			this.node.Value = value;
		}
	}

	public string Name
	{
		get
		{
			return this.node.Name;
		}
	}

	public string DebugAsString()
	{
		StringWriter stringWriter = new StringWriter();
		XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
		xmlTextWriter.Formatting = Formatting.Indented;
		this.node.WriteContentTo(xmlTextWriter);
		xmlTextWriter.Flush();
		return stringWriter.GetStringBuilder().ToString();
	}
	
	public List<IXMLNode> GetNodeList(string path)
	{
		return GetObject(path) as List<IXMLNode>;
	}
	
	public IXMLNode GetNode(string path)
	{
		return GetObject(path) as IXMLNode;
	}
	
	public string GetValue(string path)
	{
		return GetObject(path) as string;
	}
	
	/**
	 * Accepts "a>0>b>0" or
	 *         "a>b"
	 **/
	private object GetObject(string path)
	{
		string[] bits = path.Split(">"[0]);
		
		System.Xml.XmlNode currentNode = this.node;
		System.Xml.XmlNodeList currentNodeList = null;
		
		bool listMode=false;
		
		object ob = null;
		
		for(int i=0;i<bits.Length;i++)
		{
			string segment = bits[i];
			if(listMode)
			{
				ob=currentNode= currentNodeList[int.Parse(segment)] as System.Xml.XmlNode;
				listMode=false;
			}
			else
			{
				if(currentNode[segment] != null)
				{
					ob=currentNode[segment];
					currentNodeList = currentNode.SelectNodes("//" + segment);
					listMode=true;
				}
				else
				{
					if(currentNode.Attributes[segment] != null)
					{
						ob = currentNode.Attributes[segment].Value;
					}
					else
					{
						ob = currentNode.Value;
					}
					// reached a leaf node/attribute
					if(i!=(bits.Length-1))
					{
						// unexpected leaf node
						string actualPath="";
						for(int j=0;j<=i;j++)
						{
							actualPath=actualPath+">"+bits[j];
						}
					}
					return ob;
				}
			}
		}
		if(listMode)
		{
			// convert System.Xml.XmlNodeList to List<IXMLNode>
			List<IXMLNode> list = new List<IXMLNode>();
			foreach(System.Xml.XmlNode n in currentNodeList)
			{
				if(n.NodeType == XmlNodeType.Element)
				{
					list.Add(new SystemXMLNode(n as System.Xml.XmlElement));
				}
			}
			return list;
		}
		else
		{
			if(currentNode.Equals(ob))
			{
				ob = new SystemXMLNode(currentNode as System.Xml.XmlElement);
			}
			return ob;
		}
	}
}
