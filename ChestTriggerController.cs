using UnityEngine;
using System.Collections;

public class ChestTriggerController : MonoBehaviour {
    public bool triggered = false;
    InteractiveObjectController collidedObj = null;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnTriggerEnter (Collider collided)
    {
        if (collided.name == "BronzeLock_Hook")//(collided != collidedObj)
        {
            if (collided != collidedObj)
            {
                triggered = true;
            }

            collidedObj = collided.GetComponent<InteractiveObjectController>();
        }
    }

    void OnTriggerExit(Collider collided)
    {
        if (collided == collidedObj)
        {
            collidedObj = null;
            triggered = false;
        }
    }
}
