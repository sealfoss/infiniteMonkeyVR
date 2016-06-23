using UnityEngine;
using System.Collections;

public class InteractiveObjectController : MonoBehaviour {
    private float rbMass;
    private float rbDrag;
    private float rbADrag;

    public float velocityFactor = 2000f;
	public float rotationFactor = 80f;
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
    public bool highlightedStatus = false;

	public ManipulatorController attachedManipulator;
	public Transform interactionPoint;
    public Transform compositionPoint;
	public Transform offsetPoint;
	public SocketController socketObj;

	void Start () {
        rbMass = GetComponent<Rigidbody>().mass;
        rbDrag = GetComponent<Rigidbody>().drag;
        rbADrag = GetComponent<Rigidbody>().angularDrag;
        rigidBody = GetComponent<Rigidbody>();
		interactionPoint = new GameObject().transform;
		velocityFactor /= rigidBody.mass;
		rotationFactor /= rigidBody.mass;
		this.rigidBody.maxAngularVelocity = maxAV;
	}

	void FixedUpdate () {
        if (lockedStatus == true)
        {
            //transform.position = socketObj.socketOffset.position;
            //transform.eulerAngles = socketObj.socketOffset.rotation.eulerAngles;
		}

        if (lockedStatus == false)
        {
            if (grabbedStatus == true)
            {
                GrabbedMove();

                if (actionStatus)
                {
                    Act();
                }
            }

            if (grabbedStatus == false && pluggedStatus == true)
            {
                socketObj.Unplug(this);
            }
        }
	}

    public void Lock()
    {
        this.transform.parent = socketObj.transform;
        Destroy(GetComponent<Rigidbody>());
        lockedStatus = true;
    }

    public void Unlock()
    {
        gameObject.AddComponent<Rigidbody>();
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().mass = rbMass;
        GetComponent<Rigidbody>().drag = rbDrag;
        GetComponent<Rigidbody>().angularDrag = rbADrag;
        rigidBody = GetComponent<Rigidbody>();
        this.transform.parent = null;
        lockedStatus = false;
    }

	void Act ()
    {

    }

	public void Drop (ManipulatorController manipulator) {
        if (manipulator == attachedManipulator)
        {
            if (pluggedStatus == true)
            {
                socketObj.Unplug(this);
            }

            grabbedStatus = false;
            attachedManipulator.grabbedObj = null;
            attachedManipulator = null;
            interactionPoint.SetParent(transform, false);
        }
	}

	void PluggedMove () {
        transform.position = socketObj.socketOffset.position;
        transform.eulerAngles = socketObj.socketOffset.rotation.eulerAngles;
    }

	public void Plug () {
        GetComponent<Rigidbody>().isKinematic = true;
		GetComponent<Rigidbody>().useGravity = false;
        transform.position = Vector3.Lerp(this.transform.position, socketObj.transform.position, Time.deltaTime * 5);
        transform.rotation = Quaternion.Lerp(this.transform.rotation, socketObj.transform.rotation, Time.deltaTime * 5);
        pluggedStatus = true;
    }

	public void Grab (ManipulatorController manipulator) {
		attachedManipulator = manipulator;

		if (offsetStatus == true) {
			transform.rotation = Quaternion.Lerp(this.transform.rotation, offsetPoint.rotation, Time.deltaTime * 1);
			interactionPoint.position = offsetPoint.position;
			interactionPoint.rotation = offsetPoint.rotation;
		}

		if (offsetStatus == false) {
			interactionPoint.position = manipulator.transform.position;
			interactionPoint.rotation = manipulator.transform.rotation;
		}

		interactionPoint.SetParent(transform, true);
		grabbedStatus = true;
	}

	void GrabbedMove() {
        if (pluggedStatus == false) {

            posDelta = attachedManipulator.transform.position - interactionPoint.position;
            this.rigidBody.velocity = posDelta * velocityFactor * Time.fixedDeltaTime;

            rotDelta = attachedManipulator.transform.rotation * Quaternion.Inverse(interactionPoint.rotation);
            rotDelta.ToAngleAxis(out angle, out axis);

            if (angle > 180)
            {
                angle -= 360;
            }

            if (angle != 0 && axis != Vector3.zero) {
                this.rigidBody.angularVelocity = (Time.fixedDeltaTime * angle * axis) * rotationFactor;
            }
        }

        if (pluggedStatus == true) {
            PluggedMove();
        }
	}
}
