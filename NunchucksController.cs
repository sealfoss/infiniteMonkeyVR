using UnityEngine;
using System.Collections;

public class NunchucksController : MonoBehaviour {
    private Transform colliderRopeLeft;
    private Transform colliderRopeRight;
    private Transform colliderHandleLeft;
    private Transform colliderHandleRight;
    private Transform colliderRope;

    private Transform jointRoot;
    private Transform jointRopeLeft001;
    private Transform jointRopeRight001;
    private Transform jointRopeLeft002;
    private Transform jointRopeRight002;
    // Use this for initialization
    void Start () {
        colliderRope = transform.FindChild("Collider_Rope");
        colliderRopeLeft = transform.FindChild("Collider_Rope_Left");
        colliderRopeRight = transform.FindChild("Collider_Rope_Right");
        colliderHandleLeft = transform.FindChild("Collider_Handle_Left");
        colliderHandleRight = transform.FindChild("Collider_Handle_Right");

        jointRoot = transform.FindChild("Joint_Root");

        jointRopeLeft001 = transform.FindChild("Joint_Root/Joint_Rope_Left_001");
        jointRopeLeft002 = transform.FindChild("Joint_Root/Joint_Rope_Left_001/Joint_Rope_Left_002");

        jointRopeRight001 = transform.FindChild("Joint_Root/Joint_Rope_Right_001");
        jointRopeRight002 = transform.FindChild("Joint_Root/Joint_Rope_Right_001/Joint_Rope_Right_002");
        //jointRoot = transform.FindChild("Joint_Root");
    }
	
	// Update is called once per frame
	void Update () {
        //print("handleR x velocity = " + colliderHandleRight.GetComponent<Rigidbody>().velocity.x + ", handleR y velocity = " + colliderHandleRight.GetComponent<Rigidbody>().velocity.z + ", handleR y velocity = " + colliderHandleRight.GetComponent<Rigidbody>().velocity.z);
        jointRoot.transform.rotation = colliderRope.transform.rotation;
        jointRoot.transform.position = colliderRope.transform.position;

        jointRopeLeft001.transform.rotation = colliderRopeLeft.transform.rotation;
        jointRopeLeft002.transform.rotation = colliderHandleLeft.transform.rotation;

        jointRopeRight001.transform.rotation = colliderRopeRight.transform.rotation;
        jointRopeRight002.transform.rotation = colliderHandleRight.transform.rotation;
	}
}
