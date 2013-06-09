using WebObjects = Roar.WebObjects;

class CallbackBridge<T> : IRequestCallback where T : class
{
	public ZWebAPI.Callback<T> cb_;
	public Roar.DataConversion.IXmlToObject<T> converter_;
	
	public CallbackBridge( ZWebAPI.Callback<T> cb, Roar.DataConversion.IXmlToObject<T> converter)
	{
		cb_ = cb;
		converter_ = converter;
	}
	public void OnRequest( Roar.RequestResult info )
	{
		Prologue ();
		if (info.code != IWebAPI.OK) {
			if (cb_ != null)
				cb_.OnError( info );
			OnFailure (info);
		} else {
			T result_data = converter_.Build(info.data);
			Roar.CallbackInfo<T> result = new Roar.CallbackInfo<T> (result_data, info.code, info.msg);
			if (cb_ != null)
				cb_.OnSuccess( result );
			OnSuccess( result );
		}
		Epilogue ();
	}
	
	public virtual void OnSuccess( Roar.CallbackInfo<T> info )
	{
	}
	
	public virtual void OnFailure (Roar.RequestResult info)
	{}
	
	public virtual void Prologue ()
	{}
	public virtual void Epilogue ()
	{}
}