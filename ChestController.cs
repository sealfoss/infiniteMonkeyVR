using UnityEngine;
using System.Collections;

public class ChestController : MonoBehaviour
{
    public InteractiveObjectController latch;
    public InteractiveObjectController lid;
    public bool latchDownStatus = false;
    private bool soundStatus = false;
    public AudioClip openSnd;
    AudioSource audioSrc;
    // Use this for initialization
    void Start()
    {
        //lockTrigger = transform.FindChild("LockTrigger").GetComponent<ChestTriggerController>();
        lid = transform.FindChild("Chest_Top_LOD0").GetComponent<InteractiveObjectController>();
        latch = transform.FindChild("Chest_Top_LOD0/Chest_Latch_LOD0").GetComponent<InteractiveObjectController>();
        audioSrc = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (latch)
        {
            if (latch.grabbedStatus == true && latch.attachedManipulator.actionStatus == true && soundStatus == false)
            {
                audioSrc.enabled = true;
                print("play Audio!");
                audioSrc.PlayOneShot(openSnd, 0.5f);
                soundStatus = true;
            }
        }
    }
}