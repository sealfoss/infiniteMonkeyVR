using UnityEngine;
using System.Collections;

public class SocketTriggerController : MonoBehaviour {

    public SocketController childSocket;
	// Use this for initialization
	void Start () {
        childSocket = GetComponentInChildren<SocketController>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnTriggerEnter(Collider collided){
        if (collided.name.Contains("OrientationTriggerFront")){
            if (collided.GetComponentInParent<InteractiveObjectController>().attachedManipulator) {
                print("in trigger!");
                childSocket.plugObj = collided.GetComponentInParent<InteractiveObjectController>();
                childSocket.socketCollision = true;
            }
        }
    }

    void OnTriggerExit(Collider collided) {
        if (collided.name.Contains("OrientationTriggerFront")) {
            print("out trigger!");
            if (collided.GetComponentInParent<InteractiveObjectController>() == childSocket.plugObj) {
                childSocket.socketCollision = false;
            }
        }
	}
}
