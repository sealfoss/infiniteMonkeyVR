using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ManipulatorController : MonoBehaviour {

    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    private SteamVR_TrackedObject trackedObj; //assigns Vive controller to variable.

    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip; 
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger; //assigns Vive controller input to variables.

    public bool collisionStatus; //status booleans 
	private bool grabStatus = false;

    private GameObject collidedObj; //placeholder gameobject for objects the manipulator comes in contact with
    private InteractiveObjectController closestObj; //these variables and the hashset are used in determining which object
	private InteractiveObjectController grabbedObj; //is closest to the manipulator when the grip button is pressed, thus which object is grabbed

    HashSet<InteractiveObjectController> availableObjects = new HashSet<InteractiveObjectController>();
    //HashSet is collection of objects the controller is currently colliding with.  Closest object is picked up when grabbed.

    void Start()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    void Update () 
	{
        if (controller == null)
        {
            Debug.Log("Controller not initialized");
            return;
        }

        if (controller.GetPressDown(gripButton) && availableObjects.Count != 0)
        {
            Pickup();
        }

        if (controller.GetPressUp(gripButton) && grabbedObj != null)
        {
            Drop();
        }

        if (controller.GetPressDown(triggerButton) && grabStatus == true)
        {
            Act();
        }

        if (controller.GetPressUp(triggerButton) && grabStatus == true)
        {
            Cease();
        }
    }
		
    void Act ()
    { }

    void Cease ()
    { }

	void Pickup ()
	{
		float minDistance = float.MaxValue;
		float distance;
        
		foreach (InteractiveObjectController item in availableObjects)
		{
			distance = (item.transform.position - transform.position).sqrMagnitude;

			if (distance < minDistance) {
				minDistance = distance;
				closestObj = item;
			}
		}

		grabbedObj = closestObj;

        if (grabbedObj.pluggedStatus == false)
        {
            grabbedObj.Grab(this);
            if (grabbedObj)
            {
                if (grabbedObj.grabbedStatus == false)
                {
                    grabbedObj.Release(this);
                }
            }
        }

        if (grabbedObj.pluggedStatus == true)
        {
            grabbedObj.attachedManipulator = this;
            grabbedObj.grabbedStatus = true;
            //SocketController socket = grabbedObj.GetComponent<SocketController>();
            //socket.manipulator = this;

            if (grabbedObj.grabbedStatus == false)
            {
                grabbedObj.Release(this);
            }
        }        
	}

	void Drop ()
    { 
        grabbedObj.Release(this);
        grabbedObj = null;
	}

	void OnTriggerEnter (Collider collided)
	{
        InteractiveObjectController collidedObj = collided.GetComponent<InteractiveObjectController> ();

        if (collidedObj) 
		{
			availableObjects.Add (collidedObj);
		}
	}

	void OnTriggerExit (Collider collided)
	{
        InteractiveObjectController collidedObj = collided.GetComponent<InteractiveObjectController> ();

        if (collidedObj) 
		{
            availableObjects.Remove (collidedObj);
        }
    }
}
