using UnityEngine;
using System.Collections;

public class ToggleFriends : MonoBehaviour {
	
	public GameObject target;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnMouseDown() {
		Debug.Log("OnMouseDown called!");
		target.GetComponent<RoarFriendsListWidget>().enabled = ! target.GetComponent<RoarFriendsListWidget>().enabled;
	}
}
