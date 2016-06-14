using UnityEngine;
using System.Collections;

public class InteractiveObjectController : MonoBehaviour
{
    public float velocityFactor = 20000f; //these two variables decide the movement/rotational characteristics and speed of the object
    public float rotationFactor = 600f;
    public int maxAV = 20;

    public bool grabbedStatus = false; //this variable is used by the manipulator it comes in contact to tell the object that it is being grabbed
    public bool pluggedStatus = false; //this variable tells the object that it has been placed in a socket
    public bool offsetStatus = false;  //this variable tells the object whether or not there is a preffered position and orientation that it should be held by.
    public bool actionStatus = false;

    private Rigidbody rigidBody; //these variables are used to determine the position and the rotation of the object once grabbed
    private Vector3 posDelta;
    private Quaternion rotDelta;
    private float angle;
    private Vector3 axis;

    public ManipulatorController attachedManipulator;
    public Transform interactionPoint;
    private Transform offsetPoint;

    public GameObject socketObj;
    private bool physStatus = true;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        interactionPoint = new GameObject().transform; //interaction point determines the point in space that the manipulator was at when it grabbed the object, relative to that object
        velocityFactor /= rigidBody.mass;
        rotationFactor /= rigidBody.mass;
        this.rigidBody.maxAngularVelocity = maxAV;
    }

    protected void FixedUpdate()
    {
        if (attachedManipulator && grabbedStatus && pluggedStatus == false)
        {
            GrabbedMove();
        }
        
        if (pluggedStatus == true)
        {
            PluggedMove();
        }

    }

    void PluggedMove()
    {
        if (grabbedStatus == true)
        {
            if (physStatus == false)
            {
                print("phys ressurected!");
                GetComponent<Rigidbody>().isKinematic = true;
                GetComponent<Rigidbody>().useGravity = false;
                physStatus = true;
            }
            this.transform.position = socketObj.transform.position;
            this.transform.rotation = socketObj.transform.rotation;
            //GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }

        if (grabbedStatus == false)
        {
            if (physStatus == true)
            {
                print("phys killed!");
                GetComponent<Rigidbody>().isKinematic = false;
                GetComponent<Rigidbody>().useGravity = true;
                physStatus = false;
            }


        }
    }
    
    public void Plug()
    {
        print("object Plug!");
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().useGravity = false;
        transform.position = Vector3.Lerp(this.transform.position, socketObj.transform.position, Time.deltaTime * 1);
        transform.rotation = Quaternion.Lerp(this.transform.rotation, socketObj.transform.rotation, Time.deltaTime * 1);
        socketObj.GetComponent<SocketController>().plugComplete = true;
        pluggedStatus = true;
    }

    public void SetOffset(ObjectOffsetController offsetObj)
    {
        offsetStatus = true;
        offsetPoint = offsetObj.transform;
    }
    
    public void Grab(ManipulatorController manipulator)
    {
        attachedManipulator = manipulator;

        if (offsetStatus == true)
        {
            transform.rotation = Quaternion.Lerp(this.transform.rotation, offsetPoint.rotation, Time.deltaTime * 1);
            interactionPoint.position = offsetPoint.position;
            interactionPoint.rotation = offsetPoint.rotation;
        }

        if (offsetStatus == false)
        {
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

    public void Release(ManipulatorController manipulator)
    {
        if (manipulator == attachedManipulator)
        {
            attachedManipulator = null;
            grabbedStatus = false;
            interactionPoint.SetParent(transform, false);
        }
    }

    public void Unplug()
    {
        this.GetComponent<Rigidbody>().isKinematic = false;
        this.GetComponent<Rigidbody>().useGravity = true;
        return;
    }
    /**
    private void OnDestroy()
    {
        Destroy(interactionPoint.gameObject);
    }
    **/
}