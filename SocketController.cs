using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SocketController : MonoBehaviour {

    private bool grabbedStatus = false;
    public bool plugComplete = false;
    public bool frontPlugStatus = false;
    public bool backPlugStatus = false;

    private float handY;
    private float lastHandY;
    private float maxY = 1.0f;
    private float minY = 0.4f;
    private float socketY;
   
    public InteractiveObjectController plugObj;
    public ManipulatorController manipulator;

    void Start ()
    {
    
    }

    // Update is called once per frame
    void Update ()
    {
        print("frontPlug = " + frontPlugStatus + " backPlug = " + backPlugStatus);

        if (frontPlugStatus == true && backPlugStatus == true)
        {
            if (plugComplete == false)
            {
                Plug();
            }
        }

        if (plugComplete == true)
        {
            //Debug.Log(manipulator.transform.localPosition.y);
            if (manipulator)
            {
                   
                grabbedStatus = plugObj.GetComponent<InteractiveObjectController>().grabbedStatus;

                if (grabbedStatus)
                {
                    //print("grabbedStatus = " + grabbedStatus);
                    PluggedMove();
                }

                if (!grabbedStatus)
                {
                    Unplug();
                }
            }
        }
        
    }

    public void Plug()
    {
        manipulator = plugObj.attachedManipulator;
        plugObj.socketObj = this.gameObject;
        plugObj.Plug();
    }

    void PluggedMove()
    {
        handY = manipulator.transform.localPosition.y;
        if (handY > lastHandY) { socketY = socketY + (handY - lastHandY); }
        if (handY < lastHandY) { socketY = socketY - (lastHandY - handY); }
        if (socketY > maxY) { socketY = maxY; }
        if (socketY < minY) { socketY = minY; }
        this.transform.localPosition = new Vector3((this.transform.localPosition.x), (socketY), (this.transform.localPosition.z));
        lastHandY = handY;
        print("hand = " + manipulator + "local y = " + manipulator.transform.localPosition.y);
    }

    void Unplug ()
    {
        plugObj.transform.parent = null;
        plugObj.socketObj = null;
        plugObj.GetComponent<Rigidbody>().isKinematic = false;
        plugObj.GetComponent<Rigidbody>().useGravity = true;
        //plugObj.GetComponent<BoxCollider>().isTrigger = false;
        plugObj.Grab(manipulator);
        plugObj.pluggedStatus = false;
        plugComplete = false;
        frontPlugStatus = false;
        backPlugStatus = false;
    }



    void OnTriggerEnter(Collider collided)
    {
        if (collided.name.Contains("OrientationTriggerFront"))
        {
            if (collided.GetComponentInParent<InteractiveObjectController>().attachedManipulator)
            {
                plugObj = collided.GetComponentInParent<InteractiveObjectController>();
                frontPlugStatus = true;
            }

            if ((!collided.GetComponentInParent<InteractiveObjectController>().attachedManipulator) && (plugObj == collided.GetComponentInParent<InteractiveObjectController>()))
            {
                Unplug();
            }
        }
    }

    void OnTriggerExit(Collider collided)
    {
        if (collided.name.Contains("OrientationTriggerFront"))
        {
            if (collided.GetComponentInParent<InteractiveObjectController>().attachedManipulator)
            {
                frontPlugStatus = false;
                //plugObj = null;
            }
        }
    }
}
