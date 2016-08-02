using UnityEngine;
using System.Collections;

public class SquirtGunController : MonoBehaviour
{
    public InteractiveObjectController objectController;
    public Transform trigger;
    public Transform handleTrans;
    public InteractiveObjectController handle;
    private float handleDistance;
    private float lastHandleDistance;

    public Transform streamTrans;
    public ParticleSystem stream;
    public ParticleSystem.EmissionModule streamEM;

    public Transform sprayTrans;
    public ParticleSystem spray;
    public ParticleSystem.EmissionModule sprayEM;

    public Transform streamSprayTrans;
    public ParticleSystem streamSpray;
    public ParticleSystem.EmissionModule streamSprayEM;

    public Transform splash1Trans;
    public ParticleSystem splash1;
    public ParticleSystem.EmissionModule splash1EM;

    public Transform splash2Trans;
    public ParticleSystem splash2;
    public ParticleSystem.EmissionModule splash2EM;

    public Vector3 triggerPressPos;
    public Vector3 triggerPos;
    public bool squirtStatus;
    public bool tankStatus;
    public float waterLevel;
    public float minWaterLevel;
    public float waterDepletionRate;
    public float pressure;
    public float pressureDepletionRate;
    public float minP;
    public float maxP;
    public float triggerPressSpeed;
    public float rechargeMultiplier;
    public float splashSize;

    //tank specific variables
    public PlugController tankPlug;
    public InteractiveObjectController tankObj;
    public SquirtGunTankController tankController;
    public string tankName;
    public float tankLockPosThreshold;
    public float tankRot;

    //vibration variables
    public float squirtVibrationMultiplier;

    //audio variables
    AudioSource audioSrc;
    public AudioClip spraySnd;
    public float volume;

    void Start()
    {
        audioSrc = GetComponent<AudioSource>();

        objectController = GetComponent<InteractiveObjectController>();
        trigger = transform.FindChild("Trigger");
        tankName = "SquirtGun_Tank";
        tankPlug.soughtForName = tankName;

        streamTrans = transform.FindChild("Stream");
        stream = streamTrans.GetComponent<ParticleSystem>();
        streamEM = stream.emission;
        streamEM.enabled = false;

        sprayTrans = transform.FindChild("Spray");
        spray = sprayTrans.GetComponent<ParticleSystem>();
        sprayEM = spray.emission;
        sprayEM.enabled = false;

        streamSprayTrans = transform.FindChild("StreamSpray");
        streamSpray = streamSprayTrans.GetComponent<ParticleSystem>();
        streamSprayEM = streamSpray.emission;
        streamSprayEM.enabled = false;

        splash1Trans = transform.FindChild("Stream/Splash1");
        splash1 = splash1Trans.GetComponent<ParticleSystem>();
        splash1EM = splash1.emission;

        splash2Trans = transform.FindChild("Stream/Splash2");
        splash2 = splash2Trans.GetComponent<ParticleSystem>();
        splash2EM = splash2.emission;

        triggerPressPos = new Vector3(0, 0, -0.01f);
        triggerPos = new Vector3(0, 0, 0);
        triggerPressSpeed = 0.5f;

        waterLevel = 1.0f;
        minWaterLevel = 0.985f;
        waterDepletionRate = 0.000005f;
        pressure = 0;
        pressureDepletionRate = 0.000005f;
        minP = 0.75f;
        maxP = 10;
        rechargeMultiplier = 10.0f;

        handleTrans = transform.FindChild("Handle");
        handle = handleTrans.GetComponent<InteractiveObjectController>();
        handle.minPos = -0.1385f;
        handle.maxDistance = 1.0f;
        handle.movementResistance = 10;
        squirtStatus = false;
    }

    void FixedUpdate()
    {
        if (handle.grabbedStatus)
        {
            handleDistance = Vector3.Distance(this.transform.position, handle.transform.position);

            if (handleDistance > lastHandleDistance)
            {
                Recharge();
            }

            lastHandleDistance = handleDistance;
        }

        if (objectController.grabbedStatus)
        {
            if (objectController.attachedManipulator.actionStatus)
            {
                PullTrigger();

                if (pressure > minP)
                {
                    Squirt();
                }
            }

            if (!objectController.attachedManipulator.actionStatus || !objectController.attachedManipulator)
            {
                ReleaseTrigger();

                if (squirtStatus)
                {
                    ShutOff();
                }
            }
        }

        if (tankPlug.pluggedStatus)
        {
            if (!tankObj)
            {
                tankObj = tankPlug.pluggedObj;
                tankController = tankPlug.pluggedObj.GetComponent<SquirtGunTankController>();
                waterLevel = tankController.waterLevel;
            }

            if(tankPlug.pluggedObj.attachedManipulator)
            {
                ManipulateTank();
            }
        }

        if (tankObj && !tankPlug.pluggedStatus)
        {
            tankObj = null;
        }
    }

    void ManipulateTank ()
    {
        tankRot = tankObj.transform.localEulerAngles.z;

        //make a note when the tank is past the tank lock position threshold
        if (tankObj.transform.localPosition.z < tankLockPosThreshold && !tankPlug.lockPosStatus)
        {
            tankPlug.lockPosStatus = true;
        }

        //erase the note when it goes back across the threshold on its way out
        if (tankObj.transform.localPosition.z > tankLockPosThreshold && tankPlug.lockPosStatus)
        {
            tankObj.transform.rotation = tankPlug.guide.rotation;
            tankObj.rotationAmount = 0;
            tankPlug.lockPosStatus = false;
        }

        // if the tank is within the threshold, and has been rotated past the rotation lock threshold, disallow lateral movement, and set the object to locked.
        if (tankPlug.lockPosStatus && !tankObj.lockedStatus && tankObj.revolutions == 1)
        {
            tankObj.moveAxisCondition = 0;
            tankObj.lockedStatus = true;
        }

        //if the tank is within the threshold and rotates back under the rotation lock threshold, re-enable lateral movement, unlock the object.
        if (tankPlug.lockPosStatus && tankObj.lockedStatus && tankObj.revolutions == 0)
        {
            tankObj.moveAxisCondition = 3;
            tankObj.lockedStatus = false;
        }

        if (tankObj.revolutions == tankObj.maxRevolutions && !tankObj.stopRot)
        {
            tankObj.stopRot = true;
            tankStatus = true;
        }

        if (tankObj.stopRot && tankObj.revolutions < tankObj.maxRevolutions)
        {
            tankObj.stopRot = false;
            tankStatus = false;
        }
    }

    void Recharge()
    {
        pressure = pressure + (Mathf.Abs(lastHandleDistance - handleDistance) * rechargeMultiplier);

        if (pressure > maxP)
        {
            pressure = maxP;
        }

        ushort vibration = (ushort)(pressure * squirtVibrationMultiplier);
        handle.attachedManipulator.Vibrate(vibration);

    }

    void Squirt()
    {
        //turn on the particle effect
        if (streamEM.enabled == false)
        {
            streamEM.enabled = true;
            sprayEM.enabled = true;
            streamSprayEM.enabled = true;
        }

        //tell the gun it's squirting
        squirtStatus = true;

        //if the tank isn't attached, there is no water
        if (!tankStatus)
        {
            waterLevel = 0;
        }

        // if the tank is attached, caluclate water level over time, as the gun is fired
        else
        {
            waterLevel = waterLevel - waterDepletionRate;

            if (waterLevel <= minWaterLevel)
            {
                waterLevel = 0;
            }
        }

        //calculate water pressure
        pressure = (pressure - pressureDepletionRate) * waterLevel;

        //prevent negative pressure from occuring, because THATS IMPOSSIBLE!!(*$&(@!$
        if (pressure < 0)
        {
            pressure = 0;
        }

        //if water pressure drops below a threshold, stop squirting
        if (pressure < minP)
        {
            ShutOff();
        }

        //here are the functions controlling the squirting particle effect used when firing the gun
        //the particle system attrbutes are influenced by water pressure
        var streamRate = streamEM.rate;
        var sprayRate = sprayEM.rate;
        var streamSprayRate = streamSprayEM.rate;
        var streamSprayShape = streamSpray.shape;

        //stream color and alpha variables
        float streamColorAlpha = (pressure / 15);
        Vector4 streamColor = new Vector4(255, 255, 255, streamColorAlpha);

        //stream particle system functions
        stream.startSpeed = pressure * 2;
        stream.startSize = (pressure / 10);
        stream.startColor = streamColor;
        streamRate.constantMax = pressure * 50;

        //spray particle system functions
        spray.startSpeed = pressure / 8;
        spray.startSize = (pressure / 5);
        sprayRate.constantMax = pressure * 5;

        //streamSpray particle system functions
        streamSpray.startSpeed = pressure * 2;
        streamSpray.startSize = (pressure / 100);
        streamSprayRate.constantMax = pressure * 10;
        streamSprayShape.angle = (pressure / 100);

        //splash1 particle system functions
        splash1.startSize = pressure / 6;
        splash1.startColor = new Vector4(splash1.startColor.r, splash1.startColor.g, splash1.startColor.b, streamColorAlpha);

        //splash1 particle system functions
        splash2.startSize = Random.Range(0.01f, (pressure / 40));
        splash2.startColor = new Vector4(splash2.startColor.r, splash2.startColor.g, splash2.startColor.b, streamColorAlpha);

        //vibrate the controller accordingly
        ushort vibration = (ushort)(pressure * squirtVibrationMultiplier);
        objectController.attachedManipulator.Vibrate(vibration);

        if (handle.grabbedStatus)
        {
            handle.attachedManipulator.Vibrate(vibration);
        }

        //play sound

        if (!audioSrc.enabled || !audioSrc.isPlaying)
        {
            
            audioSrc.enabled = true;
            print("play Audio!");
            audioSrc.PlayOneShot(spraySnd, (pressure * volume));
        }
    }

    void ShutOff()
    {
        squirtStatus = false;
        streamEM.enabled = false;
        sprayEM.enabled = false;
        streamSprayEM.enabled = false;
        audioSrc.enabled = false;
    }

    void PullTrigger()
    {
        trigger.localPosition = Vector3.Lerp(Vector3.zero, triggerPressPos, (Time.time * triggerPressSpeed));
    }

    void ReleaseTrigger()
    {
        trigger.localPosition = Vector3.Lerp(triggerPressPos, Vector3.zero, (Time.time * triggerPressSpeed));
    }
}