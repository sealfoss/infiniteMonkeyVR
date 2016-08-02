using UnityEngine;
using System.Collections;

public class FistController : MonoBehaviour {
    public ManipulatorConroller manipulator;
    bool fistStatus;
    public int powerLevel;
    public int maxPowerLevel;
    public float fistForceMultiplier;
    public ParticleSystem sparkleEffect;
    ParticleSystem.EmissionModule sparkleEM;
    // Use this for initialization
    void Start() {
        sparkleEM = sparkleEffect.emission;
        sparkleEM.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (manipulator)
        {
            if (manipulator.actionStatus && !manipulator.grabbedObj)
            {
                CloseFist();
            }

            if (fistStatus && !!manipulator.actionStatus)
            {
                OpenFist();
            }

            if (fistStatus && (powerLevel > 0))
            {
                Sparkle();
            }
        }
    }

    void Sparkle()
    {
        sparkleEM.enabled = true;
    }

    void CloseFist()
    {
        var sparkleRate = sparkleEM.rate;
        fistStatus = true;
        GetComponent<Collider>().isTrigger = false;

        if (powerLevel < maxPowerLevel)
        {
            powerLevel = powerLevel + 1;
        }

        if (powerLevel != 0)
        {
            switch (powerLevel)
            {
                case 1:
                    sparkleRate.constantMax = 10;
                    sparkleEffect.startColor = Color.blue;
                    break;
                case 2:
                    sparkleRate.constantMax = 20;
                    sparkleEffect.startColor = Color.yellow;
                    break;
                case 3:
                    sparkleRate.constantMax = 30;
                    sparkleEffect.startColor = Color.red;
                    break;
            }
        }
    }

    void OpenFist ()
    {
        fistStatus = false;
        GetComponent<Collider>().isTrigger = true;
        sparkleEM.enabled = false;
    }

    Vector3 GetDirection(GameObject target)
    {
        var heading = target.transform.position - this.transform.position;
        var distance = heading.magnitude;
        var direction = heading / distance;
        return direction;
    }

    void OnCollisionEnter(Collision col)
    {
        Rigidbody hitBody = col.rigidbody;

        print("BLAM!");

        if (hitBody)
        {
            var direction = GetDirection(col.gameObject);
            hitBody.AddForce(direction * (powerLevel * fistForceMultiplier), ForceMode.Impulse);
            powerLevel = 0;
        }
    }
}
