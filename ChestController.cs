using UnityEngine;
using System.Collections;

public class ChestController : MonoBehaviour {
    public InteractiveObjectController latch;
    public InteractiveObjectController lid;
    public ChestTriggerController lockTrigger;
    public bool latchDownStatus = false;
    Quaternion latchDownRot;
    Quaternion latchUpRot;
    private bool soundStatus = false;
    public AudioClip openSnd;
    AudioSource audioSrc;
	// Use this for initialization
	void Start () {
        //lockTrigger = transform.FindChild("LockTrigger").GetComponent<ChestTriggerController>();
        lid = transform.FindChild("Chest_Top_LOD0").GetComponent<InteractiveObjectController>();
        latch = transform.FindChild("Chest_Top_LOD0/Chest_Latch_LOD0").GetComponent<InteractiveObjectController>();
        latchDownRot = Quaternion.Euler(new Vector3(92, 0, 0));
        latchUpRot = Quaternion.Euler(new Vector3(60, 0, 0));
        audioSrc = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
        if (latch)
        {
            if (latch.grabbedStatus == true && latch.actionStatus == true && soundStatus == false)
            {
                audioSrc.enabled = true;
                print("play Audio!");
                audioSrc.PlayOneShot(openSnd, 0.5f);
                soundStatus = true;
            }

            if (lid.transform.rotation.x > -1.0f)
            {
                if (latch.grabbedStatus == false && latch.transform.localRotation.x > 70.0f && latchDownStatus == false)
                {
                    lid.GetComponent<Rigidbody>().isKinematic = true;
                    lid.GetComponent<Rigidbody>().useGravity = false;
                    latch.GetComponent<Rigidbody>().isKinematic = true;
                    latch.GetComponent<Rigidbody>().useGravity = false;
                    latch.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, latchDownRot, Time.time * 0.5f);
                    latchDownStatus = true;
                }

                if (latch.grabbedStatus == true && latchDownStatus == true && lockTrigger.triggered == false)
                {
                    lid.GetComponent<Rigidbody>().isKinematic = false;
                    lid.GetComponent<Rigidbody>().useGravity = true;
                    latch.GetComponent<Rigidbody>().isKinematic = false;
                    latch.GetComponent<Rigidbody>().useGravity = true;
                    latch.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, latchUpRot, Time.time * 0.5f);
                    latchDownStatus = false;
                }
            }
        }
	}
}
