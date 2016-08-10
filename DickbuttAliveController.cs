using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DickbuttAliveController : MonoBehaviour
{
    //movement related stuff
    public float radius;
    public Vector3 currentPosition;
    public Vector3 newPosition;

    //status booleans
    public bool aliveStatus;
    bool grabbedStatus;

    //all of this has to do with when dickbutt catches on fire
    public bool onFireStatus;
    public bool burningStatus;
    public ParticleSystem burning;
    public float burnLength;
    public float burnRate;
    public float minBurnLength;
    public List<InteractiveObjectController> objectsOnFire;

    //slot for which collider is grabbed. two for two manipulators
    InteractiveObjectController grabbed1;
    InteractiveObjectController grabbed2;
    int grabbedArray1;
    int grabbedArray2;

    //animation
    Animation anim;

    //colliders
    private Transform colliderHip;
    private Transform colliderTorso;
    private Transform colliderHead;
    private Transform colliderButt;
    private Transform colliderDick;
    private Transform colliderThighLeft;
    private Transform colliderKneeLeft;
    private Transform colliderFootLeft;
    private Transform colliderThighRight;
    private Transform colliderKneeRight;
    private Transform colliderFootRight;
    private Transform colliderShoulderLeft;
    private Transform colliderElbowLeft;
    private Transform colliderWristLeft;
    private Transform colliderShoulderRight;
    private Transform colliderElbowRight;
    private Transform colliderWristRight;

    //collider array
    private Transform[] colliders = new Transform[17];
    private Transform[] spineColliders = new Transform[2];
    private Transform[] buttColliders = new Transform[2];
    private Transform[] leftLegColliders = new Transform[3];
    private Transform[] rightLegColliders = new Transform[3];
    private Transform[] leftArmColliders = new Transform[3];
    private Transform[] rightArmColliders = new Transform[3];

    //joints
    //private Transform jointRoot;
    private Transform jointHip;
    private Transform jointSpine002;
    private Transform jointSpine004;
    private Transform jointTail002;
    private Transform jointDick001;
    private Transform jointThighLeft;
    private Transform jointKneeLeft;
    private Transform jointAnkleLeft;
    private Transform jointThighRight;
    private Transform jointKneeRight;
    private Transform jointAnkleRight;
    private Transform jointShoulderLeft;
    private Transform jointElbowLeft;
    private Transform jointWristLeft;
    private Transform jointShoulderRight;
    private Transform jointElbowRight;
    private Transform jointWristRight;

    //joints array
    private Transform[] joints = new Transform[17];
    private Transform[] spineJoints = new Transform[2];
    private Transform[] buttJoints = new Transform[2];
    private Transform[] leftLegJoints = new Transform[3];
    private Transform[] rightLegJoints = new Transform[3];
    private Transform[] leftArmJoints = new Transform[3];
    private Transform[] rightArmJoints = new Transform[3];

    //interactive objects
    private InteractiveObjectController hipObj;
    private InteractiveObjectController torsoObj;
    private InteractiveObjectController headObj;
    private InteractiveObjectController buttObj;
    private InteractiveObjectController dickObj;
    private InteractiveObjectController thighLeftObj;
    private InteractiveObjectController kneeLeftObj;
    private InteractiveObjectController footLeftObj;
    private InteractiveObjectController thighRightObj;
    private InteractiveObjectController kneeRightObj;
    private InteractiveObjectController footRightObj;
    private InteractiveObjectController shoulderLeftObj;
    private InteractiveObjectController elbowLeftObj;
    private InteractiveObjectController wristLeftObj;
    private InteractiveObjectController shoulderRightObj;
    private InteractiveObjectController elbowRightObj;
    private InteractiveObjectController wristRightObj;

    //interactive objects array
    private InteractiveObjectController[] objects = new InteractiveObjectController[17];
    private InteractiveObjectController[] spineObjs = new InteractiveObjectController[2];
    private InteractiveObjectController[] buttObjs = new InteractiveObjectController[2];
    private InteractiveObjectController[] leftLegObjs = new InteractiveObjectController[3];
    private InteractiveObjectController[] rightLegObjs = new InteractiveObjectController[3];
    private InteractiveObjectController[] leftArmObjs = new InteractiveObjectController[3];
    private InteractiveObjectController[] rightArmObjs = new InteractiveObjectController[3];

    //default positions array
    private Vector3[] defaults = new Vector3[17];

    // Use this for initialization
    void Start()
    {
        objectsOnFire = new List<InteractiveObjectController>();
        burning.gameObject.SetActive(false);
        anim = GetComponent<Animation>();

        //get colliders
        colliderHip = transform.FindChild("Collider_Hip");
        colliderTorso = transform.FindChild("Collider_Torso");
        colliderHead = transform.FindChild("Collider_Head");
        colliderButt = transform.FindChild("Collider_Butt");
        colliderDick = transform.FindChild("Collider_Dick");
        colliderThighLeft = transform.FindChild("Collider_Thigh_Left");
        colliderKneeLeft = transform.FindChild("Collider_Knee_Left");
        colliderFootLeft = transform.FindChild("Collider_Foot_Left");
        colliderThighRight = transform.FindChild("Collider_Thigh_Right");
        colliderKneeRight = transform.FindChild("Collider_Knee_Right");
        colliderFootRight = transform.FindChild("Collider_Foot_Right");
        colliderShoulderLeft = transform.FindChild("Collider_Shoulder_Left");
        colliderElbowLeft = transform.FindChild("Collider_Elbow_Left");
        colliderWristLeft = transform.FindChild("Collider_Wrist_Left");
        colliderShoulderRight = transform.FindChild("Collider_Shoulder_Right");
        colliderElbowRight = transform.FindChild("Collider_Elbow_Right");
        colliderWristRight = transform.FindChild("Collider_Wrist_Right");

        //setup collider arrays
        colliders[0] = colliderHip;
        colliders[1] = colliderTorso;
        colliders[2] = colliderHead;
        colliders[3] = colliderButt;
        colliders[4] = colliderDick;
        colliders[5] = colliderThighLeft;
        colliders[6] = colliderKneeLeft;
        colliders[7] = colliderFootLeft;
        colliders[8] = colliderThighRight;
        colliders[9] = colliderKneeRight;
        colliders[10] = colliderFootRight;
        colliders[11] = colliderShoulderLeft;
        colliders[12] = colliderElbowLeft;
        colliders[13] = colliderWristLeft;
        colliders[14] = colliderShoulderRight;
        colliders[15] = colliderElbowRight;
        colliders[16] = colliderWristRight;

        spineColliders[0] = colliderTorso;
        spineColliders[1] = colliderHead;

        buttColliders[0] = colliderButt;
        buttColliders[1] = colliderDick;

        leftLegColliders[0] = colliderThighLeft;
        leftLegColliders[1] = colliderKneeLeft;
        leftLegColliders[2] = colliderFootLeft;

        rightLegColliders[0] = colliderThighRight;
        rightLegColliders[1] = colliderKneeRight;
        rightLegColliders[2] = colliderFootRight;

        leftArmColliders[0] = colliderShoulderLeft;
        leftArmColliders[1] = colliderElbowLeft;
        leftArmColliders[2] = colliderWristLeft;

        rightArmColliders[0] = colliderShoulderRight;
        rightArmColliders[1] = colliderElbowRight;
        rightArmColliders[2] = colliderWristRight;

        //get joints
        //jointRoot = transform.FindChild("Joint_Root");
        jointHip = transform.FindChild("Joint_Root/Joint_Hip");
        jointSpine002 = transform.FindChild("Joint_Root/Joint_Hip/Joint_Spine_001/Joint_Spine_002");
        jointSpine004 = transform.FindChild("Joint_Root/Joint_Hip/Joint_Spine_001/Joint_Spine_002/Joint_Spine_003/Joint_Spine_004");
        jointTail002 = transform.FindChild("Joint_Root/Joint_Hip/Joint_Tail_001/Joint_Tail_002");
        jointDick001 = transform.FindChild("Joint_Root/Joint_Hip/Joint_Tail_001/Joint_Tail_002/Joint_Dick_001");
        jointThighLeft = transform.FindChild("Joint_Root/Joint_Hip/Joint_Thigh_Left");
        jointKneeLeft = transform.FindChild("Joint_Root/Joint_Hip/Joint_Thigh_Left/Joint_Knee_Left");
        jointAnkleLeft = transform.FindChild("Joint_Root/Joint_Hip/Joint_Thigh_Left/Joint_Knee_Left/Joint_Ankle_Left");
        jointThighRight = transform.FindChild("Joint_Root/Joint_Hip/Joint_Thigh_Right");
        jointKneeRight = transform.FindChild("Joint_Root/Joint_Hip/Joint_Thigh_Right/Joint_Knee_Right");
        jointAnkleRight = transform.FindChild("Joint_Root/Joint_Hip/Joint_Thigh_Right/Joint_Knee_Right/Joint_Ankle_Right");
        jointShoulderLeft = transform.FindChild("Joint_Root/Joint_Hip/Joint_Spine_001/Joint_Spine_002/Joint_Spine_003/Joint_Shoulder_Left");
        jointElbowLeft = transform.FindChild("Joint_Root/Joint_Hip/Joint_Spine_001/Joint_Spine_002/Joint_Spine_003/Joint_Shoulder_Left/Joint_Elbow_Left");
        jointWristLeft = transform.FindChild("Joint_Root/Joint_Hip/Joint_Spine_001/Joint_Spine_002/Joint_Spine_003/Joint_Shoulder_Left/Joint_Elbow_Left/Joint_Wrist_Left");
        jointShoulderRight = transform.FindChild("Joint_Root/Joint_Hip/Joint_Spine_001/Joint_Spine_002/Joint_Spine_003/Joint_Shoulder_Right");
        jointElbowRight = transform.FindChild("Joint_Root/Joint_Hip/Joint_Spine_001/Joint_Spine_002/Joint_Spine_003/Joint_Shoulder_Right/Joint_Elbow_Right");
        jointWristRight = transform.FindChild("Joint_Root/Joint_Hip/Joint_Spine_001/Joint_Spine_002/Joint_Spine_003/Joint_Shoulder_Right/Joint_Elbow_Right/Joint_Wrist_Right");

        //setup joints array
        //root joint is ignored
        joints[0] = jointHip;
        joints[1] = jointSpine002;
        joints[2] = jointSpine004;
        joints[3] = jointTail002;
        joints[4] = jointDick001;
        joints[5] = jointThighLeft;
        joints[6] = jointKneeLeft;
        joints[7] = jointAnkleLeft;
        joints[8] = jointThighRight;
        joints[9] = jointKneeRight;
        joints[10] = jointAnkleRight;
        joints[11] = jointShoulderLeft;
        joints[12] = jointElbowLeft;
        joints[13] = jointWristLeft;
        joints[14] = jointShoulderRight;
        joints[15] = jointElbowRight;
        joints[16] = jointWristRight;

        spineJoints[0] = jointSpine002;
        spineJoints[1] = jointSpine004;

        buttJoints[0] = jointTail002;
        buttJoints[1] = jointDick001;

        leftLegJoints[0] = jointThighLeft;
        leftLegJoints[1] = jointKneeLeft;
        leftLegJoints[2] = jointAnkleLeft;

        rightLegJoints[0] = jointThighRight;
        rightLegJoints[1] = jointKneeRight;
        rightLegJoints[2] = jointAnkleRight;

        leftArmJoints[0] = jointShoulderLeft;
        leftArmJoints[1] = jointElbowLeft;
        leftArmJoints[2] = jointWristLeft;

        rightArmJoints[0] = jointShoulderRight;
        rightArmJoints[1] = jointElbowRight;
        rightArmJoints[2] = jointWristRight;

        //get interactive objects
        hipObj = colliderHip.gameObject.GetComponent<InteractiveObjectController>();
        torsoObj = colliderTorso.gameObject.GetComponent<InteractiveObjectController>();
        headObj = colliderHead.gameObject.gameObject.GetComponent<InteractiveObjectController>();
        buttObj = colliderButt.gameObject.GetComponent<InteractiveObjectController>();
        dickObj = colliderDick.gameObject.GetComponent<InteractiveObjectController>();
        thighLeftObj = colliderThighLeft.gameObject.GetComponent<InteractiveObjectController>();
        kneeLeftObj = colliderKneeLeft.gameObject.GetComponent<InteractiveObjectController>();
        footLeftObj = colliderFootLeft.gameObject.GetComponent<InteractiveObjectController>();
        thighRightObj = colliderThighRight.gameObject.GetComponent<InteractiveObjectController>();
        kneeRightObj = colliderKneeRight.gameObject.GetComponent<InteractiveObjectController>();
        footRightObj = colliderFootRight.gameObject.GetComponent<InteractiveObjectController>();
        shoulderLeftObj = colliderShoulderLeft.gameObject.GetComponent<InteractiveObjectController>();
        elbowLeftObj = colliderElbowLeft.gameObject.GetComponent<InteractiveObjectController>();
        wristLeftObj = colliderWristLeft.gameObject.GetComponent<InteractiveObjectController>();
        shoulderRightObj = colliderShoulderRight.gameObject.GetComponent<InteractiveObjectController>();
        elbowRightObj = colliderElbowRight.gameObject.GetComponent<InteractiveObjectController>();
        wristRightObj = colliderWristRight.gameObject.GetComponent<InteractiveObjectController>();

        //setup interactive objects array
        objects[0] = hipObj;
        objects[1] = torsoObj;
        objects[2] = headObj;
        objects[3] = buttObj;
        objects[4] = dickObj;
        objects[5] = thighLeftObj;
        objects[6] = kneeLeftObj;
        objects[7] = footLeftObj;
        objects[8] = thighRightObj;
        objects[9] = kneeRightObj;
        objects[10] = footRightObj;
        objects[11] = shoulderLeftObj;
        objects[12] = elbowLeftObj;
        objects[13] = wristLeftObj;
        objects[14] = shoulderRightObj;
        objects[15] = elbowRightObj;
        objects[16] = wristRightObj;

        spineObjs[0] = torsoObj;
        spineObjs[1] = headObj;

        buttObjs[0] = buttObj;
        buttObjs[1] = dickObj;

        leftLegObjs[0] = thighLeftObj;
        leftLegObjs[1] = kneeLeftObj;
        leftLegObjs[2] = footLeftObj;

        rightLegObjs[0] = thighRightObj;
        rightLegObjs[1] = kneeRightObj;
        rightLegObjs[2] = footRightObj;

        leftArmObjs[0] = shoulderLeftObj;
        leftArmObjs[1] = elbowLeftObj;
        leftArmObjs[2] = wristLeftObj;

        rightArmObjs[0] = shoulderRightObj;
        rightArmObjs[1] = elbowRightObj;
        rightArmObjs[2] = wristRightObj;

        for (int i = 0; i < 17; i++)
        {
            defaults[i] = joints[i].localPosition;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (objectsOnFire.Count > 0)
        {
            onFireStatus = true;
        }

        if (!aliveStatus)
        {
            for (int i = 0; i < 17; i++)
            {
                joints[i].rotation = colliders[i].rotation;
                
                if (colliders[i].GetComponent<InteractiveObjectController>().effectStatus)
                {
                    objectsOnFire.Add(objects[i]);
                }
            }

            jointHip.position = colliderHip.position;
        }

        if (onFireStatus && !burning.gameObject.activeSelf)
        {
            Burn();
        }

        if (onFireStatus && burningStatus)
        {
            burnLength = burnLength - burnRate;

            if (burnLength < minBurnLength)
            {
                onFireStatus = false;
            }
        }

        if (!onFireStatus && burning.gameObject.activeSelf)
        {
            Extinguish();
        }

        
    }

    void Burn ()
    {
        burning.gameObject.SetActive(true);
        burningStatus = true;
    }

    void Extinguish()
    {
        for (int i = 0; i < objectsOnFire.Count; i++)
        {
            objectsOnFire[i].effectStatus = false;
        }

        objectsOnFire.Clear();
        burning.gameObject.SetActive(false);
        burnLength = 10;

        burningStatus = false;
    }
}