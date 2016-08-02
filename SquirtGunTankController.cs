using UnityEngine;
using System.Collections;

public class SquirtGunTankController : MonoBehaviour {
    public InteractiveObjectController objectController;
    public SquirtGunTankMouthController mouthController;

    //particle effects
    public ParticleSystem pour;
    ParticleSystem.EmissionModule pourEM;
    public ParticleSystem pourSplash;
    ParticleSystem.EmissionModule pourSplashEM;

    public float waterLevel;
    public float lastWaterLevel;
    public float vDot;
    public float tilt;
    public float minTilt;
    public float leakRate;
    public float pourThreshold;
    public bool sealStatus;
    public bool leakStatus;
    public bool pourStatus;
    public ushort leakVibration;

    // Use this for initialization
	void Start ()
    {
        minTilt = 0.1f;
        pourEM = pour.emission;
        pourEM.enabled = false;
        pourSplashEM = pourSplash.emission;
        pourSplashEM.enabled = false;
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (!sealStatus && waterLevel > 0)
        {
            Leak();
        }

        if (sealStatus || waterLevel == 0)
        {
            leakStatus = false;
        }

        if (objectController.lockedStatus && !sealStatus)
        {
            sealStatus = true;
        }

        if (!objectController.lockedStatus && sealStatus)
        {
            sealStatus = false;
        }
    }

    void Leak()
    {
        if (waterLevel > .01)
        {
            leakStatus = true;
            tilt = (vDot + 1) / 2;
            leakRate = (waterLevel / 10) * (tilt / 10);

            if (tilt < minTilt)
            {
                leakRate = 0;

                if (pourStatus)
                {
                    EndPour();
                }
            }

            if (tilt > minTilt)
            {
                Pour();
            }

            waterLevel = waterLevel - leakRate;

            if (waterLevel >= 1)
            {
                waterLevel = 1;
            }

            lastWaterLevel = waterLevel;
        }

        if (waterLevel < .01)
        {
            EndPour();
        }
    }

    void Pour()
    {
        pourStatus = true;

        if (pourEM.enabled == false || pourSplashEM.enabled == false)
        {
            pourEM.enabled = true;
            pourSplashEM.enabled = true;
        }

        if (objectController.grabbedStatus)
        {
            objectController.attachedManipulator.Vibrate(leakVibration);
        }
    }

    void EndPour ()
    {
        pourStatus = false;
        pourEM.enabled = false;
        pourSplashEM.enabled = false;
    }
}