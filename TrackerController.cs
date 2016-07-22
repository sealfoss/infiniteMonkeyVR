using UnityEngine;
using System.Collections;

public class TrackerController : MonoBehaviour {
    public SocketController socket;
    public ManipulatorController manipulator;
	// Use this for initialization
	void Start () {
        socket = GetComponentInParent<SocketController>();
	}

    // Update is called once per frame
    void Update() {

        if (socket)
        {
            if (socket.tracker == null) {
                socket.tracker = this;
            }

            if (socket.plugObj) {
                if (!manipulator)
                {
                    manipulator = socket.manipulator;
                }

                if (manipulator && socket.plugObj.grabbedStatus)
                {
                    transform.position = manipulator.transform.position;
                    transform.rotation = manipulator.transform.rotation;
                }
            }

            if (((socket.plugObj == null) || (socket.plugObj.grabbedStatus == false)) && (transform.localPosition != Vector3.zero)) {
                manipulator = null;
                transform.localPosition = Vector3.zero;
            }
        }

        if (!socket) {
            socket = GetComponentInParent<SocketController>();
        }
	}
}
