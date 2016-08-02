using UnityEngine;
using System.Collections;

public class RefillController : MonoBehaviour {

    SquirtGunTankController tankController;
    public ushort vibration;
    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnParticleCollision(GameObject hitObj)
    {
        InteractiveObjectController objController = hitObj.GetComponent<InteractiveObjectController>();
        tankController = hitObj.GetComponent<SquirtGunTankController>();

        if (tankController)
        {
            tankController.waterLevel = tankController.waterLevel + 0.01f;
        }

        if (objController)
        {
            if (objController.grabbedStatus)
            { 
                objController.attachedManipulator.Vibrate(vibration);
            }
        }
    }
}
