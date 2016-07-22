using UnityEngine;
using System.Collections;

public class SquirtController : MonoBehaviour {
    public InteractiveObjectController objectController;
    public Transform trigger;
    public Transform handleTrans;
    public AppendageController handle;
    private float handleDistance;
    private float lastHandleDistance;
    public Transform sprayTrans;
    public ParticleSystem spray;
    public Vector3 triggerPressPos;
    public Vector3 triggerPos;
    public bool squirtStatus = false;
    public float pressure;
    private float lastPressure;
    public float depletionRate;
    public float minP;
    public float maxP;
    public float triggerPressSpeed;
    public float rechargeMultiplier;
    // Use this for initialization
	void Start ()
    {
        objectController = GetComponent<InteractiveObjectController>();
        trigger = transform.FindChild("Trigger");
        
        sprayTrans = transform.FindChild("Spray");
        spray = sprayTrans.GetComponent<ParticleSystem>();
        spray.enableEmission = false;
        triggerPressPos = new Vector3(0, 0, -0.01f);
        triggerPos = new Vector3(0, 0, 0);
        triggerPressSpeed = 0.5f;
        pressure = 10;
        depletionRate = 0.015f;
        minP = 0.1f;
        maxP = 10;
        rechargeMultiplier = 10.0f;

        handleTrans = transform.FindChild("Handle");
        handle = handleTrans.GetComponent<AppendageController>();
        handle.minPos = -0.1385f;
        handle.maxDistance = 2.0f;
        handle.movementOffset = -2.0f;
        handle.movementResistance = 10;
    }
	
	// Update is called once per frame
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

        if (objectController.actionStatus)// && !squirtStatus)
        {
            PullTrigger();

            if (pressure > minP)
            {
                Squirt();
            }
        }

        if (!objectController.actionStatus && squirtStatus)
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
        squirtStatus = true;
        pressure = pressure - depletionRate;
        lastPressure = pressure;
        spray.startSpeed = pressure * 2;
        spray.emissionRate = pressure * 50;
        spray.enableEmission = true;

        if (pressure < minP)
        {
            ShutOff();
        }
    }
    
    void ShutOff()
    {
        squirtStatus = false;
        spray.enableEmission = false;
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
