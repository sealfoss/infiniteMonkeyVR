using UnityEngine;
using System.Collections;

public class InteractiveObjectController : MonoBehaviour {

	public float velocityFactor = 20000f;
	public float rotationFactor = 600f;
	public int maxAV = 20;
		
	private Rigidbody rigidBody; 
	private Vector3 posDelta;
	private Quaternion rotDelta;
	private float angle;
	private Vector3 axis;

	public bool lockedStatus = false;
	public bool grabbedStatus = false;
	public bool pluggedStatus = false;
	public bool actionStatus = false;
	public bool offsetStatus = false;

	public ManipulatorController attachedManipulator;
	public Transform interactionPoint;
	private Transform offsetPoint;
	public GameObject socketObj;

	void Start () {
		rigidBody = GetComponent<Rigidbody>();
		interactionPoint = new GameObject().transform;
		velocityFactor /= rigidBody.mass;
		rotationFactor /= rigidBody.mass;
		this.rigidBody.maxAngularVelocity = maxAV;
	}

	void FixedUpdate () {
		if (lockedStatus) {
			if (grabbedStatus && actionStatus) {
				lockedStatus = false;
			}
		}

		if (grabbedStatus) {
			GrabbedMove ();

			if (actionStatus) {
				Act ();
			}
		}

		if (pluggedStatus) {
			PluggedMove ();
		}
	}

	void Act () {
	}

	void Drop () {
		if (pluggedStatus) {
			GetComponent<Rigidbody>().isKinematic = true;
			GetComponent<Rigidbody>().useGravity = false;
		}

		grabbedStatus = false;
		pluggedStatus = false;
		attachedManipulator = null;
		interactionPoint.SetParent (transform, false);
	}

	void PluggedMove () {
		this.transform.position = socketObj.transform.position;
		this.transform.rotation = socketObj.transform.rotation;
	}

	public void Plug () {
		GetComponent<Rigidbody>().isKinematic = true;
		GetComponent<Rigidbody>().useGravity = false;
		transform.position = Vector3.Lerp(this.transform.position, socketObj.transform.position, Time.deltaTime * 1);
		transform.rotation = Quaternion.Lerp(this.transform.rotation, socketObj.transform.rotation, Time.deltaTime * 1);
		pluggedStatus = true;
	}

	void PluggedMove () {
	
	}

	public void Grab (ManipulatorController manipulator) {
		attachedManipulator = manipulator;

		if (offsetStatus) {
			transform.rotation = Quaternion.Lerp(this.transform.rotation, offsetPoint.rotation, Time.deltaTime * 1);
			interactionPoint.position = offsetPoint.position;
			interactionPoint.rotation = offsetPoint.rotation;
		}

		if (!offsetStatus) {
			interactionPoint.position = manipulator.transform.position;
			interactionPoint.rotation = manipulator.transform.rotation;
		}

		interactionPoint.SetParent(transform, true);
		grabbedStatus = true;
	}

	void GrabbedMove()
	{
		if (pluggedStatus) { return; }

		posDelta = attachedManipulator.transform.position - interactionPoint.position;
		this.rigidBody.velocity = posDelta * velocityFactor * Time.fixedDeltaTime;

		rotDelta = attachedManipulator.transform.rotation * Quaternion.Inverse(interactionPoint.rotation);
		rotDelta.ToAngleAxis(out angle, out axis);

		if (angle > 180)
		{
			angle -= 360;
		}

		this.rigidBody.angularVelocity = (Time.fixedDeltaTime * angle * axis) * rotationFactor;
	}
}
