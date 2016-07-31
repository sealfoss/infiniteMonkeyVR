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

    //limits and conditions for guided movment
    public int moveAxisCondition;
    public int rotAxisCondition;
    public bool moveAxisX;
    public bool moveAxisY;
    public bool moveAxisZ;
    public bool rotAxisX;
    public bool rotAxisY;
    public bool rotAxisZ;
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

    public ManipulatorConroller attachedManipulator;
    public Transform offsetPoint;
    public PlugController plugObj;

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
        //if object is in guided mode, and conditions haven't already been set, decide the movement and rotationa condition for the guided move function, based on the object's attributes.
        if (guidedMove && (moveAxisCondition == 00 &&  rotAxisCondition == 00))
        {
            SetMoveCondition();
        }

        //reset plug values upon unplug
        {
            if (!plugObj && rotationAmount != 0)
            {
                rotationAmount = 0;
                revolutions = 0;
            }
        }
    }

    void SetMoveCondition ()
    {
        if (moveAxisX || moveAxisY || moveAxisZ)
        {
            moveAxisCondition = 1;
            bool[] moveConditions = new bool[] { moveAxisX, moveAxisY, moveAxisZ };

            foreach (bool axis in moveConditions)
            {
                if (axis == true)
                {
                    break;
                }

                else
                {
                    moveAxisCondition++;
                }
            }
        }

        if (rotAxisX || rotAxisY || rotAxisZ)
        {
            rotAxisCondition = 1;
            bool[] rotConditions = new bool[] { rotAxisX, rotAxisY, rotAxisZ };

            foreach (bool axis in rotConditions)
            {
                if (axis == true)
                {
                    break;
                }

                else
                {
                    rotAxisCondition++;
                }
            }
        }
    }
}
