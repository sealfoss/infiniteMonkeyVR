using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ManipulatorConroller : MonoBehaviour
{
    //Variables specific to the SteamVR controller
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    private SteamVR_TrackedObject trackedObj;
    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;

    //variables for sorting through avaialable objects, and picking which one to grab
    public HashSet<InteractiveObjectController> availableObjects;
    private InteractiveObjectController closestObj;
    private InteractiveObjectController lastClosestObj;
    public InteractiveObjectController grabbedObj;

    //status bools
    public bool grabbingStatus;
    public bool actionStatus;

    //variables for determining where the grabbed object should be, in relation to the position of the manipulator that grabbed it
    public Transform interactionPoint;
    public Transform offsetPoint;
    private float lastTrackerPos;
    private float trackerPos;
    public float guidePos;
    public float guideRot;
    public float lockedRot;
    public float lastLockedRot;
    private Quaternion newRotation;
    private bool resetLockRot;

    void Start()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        availableObjects = new HashSet<InteractiveObjectController>();
        grabbingStatus = false;
        actionStatus = false;
        grabbedObj = null;
    }

    // we use fixed update because we're dealing with physics objects
    void FixedUpdate()
    {
        //if the user is grabbing, and has managed to grab an object, move that object with the manipulator.
        if (grabbingStatus && grabbedObj)
        {
            Move();
        }

        //if user releases the trigger button, end action.
        if (controller.GetPressUp(triggerButton))
        {
            EndAct();
        }

        //if user pulls the trigger button, initiate action.
        if (controller.GetPressDown(triggerButton))
        {
            Act();
        }

        //if the user releases the grip button, release any grabbed objects.
        if (controller.GetPressUp(gripButton))
        {
            grabbingStatus = false;

            if (grabbedObj)
            {
                Drop();
            }
        }

        //if user presses controller grip button, start grabbing
        if (controller.GetPressDown(gripButton))
        {
            grabbingStatus = true;

            //if there's something to grab (if the controller has collided with an interactive object), grab the closest one.
            if (!grabbedObj && availableObjects.Count != 0)
            {
                Grab(closestObj);
            }
        }

        if (availableObjects.Count > 0)
        {
            //In order to prevent grabbing more than one object at a time, the closest object of all objects colliding with the manipulator is calculated.
            Sort();
        }
    }

    void Act()
    {
        actionStatus = true;
    }

    void EndAct()
    {
        actionStatus = false;
    }

    void BuildJoint()
    {

    }

    void JointedMove()
    {

    }

    void GuidedMove()
    {
        //just so it doesn't thrown me an error, i'm checking to make sure there's something grabbed, before initiating the guided move.  even though there already should be.
        if (grabbedObj)
        {
            //guided move works by calculating the object's correct position based on the position of the object it is plugged into.
            GameObject grabbedParent = grabbedObj.transform.parent.gameObject;
            //a new transform object is used to caluclate the manipulator's position, from the point of view of the grabbed object's parent.
            Transform tracker = new GameObject().transform;
            tracker.gameObject.name = "GuidedMoveTracker";
            tracker.position = this.transform.position;
            tracker.rotation = this.transform.rotation;
            tracker.parent = grabbedParent.transform;

            //depending on the object, movement is constrained to a specific axis.
            switch (grabbedObj.moveAxisCondition)
            {
                case 1:
                    grabbedObj.transform.localPosition = GuidedMoveX(tracker);
                    break;

                case 2:
                    grabbedObj.transform.localPosition = GuidedMoveY(tracker);
                    break;

                case 3:
                    grabbedObj.transform.localPosition = GuidedMoveZ(tracker);
                    break;
            }

            //as is rotation
            switch (grabbedObj.rotAxisCondition)
            {
                case 1:
                    grabbedObj.transform.localRotation = GuidedRotX(tracker);
                    break;

                case 2:
                    grabbedObj.transform.localRotation = GuidedRotY(tracker);
                    break;

                case 3:
                    grabbedObj.transform.localRotation = GuidedRotZ(tracker);
                    break;
            }

            //when we're done with the tracker object, it is destroyed.  gotta keep the scene tidy.
            Destroy(tracker.gameObject);
        }
    }
    
    Vector3 GuidedMoveX(Transform tracker)
    {
        //these same steps apply to all of the GuidedMove(Axix) functions
        Vector3 newPosition;
        //the spot that you want to move the guided object to is the manipulator's postion - the position of the interaction point.
        // if you just track the manipulator's position, without accounting for the interaction point, you'll end up grabbing the object at it's origin.
        trackerPos = tracker.localPosition.x - interactionPoint.localPosition.x;
        guidePos = trackerPos;

        //constrain the movement within min and max values
        if (trackerPos > grabbedObj.maxPos)
        {
            guidePos = grabbedObj.maxPos;
        }

        if (trackerPos < grabbedObj.minPos)
        {
            guidePos = grabbedObj.minPos;
        }

        //package up the new position and send it back to the guided move function
        newPosition = new Vector3(guidePos, grabbedObj.transform.localPosition.y, grabbedObj.transform.localPosition.z);
        return (newPosition);
    }

    Vector3 GuidedMoveY(Transform tracker)
    {
        Vector3 newPosition;
        trackerPos = tracker.localPosition.z - interactionPoint.localPosition.z;
        guidePos = trackerPos;

        if (trackerPos > grabbedObj.maxPos)
        {
            guidePos = grabbedObj.maxPos;
        }

        if (trackerPos < grabbedObj.minPos)
        {
            guidePos = grabbedObj.minPos;
        }

        newPosition = new Vector3(grabbedObj.transform.localPosition.x, guidePos, grabbedObj.transform.localPosition.z);
        return (newPosition);
    }

    Vector3 GuidedMoveZ(Transform tracker)
    {
        Vector3 newPosition;
        trackerPos = tracker.localPosition.z - interactionPoint.localPosition.z;
        guidePos = trackerPos;

        if (trackerPos > grabbedObj.maxPos)
        {
            guidePos = grabbedObj.maxPos;
        }

        if (trackerPos < grabbedObj.minPos)
        {
            guidePos = grabbedObj.minPos;
        }

        newPosition = new Vector3(grabbedObj.transform.localPosition.x, grabbedObj.transform.localPosition.y, guidePos);
        return (newPosition);
    }

    Quaternion GuidedRotX(Transform tracker)
    {
        //these same steps apply to all the other GuidedRot(Axis) functions.
        //get the rotation of the manipulator, on the desired axis.
        guideRot = tracker.localEulerAngles.x;

        //normalize the rotation, so the object dosn't flip.
        if (guideRot > 180)
        {
            guideRot -= 360;
        }

        //constrain rotation within min and max values
        if (guideRot > grabbedObj.maxRot)
        {
            guideRot = grabbedObj.maxRot;
        }

        if (guideRot < grabbedObj.minRot)
        {
            guideRot = grabbedObj.minRot;
        }

        //package up the new roation in a quaternion, and send it back to the guided move function.
        newRotation = Quaternion.Euler(guideRot, grabbedObj.transform.localEulerAngles.y, grabbedObj.transform.localEulerAngles.z);

        //i wanted to keep the system flexible, so you may actually want the inverse rotation, depending on the plug and its orientation related to the object
        if (grabbedObj.plugObj.inverseRotation)
        {
            newRotation = Quaternion.Inverse(newRotation);
        }

        return (newRotation);
    }

    Quaternion GuidedRotY(Transform tracker)
    {
        guideRot = tracker.localEulerAngles.y;

        if (guideRot > 180)
        {
            guideRot -= 360;
        }

        if (guideRot > grabbedObj.maxRot)
        {
            guideRot = grabbedObj.maxRot;
        }

        if (guideRot < grabbedObj.minRot)
        {
            guideRot = grabbedObj.minRot;
        }

        newRotation = Quaternion.Euler(grabbedObj.transform.localEulerAngles.x, guideRot, grabbedObj.transform.localEulerAngles.z);

        if (grabbedObj.plugObj.inverseRotation)
        {
            newRotation = Quaternion.Inverse(newRotation);
        }

        return (newRotation);
    }

    Quaternion GuidedRotZ(Transform tracker)
    {
        //i just want you to know, maybe i'm remedial, but this logic was crazy hard to figure out
        lockedRot = tracker.localEulerAngles.z;

        //first, if the lock rotation has been reset, set the last lock rotation to the rotation of the tracker, so that it will have no influence
        if (resetLockRot)
        {
            lastLockedRot = lockedRot;
            resetLockRot = false;
        }

        //in unity, if euler angle values go over 360, they're automatically reset and start over at zero
        //if the value goes under 0, it gets reset and starts over at 360
        //this is why they tell you not to increment euler values, but I didn't really have a choice here (at least, none that I can see)
        //so, we have to detect if the rotation has been reset by going over 360 degrees or under 0 degrees, and account for that
        //fudged the values by 10 on either side of the threshold, to give the system some wiggle room
        if (lockedRot < 10 && lastLockedRot > 350)
        {
            lastLockedRot = lastLockedRot - 360;
        }

        if (lockedRot > 350 && lastLockedRot < 10)
        {
            lastLockedRot = lastLockedRot + 360;
        }

        //if the object is being rotated in a positive direction, add to the object's rotation amount
        if (lockedRot > lastLockedRot)
        {
            grabbedObj.rotationAmount = grabbedObj.rotationAmount - (lastLockedRot - lockedRot);

            // if the rotation amount is above 360 (the limit for euler rotations in unity), reset it, and add to the revolution count, up to the max revolution value
            if (grabbedObj.rotationAmount > 360)
            {
                grabbedObj.rotationAmount = grabbedObj.rotationAmount - 360;

                //check to see if the object is in a locked position, and revolutions need to be counted
                if (grabbedObj.plugObj.lockPosStatus)
                {
                    //count revolutions, as needed
                    if (!grabbedObj.plugObj.inverseRotation && grabbedObj.revolutions < grabbedObj.maxRevolutions)
                    {
                        grabbedObj.revolutions = grabbedObj.revolutions + 1;
                    }

                    //if the rotation is inversed, we'll need the opposite of this operation, down to zero
                    if (grabbedObj.plugObj.inverseRotation)
                    {
                        grabbedObj.revolutions = grabbedObj.revolutions - 1;

                        if (grabbedObj.revolutions < 0)
                        {
                            grabbedObj.revolutions = 0;
                        }
                    }
                }
            }
        }

        //if the object is being rotated in a negative rotation, subtract fromt the object's rotation amount
        if (lockedRot < lastLockedRot)
        {
            grabbedObj.rotationAmount = grabbedObj.rotationAmount - (lastLockedRot - lockedRot);

            //if rotation amount goes below 0, reset it, and subtract from the revolution count, but don't go under zero
            if (grabbedObj.rotationAmount < 0)
            {
                grabbedObj.rotationAmount = grabbedObj.rotationAmount + 360;

                //check to see if the object is in a locked position, and revolutions need to be counted
                if (grabbedObj.plugObj.lockPosStatus)
                {
                    //check to see if the object is in a locked position, and revolutions need to be counted
                    if (!grabbedObj.plugObj.inverseRotation)
                    {
                        grabbedObj.revolutions = grabbedObj.revolutions - 1;

                        if (grabbedObj.revolutions < 0)
                        {
                            grabbedObj.revolutions = 0;
                        }
                    }

                    //if the rotation is inversed, we'll need the opposite of this operation, up to max revolutions
                    if (grabbedObj.plugObj.inverseRotation && grabbedObj.revolutions < grabbedObj.maxRevolutions)
                    {
                        grabbedObj.revolutions = grabbedObj.revolutions + 1;
                    }
                }
            }
        }

        //after all that, we'll save what come out to a variable
        guideRot = grabbedObj.rotationAmount;

        //if the rotation is inversed, we'll get the negative value
        if (grabbedObj.plugObj.inverseRotation)
        {
            guideRot = guideRot * -1;
        }

        //and we'll return the guideRot as the component value for a local euler angles operation, so long as the stop rotation boolean isn't active
        if (!grabbedObj.stopRot)
        {
            newRotation = Quaternion.Euler(grabbedObj.transform.localEulerAngles.x, grabbedObj.transform.localEulerAngles.y, guideRot);
        }

        //if stop rotation is active, though, we'll just keep the object in place by giving it it's own rotation
        if(grabbedObj.stopRot)
        {
            newRotation = Quaternion.Euler(grabbedObj.transform.localEulerAngles.x, grabbedObj.transform.localEulerAngles.y, grabbedObj.transform.localEulerAngles.z);
        }

        lastLockedRot = lockedRot;
        return (newRotation);
    }

    void GrabbedMove()
    {
        float angle;
        Vector3 axis;
        Vector3 posDelta;
        Quaternion rotDelta;

        //establish the direction the grabbed object needs to be moving in, in order to place it "in" the grabbing manipulator
        posDelta = this.transform.position - interactionPoint.position;
        //set the grabbed object's velocity so that it is constantly moving towards where it should be as a grabbed object
        grabbedObj.rigidBody.velocity = posDelta * grabbedObj.velocityFactor * Time.fixedDeltaTime;

        //establish the object's appropriate rotation based on the position of the manipulator and the object's interactionPoint
        rotDelta = this.transform.rotation * Quaternion.Inverse(interactionPoint.rotation);
        rotDelta.ToAngleAxis(out angle, out axis);

        //establish the shortest route towards appropriate rotation, clockwise or counter-clockwise
        if (angle > 180)
        {
            angle -= 360;
        }

        //make sure the object isn't already in the appropriate rotation
        if (angle != 0 && axis != Vector3.zero)
        {
            //set the object's angular velocity so that the object is rotating towards where it should be as a grabbed object
            grabbedObj.rigidBody.angularVelocity = (Time.fixedDeltaTime * angle * axis) * grabbedObj.rotationFactor;
        }
    }

    void Move()
    {
        //check how far the grabbed object is from the manipulator
        float dist = Vector3.Distance(grabbedObj.transform.position, this.transform.position);

        //check to make sure grabbed object and manipulator are within appropriate distance of each other.  if not, drop the object.
        if (dist > grabbedObj.maxDistance)
        {
            Drop();
        }

        //default movement method is grabbed move
        if (!grabbedObj.guidedMove && !grabbedObj.jointedMove)
        {
            GrabbedMove();
        }

        //if object is socketed or an appendage of another object, it moves via guided move method
        if (grabbedObj.guidedMove)
        {
            GuidedMove();
        }

        //if object is meant to be used in a highly dynamic manner, it will move via the jointed move method
        if (grabbedObj.jointedMove)
        {
            JointedMove();
        }
    }

    public void Drop()
    {
        //if dropped object is plugged, but not locked, unplug the object before dropping
        if (grabbedObj.plugObj && !grabbedObj.lockedStatus)
        {
            grabbedObj.plugObj.Unplug(grabbedObj);
        }

        Destroy(interactionPoint.gameObject);
        //tell the object it is no longer grabbed.
        grabbedObj.grabbedStatus = false;
        //if the object is plugged but not locked, unplug it
        //remove the attached manipulator from the object
        grabbedObj.attachedManipulator = null;
        //clear out the grabbed object variable.
        grabbedObj = null;

        resetLockRot = true;
    }

    public void Grab(InteractiveObjectController grabObj)
    {
        InteractiveObjectController parent;

        grabbedObj = grabObj;
        Unhighlight(grabbedObj);

        if (!grabbedObj.jointedMove)
        {
            if (grabbedObj.grabbedStatus)
            {
                //if the grabbed object is already grabbed by the other manipulator, that manipulator is told to drop the object
                grabbedObj.attachedManipulator.Drop();
                grabbedObj.grabbedStatus = false;
            }

            grabbedObj.attachedManipulator = this;

            //interactionPoint is used to calculate where the object should be
            interactionPoint = new GameObject().transform;

            //if there's an offset, the interaction point position and rotation are set to it
            if (grabbedObj.offsetPoint)
            {
                grabbedObj.transform.rotation = Quaternion.Lerp(this.transform.rotation, grabbedObj.offsetPoint.rotation, Time.deltaTime * 1);
                interactionPoint.position = grabbedObj.offsetPoint.position;
                interactionPoint.rotation = grabbedObj.offsetPoint.rotation;
            }

            //if there is no offset, interaction point and position are set to wherever the manipulator was when it grabbed the object
            if (!grabbedObj.offsetPoint)
            {
                interactionPoint.position = this.transform.position;
                interactionPoint.rotation = this.transform.rotation;
            }

            //check to see if the grabbed object is a child of another object
            parent = grabbedObj.GetComponentInParent<InteractiveObjectController>();
            if (parent)
            {
                interactionPoint.SetParent(parent.transform, true);
            }
            //interactionPoint's local values need to be from the point of view of the grabbed object

            if (!parent)
            {
                interactionPoint.SetParent(grabbedObj.transform, true);
            }
        }

        if (grabbedObj.jointedMove)
        {
            BuildJoint();
        }

        grabbedObj.grabbedStatus = true;
    }

    void Sort()
    {
        float minDistance = float.MaxValue;
        float distance;
        lastClosestObj = closestObj;
        
        //Goes through avialable objects and decides which one is closest to the manipulator, every frame that the manipuator has collided with one or more interactive objects
        foreach (InteractiveObjectController item in availableObjects)
        {
            distance = (item.transform.position - transform.position).sqrMagnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                closestObj = item;
            }
        }

        if (closestObj != grabbedObj && closestObj.highlightedStatus == false && closestObj.grabbedStatus == false)
        {
            Highlight(closestObj); //closest object is highlighted, so user knows what they're going to pick up with they initiate grab.
        }

        if (lastClosestObj != null && lastClosestObj.highlightedStatus == true && lastClosestObj != closestObj)
        {
            Unhighlight(lastClosestObj); //unhighlights objects that are no longer closest to the manipulator, or grabbed.
        }
    }

    void Highlight(InteractiveObjectController colorObj)
    {
        Color col = new Color(0.5f, 0.45f, 0);
        Renderer rend;
        rend = colorObj.GetComponent<Renderer>();
        rend.material.EnableKeyword("_EMISSION");
        rend.material.SetColor("_EmissionColor", col);
        colorObj.highlightedStatus = true;
    }

    void Unhighlight(InteractiveObjectController colorObj)
    {
        Renderer rend;
        rend = colorObj.GetComponent<Renderer>();
        rend.material.EnableKeyword("_EMISSION");
        rend.material.SetColor("_EmissionColor", Color.black);
        colorObj.highlightedStatus = false;
    }

    void OnTriggerEnter(Collider collided)
    {
        //when the manipulator collides with an object, check to see if the object is interactive
        InteractiveObjectController collidedObj = collided.GetComponent<InteractiveObjectController>();

        if (collidedObj)
        {
            //if the manipulator is grabbing, but hasn't grabbed anything, "catch" the colliding object
            if (grabbingStatus && !grabbedObj)
            {
                Grab(collidedObj);
                //also add object to list of available objects to be grabbed, in case the caught object is dropped
                availableObjects.Add(collidedObj);
            }

            if (!grabbingStatus)
            {
                //if the manipulator isn't currently grabbing, add collided object to the list of available objects to be grabbed
                availableObjects.Add(collidedObj);
            }
        }
    }

    void OnTriggerExit(Collider collided)
    {
        //if an object stops colliding with the manipulator, check to see if it is interactive
        InteractiveObjectController collidedObj = collided.GetComponent<InteractiveObjectController>();

        if (collidedObj)
        {
            //if the object is interactive, assume that it was already added to the list of available objects and remove it from that list
            Unhighlight(collidedObj);
            availableObjects.Remove(collidedObj);
        }
    }
}