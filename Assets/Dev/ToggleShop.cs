using UnityEngine;
using System.Collections;

public class ToggleShop : MonoBehaviour {
	
	public GameObject target;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnMouseDown() {
		target.GetComponent<RoarShopWidget>().enabled = ! target.GetComponent<RoarShopWidget>().enabled;
	}
}
