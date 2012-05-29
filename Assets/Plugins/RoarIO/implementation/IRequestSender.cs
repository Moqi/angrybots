using System.Collections;

public delegate void RequestCallback( IXMLNode d, int code, string msg, string id, Hashtable opt);

public interface IRequestSender
{
  void make_call( string apicall, Hashtable args, RequestCallback cb, Hashtable opt );
}

