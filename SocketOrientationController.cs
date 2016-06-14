using UnityEngine;
using System.Collections;

public class SocketOrientationController : MonoBehaviour {

    SocketController thisParent;
	// Use this for initialization
	void Start () {
        thisParent = GetComponentInParent<SocketController>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider collided)
    {
        if (collided.name.Contains("OrientationTriggerBack"))
        {
            if (collided.GetComponentInParent<InteractiveObjectController>().attachedManipulator)
            {
                thisParent.plugObj = collided.GetComponentInParent<InteractiveObjectController>();
                thisParent.backPlugStatus = true;
            }
        }
    }

    void OnTriggerExit(Collider collided)
    {
        if (collided.name.Contains("OrientationTriggerBack"))
        {
            if (collided.GetComponentInParent<InteractiveObjectController>().attachedManipulator)
            {
                thisParent.backPlugStatus = false;
                //thisParent.plugObj = null;
            }
        }
    }
}
