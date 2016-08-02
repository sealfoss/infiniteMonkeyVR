using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InteractiveObjectController : MonoBehaviour
{

    //physical attributes related to object's movement when held
    public float velocityFactor = 2000f;
    public float rotationFactor = 80f;
    public float maxAV;

    //variables for modifying object's rigidBody
    public float rbMass;
    public float rbDrag;
    public float rbADrag;
    public Rigidbody rigidBody;

    //status booleans
    public bool jointedMove;
    public bool guidedMove;
    public bool lockedStatus;
    public bool grabbedStatus;
    public bool pluggedStatus;
    public bool actionStatus;
    public bool highlightedStatus;
    public bool orphanStatus;
    public bool animated;

    //limits and conditions for guided movment
    //which axis you want the object to move on when guided/plugged 1=x, 2=y, 3=z
    public int moveAxisCondition;
    //which axis you want the object to rotate on when guided/plugged 1=x, 2=y, 3=z
    public int rotAxisCondition;
    public float minPos;
    public float maxPos;
    public float minRot;
    public float maxRot;
    public float movementResistance;
    public float rotationAmount;
    public int revolutions;
    public int maxRevolutions;
    public bool stopRot;
    
    //max distance held object can be from manipulator in order to still be considered held
    public float maxDistance;

    //on collision, these variables have to do with how much you feel the impact
    float magnitude;
    public float vibrationMultiplier;

    //objects this object may need to interface with, depending on the cirumstances
    public ManipulatorConroller attachedManipulator;
    public Transform offsetPoint;
    public PlugController plugObj;
    public InteractiveObjectController parentObj;


    void Start()
    {
        if (GetComponent<Rigidbody>())
        {
            rbMass = GetComponent<Rigidbody>().mass;
            rbDrag = GetComponent<Rigidbody>().drag;
            rbADrag = GetComponent<Rigidbody>().angularDrag;
            rigidBody = GetComponent<Rigidbody>();
            velocityFactor /= rigidBody.mass;
            rotationFactor /= rigidBody.mass;
            maxAV = Mathf.Infinity;
            this.rigidBody.maxAngularVelocity = maxAV;
        }

        if (maxDistance == 0)
        {
            maxDistance = Mathf.Infinity;
        }
    }

    void FixedUpdate()
    {
        //reset plug values upon unplug
        {
            if (!plugObj && rotationAmount != 0)
            {
                rotationAmount = 0;
                revolutions = 0;
            }
        }

        if (parentObj)
        {
            //if the parent object is dropped while the plugged object is still attached to it, tell the plugged object that it is now an orphan, as its parent is dead
            if (!parentObj.attachedManipulator && !orphanStatus)
            {
                orphanStatus = true;
            }

            //if the parent object is picked back up, tell the object it has a family who loves him once more
            if(parentObj.attachedManipulator && orphanStatus)
            {
                orphanStatus = false;
            }
        }

        if (!parentObj && orphanStatus)
        {
            orphanStatus = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (attachedManipulator)
        {
            magnitude = collision.relativeVelocity.magnitude;
            ushort vibration = (ushort)(magnitude * vibrationMultiplier);
            attachedManipulator.Vibrate(vibration);
        }
    }
}
