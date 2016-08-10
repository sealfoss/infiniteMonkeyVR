using UnityEngine;
using System.Collections;

public class WandJetController : MonoBehaviour {

    public float force;
    private WandController wand;
    // Use this for initialization
    void Start()
    {
        wand = GetComponentInParent<WandController>();
    }

    // Update is called once per frame
    void Update()
    {
        force = (wand.fuel);
    }

    void OnParticleCollision(GameObject hitObj)
    {
        Rigidbody hitBody = hitObj.GetComponent<Rigidbody>();
        InteractiveObjectController hitIOC = hitObj.GetComponent<InteractiveObjectController>();

        if (hitBody)
        {
            Vector3 direction = (hitObj.transform.position - transform.position) * force;
            hitBody.AddForce(direction, ForceMode.Impulse);
        }

        if (hitIOC)
        {
            hitIOC.effectStatus = true;
        }
    }
}