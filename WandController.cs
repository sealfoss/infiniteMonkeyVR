using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WandController : MonoBehaviour {
    ManipulatorController manipulator;
    InteractiveObjectController objectController;

    bool fireMode;
    bool sparkleStatus;

    public AudioClip fireSpray;
    public AudioClip fireWoosh;
    public AudioClip sparkleTwinkle;
    public AudioClip flameBurn;
    public AudioSource jetAudioSrc;
    public AudioSource flameAudioSrc;
    public AudioSource sparkleAudioSrc;
    public AudioSource fireWooshAudioSrc;

    public ParticleSystem sparks;
    ParticleSystem.EmissionModule sparksEM;
    public Light sparksLight;

    public ParticleSystem fire;
    ParticleSystem.EmissionModule fireEM;

    public ParticleSystem smoke;
    ParticleSystem.EmissionModule smokeEM;

    public ParticleSystem flame;
    ParticleSystem.EmissionModule flameEM;

    public ParticleSystem jet;
    ParticleSystem.EmissionModule jetEM;
    public Light fireLight;

    bool jetStatus;
    public float fuel;
    public float fuelRate;
    public float minFuel;
    public Transform head;
    public Transform wandTip;
    public Transform lastTracker;
    public Transform refObject;
    bool referenceStatus;
    Vector3 direction;
    public float minDist;
    public float directionTolerance;
    public List<int> spellSequence;
    public int[] fireCode = new int[3];

    // Use this for initialization
    void Start ()
    {
        fireWooshAudioSrc.enabled = true;
        sparkleAudioSrc.enabled = true;
        flameAudioSrc.enabled = true;
        jetAudioSrc.enabled = true;
        objectController = GetComponent<InteractiveObjectController>();

        spellSequence = new List<int>();
        //spellSequence[0] = 0;

        lastTracker = new GameObject().transform;
        lastTracker.parent = head;

        sparksEM = sparks.emission;
        sparksEM.enabled = false;
        sparksLight.enabled = false;

        fireEM = fire.emission;
        fireEM.enabled = false;

        flameEM = flame.emission;
        flameEM.enabled = false;

        jetEM = jet.emission;
        jetEM.enabled = false;
        fireLight.enabled = false;

        smokeEM = smoke.emission;
        smokeEM.enabled = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (objectController.grabbedStatus)
        {
            if (fireMode)
            {
                if (!manipulator)
                {
                    manipulator = objectController.attachedManipulator;
                }

                if (manipulator)
                {
                    if (manipulator.actionStatus)
                    {
                        if (!jetStatus)
                        {
                            ShootJet();
                        }
                    }

                    if (!manipulator.actionStatus)
                    {
                        if(jetStatus)
                        {
                            EndJet();
                        }
                    }
                }
            }

            if (!fireMode)
            {
                if (!manipulator)
                {
                    manipulator = objectController.attachedManipulator;
                }

                if (manipulator)
                {
                    if (manipulator.actionStatus)
                    {
                        Track();

                        if (!sparkleStatus)
                        {
                            Sparkle();
                        }
                    }

                    if (!manipulator.actionStatus)
                    {
                        if (referenceStatus)
                        {
                            ResetTracking();
                        }

                        if (sparkleStatus)
                        {
                            EndSparkle();
                        }
                    }
                }
            }
        }

        if (!objectController.grabbedStatus)
        {
            if (fireMode)
            {
                Extinguish();
            }

            if (manipulator)
            {
                manipulator = null;
            }

            if (sparkleStatus)
            {
                EndSparkle();
                ResetTracking();
            }
        }
	}

    void ShootJet()
    {
        if (!jetAudioSrc.isPlaying)
        {
            jetAudioSrc.PlayOneShot(fireSpray, fuel);
        }

        var jetEmitRate = jetEM.rate.constantMax;
        var flameEmitRate = flameEM.rate.constantMax;

        fuel = fuel - fuelRate;

        jet.startSpeed = 10 * fuel;
        jet.startSize = fuel;
        jetEmitRate = 75 * fuel;

        flame.startSpeed = 0.1f * fuel; // divided by 10?
        flame.startSize = 0.15f * fuel;
        flameEmitRate = 50 * fuel; 

        fuel = fuel - fuelRate;

        if (!jetStatus)
        {
            fireLight.range = 2;
            jetEM.enabled = true;
            jetStatus = true;
        }

        if (fuel < minFuel)
        {
            Extinguish();
        }
    }

    void EndJet()
    {
        jetAudioSrc.Stop();
        fireLight.range = 1;
        jetEM.enabled = false;
        jetStatus = false;
    }

    void Sparkle()
    {
        fire.gameObject.SetActive(false);
        sparksEM.enabled = true;
        sparksLight.enabled = true;
        smokeEM.enabled = true;
        sparkleStatus = true;

        if (!sparkleAudioSrc.isPlaying)
        {
            sparkleAudioSrc.Play();
        }
    }

    void EndSparkle()
    {
        sparkleAudioSrc.Stop();
        smokeEM.enabled = false;
        sparksEM.enabled = false;
        sparksLight.enabled = false;
        sparkleStatus = false;
    }

    void Extinguish()
    {
        EndJet();
        flameEM.enabled = false;
        fire.gameObject.SetActive(false);
        fireEM.enabled = false;
        fireMode = false;
        fireLight.enabled = false;
        flameAudioSrc.Stop();
        fireWooshAudioSrc.Stop();
    }

    void ActivateFire()
    {
        fuel = 1;
        fire.gameObject.SetActive(true);
        fireEM.enabled = true;
        manipulator.actionStatus = false;
        flameEM.enabled = true;
        flame.startSize = 0.15f;
        fireMode = true;
        fireLight.range = 1;
        fireLight.enabled = true;
        fireWooshAudioSrc.PlayOneShot(fireWoosh, 1);
        flameAudioSrc.Play();
    }

    void ResetTracking()
    {
        Destroy(refObject.gameObject);
        Destroy(lastTracker.gameObject);
        referenceStatus = false;
        spellSequence.Clear();
        spellSequence.Add(0);
    }

    void BuildRefObjects()
    {
        refObject = new GameObject().transform;
        refObject.position = head.position;
        refObject.rotation = head.rotation;
        refObject.eulerAngles = new Vector3(0, refObject.eulerAngles.y, 0);
        lastTracker = new GameObject().transform;
        lastTracker.parent = refObject;
        referenceStatus = true;
    }

    Vector3 GetTrackerDirection(Transform tracker)
    {
        var heading = tracker.transform.localPosition - lastTracker.transform.localPosition;
        var distance = heading.magnitude;
        var dir = heading / distance;
        return dir;
    }

    void Track()
    {
        //dirNum is basically an integer designating the direciton of 
        int dirNum = 0;
        Transform tracker = new GameObject().transform;
        tracker.position = wandTip.position;

        if (!referenceStatus || lastTracker == null)
        {
            BuildRefObjects();
            lastTracker.position = tracker.position;
        }

        refObject.eulerAngles = new Vector3(0, head.eulerAngles.y, 0);
        tracker.parent = refObject;

        if (lastTracker != null)
        {
            float dist = Vector3.Distance(lastTracker.position, tracker.position);

            if (dist > minDist)
            {
                direction = GetTrackerDirection(tracker);
                lastTracker.position = tracker.position;
            }

            var dirY = direction.x;
            var dirX = direction.y;

            if (Mathf.Abs(dirX) < directionTolerance)
            {
                dirX = 0;
            }

            if (Mathf.Abs(dirY) < directionTolerance)
            {
                dirY = 0;
            }

            if (dirX > 0)
            {
                if (dirY == 0)
                {
                    //print("up");
                    dirNum = 8;
                }

                if (dirY > 0)
                {
                    //print("up right");
                    dirNum = 1;
                }

                if (dirY < 0)
                {
                    //print("up left");
                    dirNum = 7;
                }
            }

            if (dirX < 0)
            {
                if (dirY == 0)
                {
                    //print("down");
                    dirNum = 4;
                }

                if (dirY > 0)
                {
                    //print("down right");
                    dirNum = 3;
                }

                if (dirY < 0)
                {
                    //print("down left");
                    dirNum = 5;
                }
            }

            if (dirX == 0)
            {
                if (dirY == 0)
                {
                    //print("nowhere");
                    dirNum = 0;
                }

                if (dirY > 0)
                {
                    //print("right");
                    dirNum = 2;
                }

                if (dirY < 0)
                {
                    //print("left");
                    dirNum = 6;
                }
            }

            int spellCount = spellSequence.Count;

            if (spellSequence.Count > 2)
            {
                if (dirNum != spellSequence[spellCount - 1])
                {
                    spellSequence.Add(dirNum);

                    if (spellCount + 1 > 2)
                    {
                        DetectPattern();
                    }
                }
            }

            if (spellCount < 3)
            {
                spellSequence.Add(dirNum);
            }
        }

        Destroy(tracker.gameObject);
    }

    void DetectPattern()
    {
        int spellCount = spellSequence.Count;

        for (int i = 0; i < spellCount; i++)
        {
            if (spellSequence[i] == fireCode[0])
            {
                if ((i + 1 < spellCount) && (spellSequence[i + 1] == fireCode[1]))
                {
                    if ((i + 2 < spellCount) && (spellSequence[i+2] == fireCode[2] || spellSequence[i + 2] == 8 || spellSequence[i + 2] == 2))
                    {
                        ActivateFire();
                    }
                }
            }
        }
    }
}
