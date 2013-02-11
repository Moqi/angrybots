using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoarShopTest : MonoBehaviour {

	static void onShopFetch( Roar.CallbackInfo< IDictionary<string, Roar.DomainObjects.ShopEntry> > info )
	{
		Debug.Log( "onShopFetch Called" );
		Debug.Log(info);
		Debug.Log(info.code);
		Debug.Log(info.msg);
		Debug.Log(info.data);	
	}
	
	static void onLogin( Roar.CallbackInfo< Roar.WebObjects.User.LoginResponse > info )
	{
		Debug.Log( "onLogin Called" );
		Debug.Log(info);
		Debug.Log(info.code);
		Debug.Log(info.msg);
		Debug.Log(info.data);
		IRoar roar = DefaultRoar.Instance;
		roar.Shop.Fetch( onShopFetch );
	}
	
	// Use this for initialization
	void Start () {
		IRoar roar = DefaultRoar.Instance;
		roar.User.Login("mike","mike", onLogin);
	}
	
	void OnMouseDown() {
		Debug.Log ("OnMouseDown Called");
		IRoar roar = DefaultRoar.Instance;
		roar.User.Login("mike","mike", onLogin);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
