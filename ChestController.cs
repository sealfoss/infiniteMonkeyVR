using UnityEngine;
using System.Collections;

public class ChestController : MonoBehaviour
{
    public InteractiveObjectController latch;
    public InteractiveObjectController lid;
    public bool latchDownStatus = false;
    Quaternion latchDownRot;
    Quaternion latchUpRot;
    private bool soundStatus = false;
    public AudioClip openSnd;
    AudioSource audioSrc;
    // Use this for initialization
    void Start()
    {
        //lockTrigger = transform.FindChild("LockTrigger").GetComponent<ChestTriggerController>();
        lid = transform.FindChild("Chest_Top_LOD0").GetComponent<InteractiveObjectController>();
        latch = transform.FindChild("Chest_Top_LOD0/Chest_Latch_LOD0").GetComponent<InteractiveObjectController>();
        latchDownRot = Quaternion.Euler(new Vector3(92, 0, 0));
        latchUpRot = Quaternion.Euler(new Vector3(60, 0, 0));
        audioSrc = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (latch)
        {
            if (latch.grabbedStatus == true && latch.actionStatus == true && soundStatus == false)
            {
                audioSrc.enabled = true;
                print("play Audio!");
                audioSrc.PlayOneShot(openSnd, 0.5f);
                soundStatus = true;
            }
        }
    }
}