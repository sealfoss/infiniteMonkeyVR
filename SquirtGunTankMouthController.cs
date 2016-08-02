using UnityEngine;
using System.Collections;

public class SquirtGunTankMouthController : MonoBehaviour
{
    public SquirtGunTankController tankController;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (tankController.leakStatus)
        {
            tankController.vDot = GetAngle();
        }
    }

    float GetAngle()
    {
        return (Vector3.Dot(transform.up, Vector3.down));
    }
}
