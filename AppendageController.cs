using UnityEngine;
using System.Collections;

public class AppendageController : MonoBehaviour {

    public ManipulatorController manipulator;
    public bool grabbedStatus = false;
    Transform tracker;
    public bool moveAxisX = false;
    public bool moveAxisY = false;
    public bool moveAxisZ = true;
    private float trackerPos;
    private float trackerRot;
    private float lastTrackerPos;
    private float lastTrackerRot;
    public float maxPos;
    public float minPos;
    public float maxRot;
    public float minRot;
    public float maxDistance;
    public float dist;
    public float releasePos;
    public float appendagePos;
    public float appendageRot;
    public float movementResistance;
    public float movementOffset;

    // Use this for initialization
    void Start () {
	}

    // Update is called once per frame
    void Update()
    {
        if (manipulator)
        {
            /**if (manipulator.grabbedObj = GetComponentInParent<InteractiveObjectController>())
            {
                print("dropping!1");
                Drop();
            }**/

            if (manipulator.grippingStatus == true)
            {
                grabbedStatus = true;
                Track();
                Move();
                
            }

            if (grabbedStatus == true && manipulator.grippingStatus == false)
            {
                Drop();
            }
        }
	}

    void Drop()
    {
        
        grabbedStatus = false;
        manipulator = null;
    }

    void Move()
    {
        float rate = movementResistance * Time.deltaTime;
        
        if (moveAxisX)
        {
            this.transform.localEulerAngles = new Vector3(appendageRot, 0, 0);
            Vector3 newPosition = new Vector3(appendagePos, 0, 0);
            transform.localPosition = Vector3.Lerp(transform.localPosition, newPosition, rate);
        }

        if (moveAxisY)
        {
            this.transform.localEulerAngles = new Vector3(0, appendageRot, 0);
            Vector3 newPosition = new Vector3(0, appendagePos, 0);
            transform.localPosition = Vector3.Lerp(transform.localPosition, newPosition, rate);
        }

        if (moveAxisZ)
        {
            this.transform.localEulerAngles = new Vector3(0, 0, appendageRot);
            //this.transform.localPosition = new Vector3(0, 0, appendagePos);
            Vector3 newPosition = new Vector3(0, 0, appendagePos);
            transform.localPosition = Vector3.Lerp(transform.localPosition, newPosition, rate);
        }
    }

    void Track()
    {
        tracker = new GameObject().transform;
        tracker.transform.parent = this.transform.parent;
        tracker.position = manipulator.transform.position;
        tracker.rotation = manipulator.transform.rotation;

        if (moveAxisX)
        {
            trackerRot = tracker.transform.localRotation.eulerAngles.x;
            trackerPos = tracker.transform.localPosition.x + movementOffset;
        }

        if (moveAxisY)
        {
            trackerRot = tracker.transform.localRotation.eulerAngles.y;
            trackerPos = tracker.transform.localPosition.y + movementOffset;
        }

        if (moveAxisZ)
        {
            trackerRot = tracker.transform.localRotation.eulerAngles.z;
            trackerPos = tracker.transform.localPosition.z + movementOffset;
        }

        if (trackerRot > 180) { trackerRot -= 360; }
        if (trackerRot > maxRot) { trackerRot = maxRot; }
        if (trackerRot < minRot) { trackerRot = minRot; }
        appendageRot = trackerRot;

        if (trackerPos > lastTrackerPos) { appendagePos = appendagePos + (trackerPos - lastTrackerPos); }
        if (trackerPos < lastTrackerPos) { appendagePos = appendagePos - (lastTrackerPos - trackerPos); }
        if (appendagePos > maxPos) { appendagePos = maxPos; }
        if (appendagePos < minPos) { appendagePos = minPos; }

        lastTrackerPos = trackerPos;
        dist = Vector3.Distance(this.transform.position, manipulator.transform.position);

        if (dist > maxDistance)
        {
            Drop();
        }

        Destroy(tracker.gameObject);
    }

    void OnTriggerEnter(Collider collided)
    {
        ManipulatorController collidedManipulator = collided.GetComponent<ManipulatorController>();

        if (collidedManipulator)
        {
            if (GetComponentInParent<InteractiveObjectController>().attachedManipulator == null)
            {
                collidedManipulator.availableObjects.Add(GetComponentInParent<InteractiveObjectController>());
            }

            if (GetComponentInParent<InteractiveObjectController>().attachedManipulator != null && collidedManipulator != GetComponentInParent<InteractiveObjectController>().attachedManipulator)
            {
                manipulator = collidedManipulator;
            }
        }
    }

    void OnTriggerExit(Collider collided)
    {
        ManipulatorController collidedManipulator = collided.GetComponent<ManipulatorController>();

        if (manipulator && collidedManipulator == manipulator && collidedManipulator.grippingStatus == false)
        {
            Drop();
        }
    }
}
