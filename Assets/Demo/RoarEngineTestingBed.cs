using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoarEngineTestingBed : MonoBehaviour {

	private DefaultRoar roar = null;

	// Use this for initialization
	void Start () {
		Debug.Log ("				I AM HERE!");
	}
	
	void OnLoginCallback (Roar.CallbackInfo<Roar.WebObjects.User.LoginResponse> info) {
		Debug.Log ("				CALLBACK ACTIVE! [" + info.msg + "]");
		roar.Properties.Fetch (OnFetchCallback);
	}
	
	void OnFetchCallback (Roar.CallbackInfo< IDictionary<string,Roar.DomainObjects.PlayerAttribute> > info) {
		Roar.DomainObjects.PlayerAttribute level = info.data["level"];
		Debug.Log ("				FETCH CALLBACK ACTIVE!");
		Debug.Log ("				LEVEL = [" + level.value + "]");
	}
	
	void Awake () {
		Debug.Log ("				AWAKE AWAKE AWAKE");
		if (roar == null) {
			Debug.Log ("				LOOKING FOR ROAR????");
			roar = DefaultRoar.Instance;
			//if (roar != null && roar.User != null) roar.User.Login ("we", "we", OnLoginCallback);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
