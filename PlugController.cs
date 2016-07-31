using UnityEngine;
using System.Collections;

public class PlugController : MonoBehaviour {
    public bool oneHandPlug;
    public Transform guide;
    public InteractiveObjectController pluggedObj;
    public string soughtForName;
    public bool pluggedStatus;
    public float unplugPos;
    public bool lockPosStatus;
    public bool lockRotStatus;
    public bool inverseRotation;

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (pluggedObj)
        {
            if (!pluggedStatus)
            {
                Plug(pluggedObj);
            }

            if (pluggedObj.attachedManipulator)
            {
                if (pluggedObj.transform.localPosition.x >= unplugPos || pluggedObj.transform.localPosition.y >= unplugPos || pluggedObj.transform.localPosition.z >= unplugPos)
                {
                    Unplug(pluggedObj);
                }
            }
        }
	}

    public void Unplug(InteractiveObjectController unpluggingObj)
    {
        //we need to unparent the object
        unpluggingObj.transform.parent = null;
        //when unplugged, the object's rigidbody will need to be rebuilt
        unpluggingObj.gameObject.AddComponent<Rigidbody>();
        unpluggingObj.GetComponent<Rigidbody>().mass = unpluggingObj.rbMass;
        unpluggingObj.GetComponent<Rigidbody>().drag = unpluggingObj.rbDrag;
        unpluggingObj.GetComponent<Rigidbody>().angularDrag = unpluggingObj.rbADrag;
        unpluggingObj.rigidBody = unpluggingObj.GetComponent<Rigidbody>();
        //re-enable gravity and other physical forces
        unpluggingObj.GetComponent<Rigidbody>().isKinematic = false;
        unpluggingObj.GetComponent<Rigidbody>().useGravity = true;
        //re-enable grabbed move
        unpluggingObj.guidedMove = false;
        //reset the object
        unpluggingObj.pluggedStatus = false;
        unpluggingObj.plugObj = null;
        //reset the plug
        pluggedStatus = false;
        pluggedObj = null;
    }

    public void Plug(InteractiveObjectController pluggingObj)
    {
        //first, we need to get the plug's parent object
        GameObject plugParent = this.transform.parent.gameObject;
        //objects with rigid bodies containing child objects with rigidbodies is a big no-no, so we'll need to destroy the grabbed object's rigid body.
        Destroy(pluggingObj.GetComponent<Rigidbody>());
        //and then we make the object and the plug children of the same object
        pluggingObj.transform.parent = plugParent.transform;
        //move grabbed object to guided position and rotation
        pluggingObj.transform.position = guide.position;
        pluggingObj.transform.rotation = guide.rotation;
        //end grabbed move, enable switch to guided move
        pluggingObj.guidedMove = true;
        //keep a copy of the plug for when we have to unplug
        pluggingObj.plugObj = this;
        //tell the object it has been plugged
        pluggingObj.pluggedStatus = true;
        //tell the plug that everything is good to go
        pluggedStatus = true;
    }

    public void Reset ()
    {
        pluggedObj = null;
        pluggedStatus = false;
    }

    void OnTriggerEnter(Collider collided)
    {
        InteractiveObjectController collidedObj = collided.GetComponent<InteractiveObjectController>();
        
        if (collidedObj)
        {
            if (collidedObj.name == soughtForName)
            {
                if ((collidedObj.grabbedStatus && !oneHandPlug) || oneHandPlug)
                {
                    pluggedObj = collidedObj;
                }
            }
        }
    }

    void OnTriggerExit(Collider collided)
    {
    }
}
