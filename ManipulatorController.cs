using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ManipulatorController : MonoBehaviour {

	private bool grabStatus;

	private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
	private SteamVR_TrackedObject trackedObj; //assigns Vive controller to variable.

	private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip; 
	private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;

	private InteractiveObjectController closestObj = null;
    private InteractiveObjectController lastClosestObj = null;
    public InteractiveObjectController grabbedObj = null;
    public bool grippingStatus;
    public bool fistStatus;
    public float fistForceMultiplyer;
    public float maxFistForce;

	public HashSet<InteractiveObjectController> availableObjects = new HashSet<InteractiveObjectController>();

	void Start () {
		trackedObj = GetComponent<SteamVR_TrackedObject>();
        fistStatus = false;
        fistForceMultiplyer = 0;
	}

	void FixedUpdate ()
    {
        if (availableObjects.Count > 0)
        {
            Sort();
        }

        if (controller.GetPressDown(gripButton))
        {
            grippingStatus = true;

            if (availableObjects.Count != 0 && grabbedObj == null)
            {
                Grab(closestObj);
                grippingStatus = true;
            }
        }

		if (controller.GetPressUp(gripButton))
        {
            grippingStatus = false;

            if (grabbedObj)
            {
                Drop();
            }
		}

		if (controller.GetPressDown(triggerButton))
        {
            if (grabbedObj)
            {
                Act();
            }

            if (!grabbedObj && !fistStatus)
            {
                Fist();
            }
		}

        if (controller.GetPressUp(triggerButton))
        {
            if (grabbedObj)
            {
                StopAct();
            }

            if (!grabbedObj && fistStatus)
            {
                UnFist();
            }
        }
	}

    Vector3 GetDirection(GameObject target)
    {
        var heading = target.transform.position - this.transform.position;
        var distance = heading.magnitude;
        var direction = heading / distance;
        return direction;
    }

    void Fist()
    {
        GetComponent<Collider>().isTrigger = false;
        fistStatus = true;

        if (fistForceMultiplyer < maxFistForce)
        {
            fistForceMultiplyer = fistForceMultiplyer + 10;
        }
    }

    void UnFist ()
    {
        GetComponent<Collider>().isTrigger = true;
        fistStatus = false;
    }

	void Act ()
    {
		grabbedObj.actionStatus = true;
	}

    void StopAct()
    {
        grabbedObj.actionStatus = false;
    }

	public void Drop ()
    {
		grabbedObj.Drop(this);
		grabbedObj = null;
        grabStatus = false;
	}

    void Sort ()
    {
        float minDistance = float.MaxValue;
        float distance;
        lastClosestObj = closestObj;

        foreach (InteractiveObjectController item in availableObjects)
        {
            distance = (item.transform.position - transform.position).sqrMagnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                closestObj = item;
            }
        }

        if (closestObj != grabbedObj && closestObj.highlightedStatus == false && grabStatus == false && !closestObj.grabbedStatus)
        {
            Highlight(closestObj);
        }

        if (lastClosestObj != null && lastClosestObj.highlightedStatus == true && lastClosestObj != closestObj)
        {
            DeHighlight(lastClosestObj);
        }
    }

    void Highlight (InteractiveObjectController colorObj)
    {
        Color col = new Color(0.5f, 0.45f, 0);
        Renderer rend;
        rend = colorObj.GetComponent<Renderer>();
        rend.material.EnableKeyword("_EMISSION");
        rend.material.SetColor("_EmissionColor", col);
        colorObj.highlightedStatus = true;
    }

    void DeHighlight (InteractiveObjectController colorObj)
    {
        Renderer rend;
        rend = colorObj.GetComponent<Renderer>();
        rend.material.EnableKeyword("_EMISSION");
        rend.material.SetColor("_EmissionColor", Color.black);
        colorObj.highlightedStatus = false;
    }

	public void Grab (InteractiveObjectController sentObj)
	{
        grabbedObj = sentObj;
        grabStatus = true;
        DeHighlight(grabbedObj);

        if (grabbedObj.grabbedStatus == false) {
            grabbedObj.Grab(this);
        }

        if (grabbedObj.grabbedStatus == true && grabbedObj.attachedManipulator != this) {
            grabbedObj.Drop(grabbedObj.attachedManipulator);
            grabbedObj.Grab(this);
        }

	}

	void OnTriggerEnter (Collider collided)
    {
		InteractiveObjectController collidedObj = collided.GetComponent<InteractiveObjectController> ();

		if (collidedObj) {
			availableObjects.Add (collidedObj);
		}
	}

	void OnTriggerExit (Collider collided)
    {
		InteractiveObjectController collidedObj = collided.GetComponent<InteractiveObjectController> ();

		if (collidedObj) {
            DeHighlight(collidedObj);
			availableObjects.Remove (collidedObj);
		}
	}

    void OnCollisionEnter(Collision col)
    {
        Rigidbody hitBody = col.rigidbody;

        print("BLAM!");

        if (hitBody)
        {
            var direction = GetDirection(col.gameObject);
            hitBody.AddForce(direction * fistForceMultiplyer, ForceMode.Impulse);
            fistForceMultiplyer = 0;
        }
    }
}
