using UnityEngine;
using System.Collections;

public class ToggleInventory : MonoBehaviour {
	
	public GameObject target = null;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnMouseDown() {
		Debug.Log("OnMouseDown called!");
		target.GetComponent<RoarInventoryWidget>().enabled = ! target.GetComponent<RoarInventoryWidget>().enabled;
	}
}
