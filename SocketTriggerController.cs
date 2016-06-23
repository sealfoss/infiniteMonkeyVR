using UnityEngine;
using System.Collections;

public class SocketTriggerController : MonoBehaviour {

    public SocketController parentSocket;
    public string soughtAfter = "BronzeLock_Key";
	// Use this for initialization
	void Start () {
        parentSocket = GetComponentInChildren<SocketController>();
	}
	
	// Update is called once per frame
	void Update () {
	    if (parentSocket == null) {
            parentSocket = GetComponentInParent<SocketController>();
        }
	}
    void OnTriggerEnter(Collider collided){
        if (collided.name.Contains("Trigger")){
            string name = collided.GetComponentInParent<InteractiveObjectController>().name;
            if (name == "BronzeLock_Key") {
                parentSocket.plugObj = collided.GetComponentInParent<InteractiveObjectController>();
                parentSocket.socketCollision = true;
            }
        }
    }

    void OnTriggerExit(Collider collided) {
        if (collided.name.Contains("Trigger")) {
            if (collided.GetComponentInParent<InteractiveObjectController>() == parentSocket.plugObj) {
                parentSocket.socketCollision = false;
            }
        }
	}
}
