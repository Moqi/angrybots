using UnityEngine;
using System.Collections;
using Roar;

public class ThirdPersonNetwork : Photon.MonoBehaviour
{
    private TextMesh tMesh;
    private string old_name = "";
    private string old_roar_key = "";
    void Awake()
    {
        Debug.Log("SPAWN PLAYER");
        tMesh = gameObject.GetComponentInChildren<TextMesh>();
    }
    void Start ()   
    {		    	
    	transform.SendMessage ("IsLocalPlayer", photonView.isMine, SendMessageOptions.DontRequireReceiver);
        
		if (photonView.isMine)
        {
            tMesh.gameObject.active = false;
            Camera.main.SendMessage("SetLocalPlayerTransform", transform, SendMessageOptions.DontRequireReceiver);
		
        }
        else
        {
            tMesh.text = "sonda"; //photonView.owner.name;
        }


    }


    void Update()
    {
        if (!photonView.isMine)
        {
            Vector3 pos = tMesh.transform.position - new Vector3(-10, 5, 10);// Camera.main.transform.position;
           // pos = tMesh.transform.position + pos;
            tMesh.transform.LookAt(pos);
            string new_name = photonView.owner.name;
            if (new_name != old_name) {
            	tMesh.text = new_name;
            	old_name = new_name;
            	Debug.Log("NAME CHANGE DETECTED [" + new_name + "] <" + photonView.owner.roarid + ">");
            	if (photonView.owner.roarid != "") {
            		GameObject friends_widget = GameObject.Find("RoarFriendsWidget");
            		if (friends_widget != null) {
            			Debug.Log("WIDGET FOUND");
            			RoarFriendsListWidget widget = friends_widget.GetComponent<RoarFriendsListWidget>();
            			if (widget != null) {
            				Debug.Log("WIDGET SCRIPT FOUND");
            				widget.insertExternalPlayer(photonView.owner.roarid, new_name);
            			} else Debug.Log("WIDGET SCRIPT NOT FOUND");
            			//RoarFriendsListWidget widget = friends_widget.GetComponent(RoarFriendsListWidget) as RoarFriendsListWidget;
            		} else Debug.Log("WIDGET NOT FOUND");
            	}
            }
        }
    }

    void OnPhotonInstantiate(PhotonMessageInfo info)
    {       
        GameManager.AddPlayer(transform);
        Debug.Log("ADD PLAYER [" + photonView.owner.name + "]");
    }
    void OnDestroy()
    {
      
        GameManager.RemovePlayer(transform);
    }
   

}