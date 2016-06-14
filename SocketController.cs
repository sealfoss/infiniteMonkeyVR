using UnityEngine;
using System.Collections;

public class SocketController : MonoBehaviour {

	private float trackerY;
	private float lastTrackerY;
	private float maxY = 0.8f;
	private float minY = 0.4f;
	private float socketY;

	public InteractiveObjectController plugObj = null;
	public GameObject trackerObj;

	void Start () {
		trackerObj.transform.parent = this.transform;
	}
	
	// Update is called once per frame
	void Update () {
		if (plugObj != null) {
			if (plugObj.grabStatus == true && plugObj.pluggedStatus == false) {
				plugObj.Plug ();
			}

			if (plugObj.pluggedStatus) {
				PluggedMove ();
			}
		}
	}

	void PluggedMove() {
		trackerObj.transform.position = pluggedObj.attachedManipulator.transform.position;
		trackerY = trackerObj.transform.localPosition.y;
		if (trackerY > lastTrackerY) { socketY = socketY + (trackerY - lastTrackerY); }
		if (trackerY < lastTrackerY) { socketY = socketY - (lastTrackerY - trackerY); }
		if (socketY > maxY) { socketY = maxY; }
		if (socketY < minY) { socketY = minY; }
		this.transform.localPosition = new Vector3((this.transform.localPosition.x), (socketY), (this.transform.localPosition.z));
		lastTrackerY = trackerY;
	}

	void OnTriggerEnter(Collider collided)
	{
		if (collided.name.Contains("OrientationTrigger"))
		{
			if (collided.GetComponentInParent<InteractiveObjectController>().attachedManipulator)
			{
				plugObj = collided.GetComponentInParent<InteractiveObjectController>();
			}
		}
	}

	void On TriggerExit(Collider collided) {
		if (collided.GetComponentInParent<InteractiveObjectController>() == plugObj) {
			plugObj = null;
		}
	}
}
