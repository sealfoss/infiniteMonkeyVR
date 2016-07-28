using UnityEngine;
using System.Collections;

public class SquirtController : MonoBehaviour {
    public InteractiveObjectController objectController;
    public Transform trigger;
    public Transform handleTrans;
    public AppendageController handle;
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
    public bool squirtStatus = false;
    public float pressure;
    public float depletionRate;
    public float minP;
    public float maxP;
    public float triggerPressSpeed;
    public float rechargeMultiplier;
    public float splashSize;

	void Start ()
    {
        objectController = GetComponent<InteractiveObjectController>();
        trigger = transform.FindChild("Trigger");

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

        pressure = 10;
        depletionRate = 0.01f;
        minP = 0.75f;
        maxP = 10;
        rechargeMultiplier = 10.0f;

        handleTrans = transform.FindChild("Handle");
        handle = handleTrans.GetComponent<AppendageController>();
        handle.minPos = -0.1385f;
        handle.maxDistance = 1.0f;
        handle.movementOffset = -2.0f;
        handle.movementResistance = 10;
    }
	
	void FixedUpdate ()
    {
        if (handle.grabbedStatus)
        {
            handleDistance = Vector3.Distance(this.transform.position, handle.transform.position);

            if (handleDistance < lastHandleDistance)
            {
                Recharge();
            }

            lastHandleDistance = handleDistance;
        }

        if (objectController.actionStatus && objectController.grabbedStatus)
        {
            PullTrigger();

            if (pressure > minP)
            {
                Squirt();
            }
        }

        if ((!objectController.actionStatus && squirtStatus) || !objectController.attachedManipulator)
        {
            ReleaseTrigger();

            if (squirtStatus)
            {
                ShutOff();
            }
        }
	}
    
    void Recharge ()
    {
        pressure = pressure + ((lastHandleDistance - handleDistance) * rechargeMultiplier);

        if (pressure > maxP)
        {
            pressure = maxP;
        }
    }

    void Squirt ()
    {
        var streamRate = streamEM.rate;
        var sprayRate = sprayEM.rate;
        var streamSprayRate = streamSprayEM.rate;
        var streamSprayShape = streamSpray.shape;
        float streamColorAlpha = (pressure / 20);
        Vector4 streamColor = new Vector4(255, 255, 255, streamColorAlpha);

        squirtStatus = true;
        pressure = pressure - depletionRate;

        stream.startSpeed = pressure * 2;
        spray.startSpeed = pressure / 8;
        streamSpray.startSpeed = pressure * 2;

        streamRate.constantMax = pressure * 50;
        sprayRate.constantMax = pressure * 5;
        streamSprayRate.constantMax = pressure * 10;

        stream.startSize = (pressure / 10);
        spray.startSize = (pressure / 5);
        streamSpray.startSize = (pressure / 100);

        streamSprayShape.angle = (pressure / 100);

        splash1.startSize = pressure / 6;
        splash2.startSize = Random.Range(0.01f, (pressure / 40));

        stream.startColor = streamColor;
        splash1.startColor = new Vector4(splash1.startColor.r, splash1.startColor.g, splash1.startColor.b, streamColorAlpha);
        splash2.startColor = new Vector4(splash2.startColor.r, splash2.startColor.g, splash2.startColor.b, streamColorAlpha);


        if (streamEM.enabled == false)
        {
            streamEM.enabled = true;
            sprayEM.enabled = true;
            streamSprayEM.enabled = true;
        }

        if (pressure < minP)
        {
            ShutOff();
        }
    }
    
    void ShutOff()
    {
        squirtStatus = false;
        streamEM.enabled = false;
        sprayEM.enabled = false;
        streamSprayEM.enabled = false;
    }

    void PullTrigger ()
    {
        trigger.localPosition = Vector3.Lerp(Vector3.zero, triggerPressPos, (Time.time * triggerPressSpeed));
    }

    void ReleaseTrigger()
    {
        trigger.localPosition = Vector3.Lerp(triggerPressPos, Vector3.zero, (Time.time * triggerPressSpeed));
    }
}
