//Just so everybody is real clear on this, my Grab() and GrabbedMove() functions in this script were lifted from Newton VR
//Newton VR is available here: https://github.com/TomorrowTodayLabs/NewtonVR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ManipulatorController : MonoBehaviour
{
    //Variables specific to the SteamVR controller
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    public SteamVR_TrackedObject trackedObj;
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
    public float trackerRot;
    public float lastTrackerRot;
    private bool resetTrackerRot;

    void Start()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        availableObjects = new HashSet<InteractiveObjectController>();
        grabbingStatus = false;
        actionStatus = false;
        grabbedObj = null;
    }

    // we use fixed update because we're dealing with physics objects
    void Update()
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

    public void Vibrate(ushort duration)
    {
        SteamVR_Controller.Input((int)trackedObj.index).TriggerHapticPulse(duration);
    }

    void Act()
    {
        actionStatus = true;
    }

    void EndAct()
    {
        actionStatus = false;
    }

    Quaternion GetDelta(Transform objectOne, Transform objectTwo)
    {
        Vector3 delta = objectOne.position - objectTwo.position;
        Quaternion look = Quaternion.LookRotation(delta);
        return (look);
    }

    void GuidedMove()
    {
        //just so it doesn't thrown me an error, i'm checking to make sure there's something grabbed, before initiating the guided move.  even though there already should be.
        if (grabbedObj)
        {
            //guided move works by calculating the object's correct position and rotation based on the position of the object it is plugged into.
            GameObject grabbedParent = grabbedObj.transform.parent.gameObject;
            //a new transform object is used to caluclate the manipulator's position, from the point of view of the grabbed object's parent.
            Transform tracker = new GameObject().transform;
            //gave the tracker a name for greater visibility in the scene
            tracker.gameObject.name = "GuidedMoveTracker";
            tracker.position = this.transform.position;
            tracker.rotation = this.transform.rotation;
            tracker.parent = grabbedParent.transform;

            //first, check if the object is even able to move
            if (grabbedObj.moveAxisCondition != 0)
            {
                //depending on the object, movement is constrained to a specific axis.
                switch (grabbedObj.moveAxisCondition)
                {
                    case 1:
                        trackerPos = tracker.localPosition.x - interactionPoint.localPosition.x;
                        break;

                    case 2:
                        trackerPos = tracker.localPosition.y - interactionPoint.localPosition.y;
                        break;

                    case 3:
                        trackerPos = tracker.localPosition.z - interactionPoint.localPosition.z;
                        break;
                }

                //the spot that you want to move the guided object to is the manipulator's postion - the position of the interaction point.
                // if you just track the manipulator's position, without accounting for the interaction point, you'll end up grabbing the object at it's origin.
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

                //create new position, depending on which axis we're working on
                switch (grabbedObj.moveAxisCondition)
                {
                    case 1:

                        if (grabbedObj.moveResistance >= 1)
                        {
                            Vector3 moveTo = new Vector3(guidePos, grabbedObj.transform.localPosition.y, grabbedObj.transform.localPosition.z);
                            grabbedObj.transform.localPosition = Vector3.Lerp(grabbedObj.transform.localPosition, moveTo, Time.deltaTime / grabbedObj.moveResistance);
                        }

                        else
                        {
                            grabbedObj.transform.localPosition = new Vector3(guidePos, grabbedObj.transform.localPosition.y, grabbedObj.transform.localPosition.z);
                        }

                        break;

                    case 2:

                        if (grabbedObj.moveResistance >= 1)
                        {
                            Vector3 moveTo = new Vector3(grabbedObj.transform.localPosition.x, guidePos, grabbedObj.transform.localPosition.y);
                            grabbedObj.transform.localPosition = Vector3.Lerp(grabbedObj.transform.localPosition, moveTo, Time.deltaTime / grabbedObj.moveResistance);
                        }

                        else
                        {
                            grabbedObj.transform.localPosition = new Vector3(grabbedObj.transform.localPosition.x, guidePos, grabbedObj.transform.localPosition.y);
                        }

                        break;

                    case 3:
                        //resist movment, if applicable
                        if (grabbedObj.moveResistance >= 1)
                        {
                            Vector3 moveTo = new Vector3(grabbedObj.transform.localPosition.x, grabbedObj.transform.localPosition.y, guidePos);
                            grabbedObj.transform.localPosition = Vector3.Lerp(grabbedObj.transform.localPosition, moveTo, Time.deltaTime / grabbedObj.moveResistance);
                        }

                        else
                        {
                            grabbedObj.transform.localPosition = new Vector3(grabbedObj.transform.localPosition.x, grabbedObj.transform.localPosition.y, guidePos);
                        }

                        break;
                }
            }

            //dealing with rotation isn't nearly as straight forward
            if (grabbedObj.rotAxisCondition != 0)
            {
                //decide which axis we're working on, first
                switch (grabbedObj.rotAxisCondition)
                {
                    case 1:
                        trackerRot = tracker.localEulerAngles.x;
                        break;

                    case 2:
                        trackerRot = tracker.localEulerAngles.y;
                        break;

                    case 3:
                        trackerRot = tracker.localEulerAngles.z;
                        break;
                }

                //reset last tracker rotation, if needed
                if (resetTrackerRot)
                {
                    lastTrackerRot = trackerRot;
                    resetTrackerRot = false;
                }

                //in unity, if euler angle values go over 360, they're automatically reset and start over at zero
                //if the value goes under 0, it gets reset and starts over at 360
                //this is why they tell you not to increment euler angle values, but I didn't really have a choice here (at least, none that I can see)
                //so, we have to detect if the rotation has been reset by going over 360 degrees or under 0 degrees, and account for that
                //fudged the values by 10 on either side of the threshold, to give the system some wiggle room
                if (trackerRot < 10 && lastTrackerRot > 350)
                {
                    lastTrackerRot = lastTrackerRot - 360;
                }

                if (trackerRot > 350 && lastTrackerRot < 10)
                {
                    lastTrackerRot = lastTrackerRot + 360;
                }

                //if the object is being rotated in a positive direction, add to the object's rotation amount
                if (trackerRot > lastTrackerRot)
                {
                    grabbedObj.rotationAmount = grabbedObj.rotationAmount - (lastTrackerRot - trackerRot);

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
                if (trackerRot < lastTrackerRot)
                {
                    grabbedObj.rotationAmount = grabbedObj.rotationAmount - (lastTrackerRot - trackerRot);

                    //if rotation amount goes below 0, reset it, and subtract from the revolution count, but don't go under zero
                    if (grabbedObj.rotationAmount < 0)
                    {
                        grabbedObj.rotationAmount = grabbedObj.rotationAmount + 360;

                        //check to see if the object is in a locked position, and revolutions need to be counted
                        if (grabbedObj.plugObj.lockPosStatus)
                        {
                            //which direction is positive rotation depends on whether the rotation is inversed, as does the revolution count
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

                //after all that, we'll save what come out to a local variable
                guideRot = grabbedObj.rotationAmount;

                //if the rotation is inversed, we'll need the negative value
                if (grabbedObj.plugObj.inverseRotation)
                {
                    guideRot = guideRot * -1;
                }

                //and we'll return the guideRot as the component value for a local euler angles operation, so long as the stop rotation boolean isn't active
                if (!grabbedObj.stopRot)
                {
                    switch (grabbedObj.rotAxisCondition)
                    {
                        case 1:
                            grabbedObj.transform.localRotation = Quaternion.Euler(guideRot, grabbedObj.transform.localEulerAngles.y, grabbedObj.transform.localEulerAngles.z);
                            break;

                        case 2:
                            grabbedObj.transform.localRotation = Quaternion.Euler(grabbedObj.transform.localEulerAngles.x, guideRot, grabbedObj.transform.localEulerAngles.z);
                            break;

                        case 3:
                            grabbedObj.transform.localRotation = Quaternion.Euler(grabbedObj.transform.localEulerAngles.x, grabbedObj.transform.localEulerAngles.y, guideRot);
                            break;
                    }
                }

                lastTrackerRot = trackerRot;
            }

            //when we're done with the tracker object, it is destroyed.  gotta keep the scene tidy.
            Destroy(tracker.gameObject);
        }
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
        if (!grabbedObj.secondaryManipulator)
        {
            rotDelta = this.transform.rotation * Quaternion.Inverse(interactionPoint.rotation);
            rotDelta.ToAngleAxis(out angle, out axis);

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

        if (grabbedObj.secondaryManipulator)
        {
            Transform secondaryTrans = new GameObject().transform;
            secondaryTrans.rotation = GetDelta(grabbedObj.handleTwo.transform, this.transform);
            secondaryTrans.transform.eulerAngles = new Vector3(secondaryTrans.eulerAngles.x, secondaryTrans.eulerAngles.y, this.transform.eulerAngles.z);
            rotDelta = secondaryTrans.rotation * Quaternion.Inverse(interactionPoint.rotation);
            rotDelta.ToAngleAxis(out angle, out axis);

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

            Destroy(secondaryTrans.gameObject);
        }
    }

    void Move()
    {
        //i put this check in just to keep it from throwing me errors, seems it can activate the move function even when there isn't an object attached sometimes, not sure why
        if (grabbedObj)
        {
            //check how far the grabbed object is from the manipulator
            float dist = Vector3.Distance(grabbedObj.transform.position, this.transform.position);

            //check to make sure grabbed object and manipulator are within appropriate distance of each other.  if not, drop the object.
            if (dist > grabbedObj.maxDistance)
            {
                Drop();
            }

            //default movement method is grabbed move
            if (!grabbedObj.guidedMove && grabbedObj.secondaryManipulator != this)
            {
                GrabbedMove();
            }

            //if object is socketed or an appendage of another object, it moves via guided move method
            if (grabbedObj.guidedMove)
            {
                GuidedMove();
            }
        }
    }

    public void Drop()
    {
        if (grabbedObj.secondaryManipulator)
        {
            if (this == grabbedObj.secondaryManipulator)
            {
                grabbedObj.velocityFactor = grabbedObj.velocityFactor / 2;
                grabbedObj.rotationFactor = grabbedObj.rotationFactor / 2;
                grabbedObj.handleTwo.transform.parent = grabbedObj.transform;
                grabbedObj.handleTwo.transform.localPosition = grabbedObj.handleTwoInitPos;
                grabbedObj.handleTwo.transform.localRotation = Quaternion.identity;
                grabbedObj.handleTwo.transform.localRotation = grabbedObj.handleTwoInitRot;
                grabbedObj.secondaryManipulator = null;
                grabbedObj = null;
            }

            else
            {
                grabbedObj.attachedManipulator = null;
                grabbedObj.grabbedStatus = false;
                grabbedObj.secondaryManipulator.Grab(grabbedObj);
                grabbedObj.secondaryManipulator = null;
                grabbedObj = null;
                Destroy(interactionPoint.gameObject);
            }
        }

        else
        {
            //if dropped object is plugged, but not locked, unplug the object before dropping
            if (grabbedObj.plugObj && !grabbedObj.lockedStatus)
            {
                grabbedObj.plugObj.Unplug(grabbedObj);
            }

            grabbedObj.attachedManipulator = null;
            Destroy(interactionPoint.gameObject);
            //tell the object it is no longer grabbed.
            grabbedObj.grabbedStatus = false;
            //clear out the grabbed object variable.
            grabbedObj = null;
            //no more object to track, so reset it
            resetTrackerRot = true;
        }
    }

    public void Grab(InteractiveObjectController grabObj)
    {
        if (grabObj)
        {
            grabbedObj = grabObj;
            Unhighlight(grabbedObj);

            if (grabbedObj.twoHanded)
            {
                if (grabbedObj.grabbedStatus)
                {
                    grabbedObj.velocityFactor = grabbedObj.velocityFactor * 2;
                    grabbedObj.rotationFactor = grabbedObj.rotationFactor * 2;
                    grabbedObj.handleTwo.position = this.transform.position;
                    grabbedObj.handleTwo.transform.parent = this.transform;
                    grabbedObj.secondaryManipulator = this;
                }

                else
                {
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

                    interactionPoint.SetParent(grabbedObj.transform, true);
                    grabbedObj.grabbedStatus = true;
                }
            }

            if (!grabbedObj.twoHanded)
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

                interactionPoint.SetParent(grabbedObj.transform, true);
                grabbedObj.grabbedStatus = true;
            }
        }
    }

    void Sort()
    {
        float minDistance = float.MaxValue;
        float distance;
        lastClosestObj = closestObj;

        //Goes through avialable objects and decides which one is closest to the manipulator, every frame that the manipuator has collided with one or more interactive objects
        foreach (InteractiveObjectController item in availableObjects)
        {
            //throwing in another check that there are actually items in available objects, because sometimes it seems there aren't by the time this functiong gets called
            if (availableObjects.Count > 0)
            {
                distance = (item.transform.position - transform.position).sqrMagnitude;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestObj = item;
                }
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
        if (colorObj)
        {
            Renderer rend;
            rend = colorObj.GetComponent<Renderer>();
            rend.material.EnableKeyword("_EMISSION");
            rend.material.SetColor("_EmissionColor", Color.black);
            colorObj.highlightedStatus = false;
        }
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
                //if you're grabbing the child object of a larger assembly which has been dropped, grab the larger assembly instead
                if (collidedObj.orphanStatus)
                {
                    Grab(collidedObj.parentObj);
                    availableObjects.Add(collidedObj.parentObj);
                }

                //otherwise, just grab the object
                else
                {
                    Grab(collidedObj);
                    //also add object to list of available objects to be grabbed, in case the caught object is dropped
                    availableObjects.Add(collidedObj);
                }
            }

            //if the manipulator isn't currently grabbing, add collided object to the list of available objects to be grabbed
            if (!grabbingStatus)
            {
                //if the object the manipulator has collided with is the child of a larger assembly, add teh assembly to available objects
                if (collidedObj.orphanStatus)
                {
                    availableObjects.Add(collidedObj.parentObj);
                }

                //otherwise, just add the object to available objects
                else
                {
                    availableObjects.Add(collidedObj);
                }
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