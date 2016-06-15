using UnityEngine;
using System.Collections;

public class TrackerController : MonoBehaviour {
    public SocketController socket;
    private ManipulatorController manipulator;
	// Use this for initialization
	void Start () {
        socket = GetComponentInParent<SocketTriggerController>().childSocket;
	}

    // Update is called once per frame
    void Update() {
        if (socket) {
            if (socket.tracker == null)
            {
                socket.tracker = this;
            }
        }

        if (!socket) {
            socket = GetComponentInParent<SocketTriggerController>().childSocket;
        }

        if (socket.plugObj) {
            if (!manipulator) {
                manipulator = socket.manipulator;
            }

            transform.position = manipulator.transform.position;
        }    

        if (socket.plugObj == null && transform.localPosition != Vector3.zero) {
            transform.localPosition = Vector3.zero;
        }
	}
}
