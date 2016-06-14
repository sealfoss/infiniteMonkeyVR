using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ManipulatorController : MonoBehaviour {

	private bool grabStatus;

	private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
	private SteamVR_TrackedObject trackedObj; //assigns Vive controller to variable.

	private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip; 
	private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;

	private GameObject collidedOb = null;
	private InteractiveObjectController closestObj = null; 
	private InteractiveObjectController grabbedObj = null;

	HashSet<InteractiveObjectController> availableObjects = new HashSet<InteractiveObjectController>();

	void Start () {
		trackedObj = GetComponent<SteamVR_TrackedObject>();
	}

	void Update () {
		if (controller == null)
		{
			Debug.Log("Controller not initialized");
			return;
		}

		if (controller.GetPressDown(gripButton) && availableObjects.Count != 0 && grabbedObj == null) {
			Grab();
		}

		if (controller.GetPressUp(gripButton) && grabbedObj) {
			Drop();
		}

		if (controller.GetPressDown(triggerButton) && grabbedObj) {
			Act();
		}

		if (controller.GetPressUp(triggerButton) && grabbedObj) {
			Cease();
		}
	}

	void Cease () {
		grabbedObj.actionStatus = false;
	}

	void Act () {
		grabbedObj.actionStatus = true;
	}

	void Drop () {
		grabbedObj.Drop;
		grabbedObj = null;
	}


	void Grab ()
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
		grabbedObj.Grab(this);
	}

	void OnTriggerEnter (Collider collided) {
		InteractiveObjectController collidedObj = collided.GetComponent<InteractiveObjectController> ();

		if (collidedObj) {
			availableObjects.Add (collidedObj);
		}
	}

	void OnTriggerExit (Collider collided){
		InteractiveObjectController collidedObj = collided.GetComponent<InteractiveObjectController> ();

		if (collidedObj) {
			availableObjects.Remove (collidedObj);
		}
	}
}
