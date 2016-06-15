using UnityEngine;
using System.Collections;

public class SocketController : MonoBehaviour {

	private float trackerY;
	private float lastTrackerY;
    public float maxPosY;
    public float minPosY;
    private float releasePosY;
	public float socketY;
    public bool socketCollision = false;

    public TrackerController tracker;
	public InteractiveObjectController plugObj = null;
    public ManipulatorController manipulator;
    public Transform defaultPosition;

    void Start () {
        defaultPosition = this.transform;
        maxPosY = 1.0f;
        minPosY = 0.3f;
        releasePosY = maxPosY + 0.4f;
    }
	
	void FixedUpdate () {
        if (plugObj != null) {
            if (plugObj.grabbedStatus == true && plugObj.pluggedStatus == false) {
                Plug();
			}

			if (plugObj.pluggedStatus == true && plugObj.grabbedStatus == true) {
                PluggedMove ();
			}
		}
	}

    void Plug () {
        manipulator = plugObj.attachedManipulator;
        plugObj.socketObj = this;
        plugObj.Plug();
        print("Plug!");
    }

	void PluggedMove() {
        trackerY = tracker.transform.localPosition.y;
		if (trackerY > lastTrackerY) { socketY = socketY + (trackerY - lastTrackerY); }
		if (trackerY < lastTrackerY) { socketY = socketY - (lastTrackerY - trackerY); }
		if (socketY > maxPosY) { socketY = maxPosY; }
		if (socketY < minPosY) { socketY = minPosY; }
        this.transform.localPosition = new Vector3(0, socketY, 0);
        lastTrackerY = trackerY;
        if (trackerY > releasePosY) { Unplug(); }
        if (tracker.transform.localPosition.x > 5.0f || tracker.transform.localPosition.x < -5.0f) { manipulator.Drop(); }
        if (tracker.transform.localPosition.z > 5.0f || tracker.transform.localPosition.z < -5.0f) { manipulator.Drop(); }
    }

    public void Unplug () {
        print("Unplug!");
        plugObj.GetComponent<Rigidbody>().isKinematic = false;
        plugObj.GetComponent<Rigidbody>().useGravity = true;
        plugObj.pluggedStatus = false;
        plugObj = null;
        transform.position = defaultPosition.position;
    }
}
