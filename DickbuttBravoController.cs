using UnityEngine;
using System.Collections;

public class DickbuttBravoController : MonoBehaviour
{
    //colliders/grabbable interactive objects
    private Transform colliderHead;
    private Transform colliderTorso;
    private Transform colliderHip;
    private Transform colliderThighLeft;
    private Transform colliderKneeLeft;
    private Transform colliderFootLeft;
    private Transform colliderThighRight;
    private Transform colliderKneeRight;
    private Transform colliderFootRight;
    private Transform colliderButt;
    private Transform colliderDick;
    private Transform colliderShoulderLeft;
    private Transform colliderElbowLeft;
    private Transform colliderWristLeft;
    private Transform colliderShoulderRight;
    private Transform colliderElbowRight;
    private Transform colliderWristRight;

    //joints
    private Transform jointSpine004;
    private Transform jointSpine002;
    private Transform jointHip;
    private Transform jointThighLeft;
    private Transform jointKneeLeft;
    private Transform jointAnkleLeft;
    private Transform jointThighRight;
    private Transform jointKneeRight;
    private Transform jointAnkleRight;
    private Transform jointTail002;
    private Transform jointDick001;
    private Transform jointShoulderLeft;
    private Transform jointElbowLeft;
    private Transform jointWristLeft;
    private Transform jointShoulderRight;
    private Transform jointElbowRight;
    private Transform jointWristRight;

    //offsets
    private Quaternion jointSpine004Offset;
    private Quaternion jointSpine002Offset;
    private Quaternion jointHipOffset;
    private Quaternion jointThighLeftOffset;
    private Quaternion jointKneeLeftOffset;
    private Quaternion jointAnkleLeftOffset;
    private Quaternion jointThighRightOffset;
    private Quaternion jointKneeRightOffset;
    private Quaternion jointAnkleRightOffset;
    private Quaternion jointTail002Offset;
    private Quaternion jointDick001Offset;
    private Quaternion jointShoulderLeftOffset;
    private Quaternion jointElbowLeftOffset;
    private Quaternion jointWristLeftOffset;
    private Quaternion jointShoulderRightOffset;
    private Quaternion jointElbowRightOffset;
    private Quaternion jointWristRightOffset;
    // Use this for initialization
    void Start()
    {
        //get the colliders
        colliderHip = transform.FindChild("Collider_Hip");
        colliderHead = transform.FindChild("Collider_Head");
        colliderTorso = transform.FindChild("Collider_Torso");
        colliderThighLeft = transform.FindChild("Collider_Thigh_Left");
        colliderKneeLeft = transform.FindChild("Collider_Knee_Left");
        colliderFootLeft = transform.FindChild("Collider_Foot_Left");
        colliderThighRight = transform.FindChild("Collider_Thigh_Right");
        colliderKneeRight = transform.FindChild("Collider_Knee_Right");
        colliderFootRight = transform.FindChild("Collider_Foot_Right");
        colliderButt = transform.FindChild("Collider_Butt");
        colliderDick = transform.FindChild("Collider_Dick");
        colliderShoulderLeft = transform.FindChild("Collider_Shoulder_Left");
        colliderElbowLeft = transform.FindChild("Collider_Elbow_Left");
        colliderWristLeft = transform.FindChild("Collider_Wrist_Left");
        colliderShoulderRight = transform.FindChild("Collider_Shoulder_Right");
        colliderElbowRight = transform.FindChild("Collider_Elbow_Right");
        colliderWristRight = transform.FindChild("Collider_Wrist_Right");

        //get the joints
        jointHip = transform.FindChild("Joint_Root/Joint_Hip");
        jointSpine004 = transform.FindChild("Joint_Root/Joint_Hip/Joint_Spine_001/Joint_Spine_002/Joint_Spine_003/Joint_Spine_004");
        jointSpine002 = transform.FindChild("Joint_Root/Joint_Hip/Joint_Spine_001/Joint_Spine_002");
        jointThighLeft = transform.FindChild("Joint_Root/Joint_Hip/Joint_Thigh_Left");
        jointKneeLeft = transform.FindChild("Joint_Root/Joint_Hip/Joint_Thigh_Left/Joint_Knee_Left");
        jointAnkleLeft = transform.FindChild("Joint_Root/Joint_Hip/Joint_Thigh_Left/Joint_Knee_Left/Joint_Ankle_Left");
        jointThighRight = transform.FindChild("Joint_Root/Joint_Hip/Joint_Thigh_Right");
        jointKneeRight = transform.FindChild("Joint_Root/Joint_Hip/Joint_Thigh_Right/Joint_Knee_Right");
        jointAnkleRight = transform.FindChild("Joint_Root/Joint_Hip/Joint_Thigh_Right/Joint_Knee_Right/Joint_Ankle_Right");
        jointTail002 = transform.FindChild("Joint_Root/Joint_Hip/Joint_Tail_001/Joint_Tail_002");
        jointDick001 = transform.FindChild("Joint_Root/Joint_Hip/Joint_Tail_001/Joint_Tail_002/Joint_Dick_001");
        jointShoulderLeft = transform.FindChild("Joint_Root/Joint_Hip/Joint_Spine_001/Joint_Spine_002/Joint_Spine_003/Joint_Shoulder_Left");
        jointElbowLeft = transform.FindChild("Joint_Root/Joint_Hip/Joint_Spine_001/Joint_Spine_002/Joint_Spine_003/Joint_Shoulder_Left/Joint_Elbow_Left");
        jointWristLeft = transform.FindChild("Joint_Root/Joint_Hip/Joint_Spine_001/Joint_Spine_002/Joint_Spine_003/Joint_Shoulder_Left/Joint_Elbow_Left/Joint_Wrist_Left");
        jointShoulderRight = transform.FindChild("Joint_Root/Joint_Hip/Joint_Spine_001/Joint_Spine_002/Joint_Spine_003/Joint_Shoulder_Right");
        jointElbowRight = transform.FindChild("Joint_Root/Joint_Hip/Joint_Spine_001/Joint_Spine_002/Joint_Spine_003/Joint_Shoulder_Right/Joint_Elbow_Right");
        jointWristRight = transform.FindChild("Joint_Root/Joint_Hip/Joint_Spine_001/Joint_Spine_002/Joint_Spine_003/Joint_Shoulder_Right/Joint_Elbow_Right/Joint_Wrist_Right");

        //get the joint offsets
        //might be wrong
        /**
        jointHipOffset = new Vector3(jointHip.localEulerAngles.x, jointHip.localEulerAngles.y, jointHip.localEulerAngles.z);
        jointSpine004Offset = new Vector3(jointSpine004.localEulerAngles.x, jointSpine004.localEulerAngles.y, jointSpine004.localEulerAngles.z);
        jointSpine002Offset = new Vector3(jointSpine002.localEulerAngles.x, jointSpine002.localEulerAngles.y, jointSpine002.localEulerAngles.z);
        jointThighLeftOffset = new Vector3(jointThighLeft.localEulerAngles.x, jointThighLeft.localEulerAngles.y, jointThighLeft.localEulerAngles.z);
        jointKneeLeftOffset = new Vector3(jointKneeLeft.localEulerAngles.x, jointKneeLeft.localEulerAngles.y, jointKneeLeft.localEulerAngles.z);
        jointAnkleLeftOffset = new Vector3(jointAnkleLeft.localEulerAngles.x, jointAnkleLeft.localEulerAngles.y, jointAnkleLeft.localEulerAngles.z);
        jointThighRightOffset = new Vector3(jointThighRight.localEulerAngles.x, jointThighRight.localEulerAngles.y, jointThighRight.localEulerAngles.z);
        jointKneeRightOffset = new Vector3(jointKneeRight.localEulerAngles.x, jointKneeRight.localEulerAngles.y, jointKneeRight.localEulerAngles.z);
        jointAnkleRightOffset = new Vector3(jointAnkleRight.localEulerAngles.x, jointAnkleRight.localEulerAngles.y, jointAnkleRight.localEulerAngles.z);
        jointTail002Offset = new Vector3(jointTail002.localEulerAngles.x, jointTail002.localEulerAngles.y, jointTail002.localEulerAngles.z);
        jointDick001Offset = new Vector3(jointDick001.localEulerAngles.x, jointDick001.localEulerAngles.y, jointDick001.localEulerAngles.z);
        jointShoulderLeftOffset = new Vector3(jointShoulderLeft.localEulerAngles.x, jointShoulderLeft.localEulerAngles.y, jointShoulderLeft.localEulerAngles.z);
        jointElbowLeftOffset = new Vector3(jointElbowLeft.localEulerAngles.x, jointElbowLeft.localEulerAngles.y, jointElbowLeft.localEulerAngles.z);
        jointWristLeftOffset = new Vector3(jointWristLeft.localEulerAngles.x, jointWristLeft.localEulerAngles.y, jointWristLeft.localEulerAngles.z);
        jointShoulderRightOffset = new Vector3(jointShoulderRight.localEulerAngles.x, jointShoulderRight.localEulerAngles.y, jointShoulderRight.localEulerAngles.z);
        jointElbowRightOffset = new Vector3(jointElbowRight.localEulerAngles.x, jointElbowRight.localEulerAngles.y, jointElbowRight.localEulerAngles.z);
        jointWristRightOffset = new Vector3(jointWristRight.localEulerAngles.x, jointWristRight.localEulerAngles.y, jointWristRight.localEulerAngles.z);**/

        jointHipOffset = Quaternion.Euler(jointHip.localEulerAngles.x, jointHip.localEulerAngles.y, jointHip.localEulerAngles.z);
        jointSpine004Offset = Quaternion.Euler(jointSpine004.localEulerAngles.x, jointSpine004.localEulerAngles.y, jointSpine004.localEulerAngles.z);
        jointSpine002Offset = Quaternion.Euler(jointSpine002.localEulerAngles.x, jointSpine002.localEulerAngles.y, jointSpine002.localEulerAngles.z);
        jointThighLeftOffset = Quaternion.Euler(jointThighLeft.localEulerAngles.x, jointThighLeft.localEulerAngles.y, jointThighLeft.localEulerAngles.z);
        jointKneeLeftOffset = Quaternion.Euler(jointKneeLeft.localEulerAngles.x, jointKneeLeft.localEulerAngles.y, jointKneeLeft.localEulerAngles.z);
        jointAnkleLeftOffset = Quaternion.Euler(jointAnkleLeft.localEulerAngles.x, jointAnkleLeft.localEulerAngles.y, jointAnkleLeft.localEulerAngles.z);
        jointThighRightOffset = Quaternion.Euler(jointThighRight.localEulerAngles.x, jointThighRight.localEulerAngles.y, jointThighRight.localEulerAngles.z);
        jointKneeRightOffset = Quaternion.Euler(jointKneeRight.localEulerAngles.x, jointKneeRight.localEulerAngles.y, jointKneeRight.localEulerAngles.z);
        jointAnkleRightOffset = Quaternion.Euler(jointAnkleRight.localEulerAngles.x, jointAnkleRight.localEulerAngles.y, jointAnkleRight.localEulerAngles.z);
        jointTail002Offset = Quaternion.Euler(jointTail002.localEulerAngles.x, jointTail002.localEulerAngles.y, jointTail002.localEulerAngles.z);
        jointDick001Offset = Quaternion.Euler(jointDick001.localEulerAngles.x, jointDick001.localEulerAngles.y, jointDick001.localEulerAngles.z);
        jointShoulderLeftOffset = Quaternion.Euler(jointShoulderLeft.localEulerAngles.x, jointShoulderLeft.localEulerAngles.y, jointShoulderLeft.localEulerAngles.z);
        jointElbowLeftOffset = Quaternion.Euler(jointElbowLeft.localEulerAngles.x, jointElbowLeft.localEulerAngles.y, jointElbowLeft.localEulerAngles.z);
        jointWristLeftOffset = Quaternion.Euler(jointWristLeft.localEulerAngles.x, jointWristLeft.localEulerAngles.y, jointWristLeft.localEulerAngles.z);
        jointShoulderRightOffset = Quaternion.Euler(jointShoulderRight.localEulerAngles.x, jointShoulderRight.localEulerAngles.y, jointShoulderRight.localEulerAngles.z);
        jointElbowRightOffset = Quaternion.Euler(jointElbowRight.localEulerAngles.x, jointElbowRight.localEulerAngles.y, jointElbowRight.localEulerAngles.z);
        jointWristRightOffset = Quaternion.Euler(jointWristRight.localEulerAngles.x, jointWristRight.localEulerAngles.y, jointWristRight.localEulerAngles.z);
    }

    // Update is called once per frame
    void Update()
    {
        jointHip.transform.rotation = colliderHip.transform.rotation * jointHipOffset;
        jointSpine004.transform.rotation = colliderHead.transform.rotation * jointSpine004Offset;
        jointSpine002.transform.rotation = colliderTorso.transform.rotation * jointSpine002Offset;
        jointThighLeft.transform.rotation = colliderThighLeft.transform.rotation * jointThighLeftOffset;
        jointKneeLeft.transform.rotation = colliderKneeLeft.transform.rotation * jointKneeLeftOffset;
        jointAnkleLeft.transform.rotation = colliderFootLeft.transform.rotation * jointAnkleLeftOffset;
        jointThighRight.transform.rotation = colliderThighRight.transform.rotation * jointThighRightOffset;
        jointKneeRight.transform.rotation = colliderKneeRight.transform.rotation * jointKneeRightOffset;
        jointAnkleRight.transform.rotation = colliderFootRight.transform.rotation * jointAnkleRightOffset;
        jointTail002.transform.rotation = colliderButt.transform.rotation * jointTail002Offset;
        jointDick001.transform.rotation = colliderDick.transform.rotation * jointDick001Offset;
        jointShoulderLeft.transform.rotation = colliderShoulderLeft.transform.rotation * jointShoulderLeftOffset;
        jointElbowLeft.transform.rotation = colliderElbowLeft.transform.rotation * jointElbowLeftOffset;
        jointWristLeft.transform.rotation = colliderWristLeft.transform.rotation * jointWristLeftOffset;
        jointShoulderRight.transform.rotation = colliderShoulderRight.transform.rotation * jointShoulderRightOffset;
        jointElbowRight.transform.rotation = colliderElbowRight.transform.rotation * jointElbowRightOffset;
        jointWristRight.transform.rotation = colliderWristRight.transform.rotation * jointWristRightOffset;
    }
}
