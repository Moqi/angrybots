using UnityEngine;
using System.Collections;

public class ToggleTasks : MonoBehaviour {
	
	public GameObject target = null;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnMouseDown() {
		Debug.Log("OnMouseDown called!");
		target.GetComponent<RoarTasksWidget>().enabled = ! target.GetComponent<RoarTasksWidget>().enabled;
	}
}
