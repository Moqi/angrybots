using System.Collections;

namespace Roar
{
	public struct RequestResult
	{
		public System.Xml.XmlElement data;
		public int code;
		public string msg;
		
		public RequestResult(System.Xml.XmlElement in_data) : this(in_data, IWebAPI.OK, "") {}
		public RequestResult(System.Xml.XmlElement in_data, int in_code) : this(in_data, in_code, "") {}

		public RequestResult (System.Xml.XmlElement in_data, int in_code, string in_msg)
		{
			data = in_data;
			code = in_code;
			msg = in_msg;
		}
	}

   /**
   * Many roar.io functions take a callback function.  Often this callback is
   * optional, but if you wish to use one it is always a #Roar.Callback type.
   * You might not need one if you choose to catch the results of the call using
   * the events in #RoarManager.
   *
   * The Hashtable returned will usually contain three parameters:
   *
   *   + code : an int corresponding to the values in #IWebAPI
   *   + msg : a string message, often empty on success, but containing more
   *     details in the case of an error
   *   + data : an object with the results of the call.
   *
   * The only place you might need to provide a function of a different signature
   * is when using the events specified in #RoarManager. These events accept a
   * function that corresponds to the data available to the event. See the
   * individual events for details.
   */
	//TODO: Can we unify this callback with the IRequestCallback class?
	public delegate void RequestCallback (RequestResult h);
	

}

public interface IRequestCallback
{
	void OnRequest ( Roar.RequestResult info);
};

public interface IRequestSender
{
	void MakeCall (string apicall, Hashtable args, IRequestCallback cb, bool requires_auth_token);
}

