using UnityEngine;
using System.Collections;

public class SquirtGunStreamController : MonoBehaviour
{
    public float force;
    private SquirtGunController squirt;
    // Use this for initialization
    void Start()
    {
        squirt = GetComponentInParent<SquirtGunController>();
    }

    // Update is called once per frame
    void Update()
    {
        force = (squirt.pressure / 20);
    }

    void OnParticleCollision(GameObject hitObj)
    {
        Rigidbody hitBody = hitObj.GetComponent<Rigidbody>();

        if (hitBody)
        {
            Vector3 direction = (hitObj.transform.position - transform.position) * force;
            hitBody.AddForce(direction, ForceMode.Impulse);
        }
    }
}