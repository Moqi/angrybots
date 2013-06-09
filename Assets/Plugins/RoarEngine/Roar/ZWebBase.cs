
//TODO: Put me in a more sensible place
namespace ZWebAPI
{
	public interface Callback<T>
	{
		void OnError( Roar.RequestResult nn );
		void OnSuccess( Roar.CallbackInfo<T> nn );
	}
}

public class CBBase<T> : ZWebAPI.Callback<T> where T : class
{
	public Roar.Callback<T> cb;
	public CBBase( Roar.Callback<T> in_cb )
	{
		cb = in_cb;
	}
	
	public virtual void HandleSuccess( Roar.CallbackInfo<T> info )
	{
	}
	
	public virtual void HandleError( Roar.RequestResult info )
	{
	}
	
	public virtual void Prologue()
	{
	}
	
	public void OnSuccess( Roar.CallbackInfo<T> info)
	{
		Prologue();
		HandleSuccess( info );
		if(cb!=null)
			cb( info );
	}
			
	public void OnError( Roar.RequestResult info)
	{
		Prologue();
		HandleError( info );
		if(cb!=null)
			cb( new Roar.CallbackInfo<T>( null, info.code, info.msg ) );
	}
}
