using UnityEngine;
using System.Collections;

public class LockController : MonoBehaviour {
    SocketController keySocket = null;
    Transform keySocketTrans = null;
    InteractiveObjectController keyObj = null;
    float keyThresholdPos;
    float keyThresholdRot;
    public bool keySocketStatus = false;

    private bool keyIn = false;
	// Use this for initialization
	void Start () {
        keySocketTrans = this.transform.FindChild("KeySocket");
        keySocket = keySocketTrans.GetComponent<SocketController>();
        keySocket.maxPos = -0.0525f;
        keySocket.minPos = -0.15f;
        keySocket.minRot = -90.0f;
        keySocket.maxRot = 0;
        keySocket.releasePos = -0.24f;
        keySocket.maxDistance = 0.275f;
        keyThresholdPos = -0.055f;
        keyThresholdRot = -5.0f;
    }
	
	// Update is called once per frame
	void Update () {
        if (keySocket.plugObj)
        {
            if (keyObj == null || keyObj != keySocket.plugObj)
            {
                keyObj = keySocket.plugObj;
            }

            if (keyObj.lockedStatus == false)
            {
                if (keySocket.socketPos > keyThresholdPos)
                {
                    keySocket.minRot = -90.0f;
                    //keySocket.minPos = -0.0525f;
                    print("key reached threshold pos, minRot = " + keySocket.minRot);
                }

                if (keySocket.socketPos < keyThresholdPos)
                {
                    keySocket.minRot = 0.0f;
                }

                if (keySocket.socketRot == keySocket.minRot && keySocket.socketPos > keyThresholdPos)
                {
                    print("locked!");
                    keyObj.Lock();
                }
            }

            if (keyObj.lockedStatus == true)
            {
                if (keySocket.trackerRot > -45)
                {
                    print("unlocked!");
                    keyObj.Unlock();
                }
            }
        }
	}
}
