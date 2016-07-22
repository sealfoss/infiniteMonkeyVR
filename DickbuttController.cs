using UnityEngine;
using System.Collections;

public class DickbuttController : MonoBehaviour {

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

    // Use this for initialization
    void Start () {
        colliderHead = transform.FindChild("Collider_Head");
        colliderTorso = transform.FindChild("Collider_Torso");
        colliderHip = transform.FindChild("Collider_Hip");
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

        jointSpine004 = transform.FindChild("Collider_Hip/Joint_Root/Joint_Hip/Joint_Spine_001/Joint_Spine_002/Joint_Spine_003/Joint_Spine_004");
        jointSpine002 = transform.FindChild("Collider_Hip/Joint_Root/Joint_Hip/Joint_Spine_001/Joint_Spine_002");
        jointHip = transform.FindChild("Collider_Hip/Joint_Root/Joint_Hip");
        jointThighLeft = transform.FindChild("Collider_Hip/Joint_Root/Joint_Hip/Joint_Thigh_Left");
        jointKneeLeft = transform.FindChild("Collider_Hip/Joint_Root/Joint_Hip/Joint_Thigh_Left/Joint_Knee_Left");
        jointAnkleLeft = transform.FindChild("Collider_Hip/Joint_Root/Joint_Hip/Joint_Thigh_Left/Joint_Knee_Left/Joint_Ankle_Left");
        jointThighRight = transform.FindChild("Collider_Hip/Joint_Root/Joint_Hip/Joint_Thigh_Right");
        jointKneeRight = transform.FindChild("Collider_Hip/Joint_Root/Joint_Hip/Joint_Thigh_Right/Joint_Knee_Right");
        jointAnkleRight = transform.FindChild("Collider_Hip/Joint_Root/Joint_Hip/Joint_Thigh_Right/Joint_Knee_Right/Joint_Ankle_Right");
        jointTail002 = transform.FindChild("Collider_Hip/Joint_Root/Joint_Hip/Joint_Tail_001/Joint_Tail_002");
        jointDick001 = transform.FindChild("Collider_Hip/Joint_Root/Joint_Hip/Joint_Tail_001/Joint_Tail_002/Joint_Dick_001");
        jointShoulderLeft = transform.FindChild("Collider_Hip/Joint_Root/Joint_Hip/Joint_Spine_001/Joint_Spine_002/Joint_Spine_003/Joint_Shoulder_Left");
        jointElbowLeft = transform.FindChild("Collider_Hip/Joint_Root/Joint_Hip/Joint_Spine_001/Joint_Spine_002/Joint_Spine_003/Joint_Shoulder_Left/Joint_Elbow_Left");
        jointWristLeft = transform.FindChild("Collider_Hip/Joint_Root/Joint_Hip/Joint_Spine_001/Joint_Spine_002/Joint_Spine_003/Joint_Shoulder_Left/Joint_Elbow_Left/Joint_Wrist_Left");
        jointShoulderRight = transform.FindChild("Collider_Hip/Joint_Root/Joint_Hip/Joint_Spine_001/Joint_Spine_002/Joint_Spine_003/Joint_Shoulder_Right");
        jointElbowRight = transform.FindChild("Collider_Hip/Joint_Root/Joint_Hip/Joint_Spine_001/Joint_Spine_002/Joint_Spine_003/Joint_Shoulder_Right/Joint_Elbow_Right");
        jointWristRight = transform.FindChild("Collider_Hip/Joint_Root/Joint_Hip/Joint_Spine_001/Joint_Spine_002/Joint_Spine_003/Joint_Shoulder_Right/Joint_Elbow_Right/Joint_Wrist_Right");
    }
	
	// Update is called once per frame
	void Update () {
        jointSpine004.transform.rotation = colliderHead.transform.rotation;
        jointSpine002.transform.rotation = colliderTorso.transform.rotation;
        jointHip.transform.rotation = colliderHip.transform.rotation;
        jointThighLeft.transform.rotation = colliderThighLeft.transform.rotation;
        jointKneeLeft.transform.rotation = colliderKneeLeft.transform.rotation;
        jointAnkleLeft.transform.rotation = colliderFootLeft.transform.rotation;
        jointThighRight.transform.rotation = colliderThighRight.transform.rotation;
        jointKneeRight.transform.rotation = colliderKneeRight.transform.rotation;
        jointAnkleRight.transform.rotation = colliderFootRight.transform.rotation;
        jointTail002.transform.rotation = colliderButt.transform.rotation;
        jointDick001.transform.rotation = colliderDick.transform.rotation;
        jointShoulderLeft.transform.rotation = colliderShoulderLeft.transform.rotation;
        jointElbowLeft.transform.rotation = colliderElbowLeft.transform.rotation;
        jointWristLeft.transform.rotation = colliderWristLeft.transform.rotation;
        jointShoulderRight.transform.rotation = colliderShoulderRight.transform.rotation;
        jointElbowRight.transform.rotation = colliderElbowRight.transform.rotation;
        jointWristRight.transform.rotation = colliderWristRight.transform.rotation;
    }
}
