using UnityEngine;
using System.Collections;

public class SocketController : MonoBehaviour {

    private float trackerPos;
    public float trackerRot;
	private float lastTrackerPos;
    private float lastTrackerRot;
    public float maxPos;
    public float minPos;
    public float maxRot;
    public float minRot;
    public float maxDistance;
    public float dist;
    public float releasePos;
	public float socketPos;
    public float socketRot;
    public bool socketCollision = false;

    public bool moveAxisX = false;
    public bool moveAxisY = false;
    public bool moveAxisZ = true;

    public TrackerController tracker;
	public InteractiveObjectController plugObj = null;
    public ManipulatorController manipulator;
    public Transform defaultPosition;
    public Transform socketOffset;

    void Start () {
        socketOffset = new GameObject().transform;
        socketOffset.parent = this.transform;
    }
	
	void FixedUpdate () {
        if (plugObj != null) {
            if (plugObj.grabbedStatus == true && plugObj.pluggedStatus == false) {
                Plug();
			}

			if (plugObj.pluggedStatus == true && plugObj.grabbedStatus == true) {
                Track();

                if ((plugObj) && plugObj.lockedStatus == false)
                {
                    PluggedMove();
                }
			}
		}
	}
    void Track ()
    {
        if (moveAxisX)
        {
            trackerRot = tracker.transform.localRotation.eulerAngles.x;
            trackerPos = tracker.transform.localPosition.x;
        }

        if (moveAxisY)
        {
            trackerRot = tracker.transform.localRotation.eulerAngles.y;
            trackerPos = tracker.transform.localPosition.y;
        }

        if (moveAxisZ)
        {
            trackerRot = tracker.transform.localRotation.eulerAngles.z;
            trackerPos = tracker.transform.localPosition.z;
        }

        if (trackerRot > 180) { trackerRot -= 360; }
        if (trackerRot > maxRot) { trackerRot = maxRot; }
        if (trackerRot < minRot) { trackerRot = minRot; }
        socketRot = trackerRot;

        if (trackerPos > lastTrackerPos) { socketPos = socketPos + (trackerPos - lastTrackerPos); }
        if (trackerPos < lastTrackerPos) { socketPos = socketPos - (lastTrackerPos - trackerPos); }
        if (socketPos > maxPos) { socketPos = maxPos; }
        if (socketPos < minPos) { socketPos = minPos; }

        lastTrackerPos = trackerPos;
        dist = Vector3.Distance(this.transform.localPosition, tracker.transform.localPosition);

        if (dist > maxDistance && plugObj.lockedStatus == false)
        {
            plugObj.Drop(manipulator);
            Unplug(plugObj);
        }
    }

    void Plug () {
        manipulator = plugObj.attachedManipulator;
        plugObj.socketObj = this;
        plugObj.Plug();
        print("Plug!");
    }

	void PluggedMove() {
        if (moveAxisX)
        {
            socketOffset.localEulerAngles = new Vector3(socketRot, 0, 0);
            socketOffset.localPosition = new Vector3(socketPos, 0, 0);
        }

        if (moveAxisY)
        {
            socketOffset.localEulerAngles = new Vector3(0, socketRot, 0);
            socketOffset.localPosition = new Vector3(0, socketPos, 0);
        }

        if (moveAxisZ)
        {
            socketOffset.localEulerAngles = new Vector3(0, 0, socketRot);
            socketOffset.localPosition = new Vector3(0, 0, socketPos);
        }
        
        if ((releasePos > 0 && trackerPos > releasePos) || (releasePos < 0 && trackerPos < releasePos))
        {
            print("UNPLUG!");
            Unplug(plugObj);
        }
    }

    public void Unplug (InteractiveObjectController plugObj) {

        if (plugObj.lockedStatus == false)
        {
            plugObj.GetComponent<Rigidbody>().isKinematic = false;
            plugObj.GetComponent<Rigidbody>().useGravity = true;
        }
        plugObj.socketObj = null;
        plugObj.pluggedStatus = false;
        plugObj = null;
    }
}
