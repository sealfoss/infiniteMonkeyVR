using UnityEngine;
using System.Collections;

public class LockController : MonoBehaviour {
    public float thrust;
    public bool closeReset = false;
    private SocketController keySocket = null;
    private Transform keySocketTrans = null;
    InteractiveObjectController keyObj = null;
    public float keyThresholdPos;
    public float keyThresholdRot;
    public bool keySocketStatus = false;
    public bool lockClosed;
    public InteractiveObjectController lockHook;
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
        thrust = 0.5f;
    }
	
	// Update is called once per frame
	void Update () {
        if (keySocket.plugObj)
        {
            if ((lockHook.transform.localPosition.y < -0.0024f)  && closeReset == false)
            {
                closeReset = true;
            }

            if (keyObj == null || keyObj != keySocket.plugObj)
            {
                keyObj = keySocket.plugObj;
            }

            if (keyObj.lockedStatus == false)
            {
                if (keySocket.socketPos > keyThresholdPos && keySocket.minRot != -90.0f)
                {
                    keySocket.minRot = -90.0f;
                }

                if (keySocket.socketPos < keyThresholdPos && keySocket.minRot != 0.0f)
                {
                    keySocket.minRot = 0.0f;
                }

                if (keySocket.socketRot < keyThresholdRot && keySocket.minPos != keySocket.maxPos)
                {
                    keySocket.minPos = keySocket.maxPos;
                }

                if (keySocket.socketRot > keyThresholdRot && keySocket.minPos == keySocket.maxPos)
                {
                    keySocket.minPos = -0.15f;
                }

                if (keySocket.socketRot == keySocket.minRot && keySocket.socketPos > keyThresholdPos)
                {
                    print("locked!");
                    keyObj.Lock();
                    if (lockClosed == true)
                    {
                        OpenLock();
                    }
                }
            }

            if (keyObj.lockedStatus == true)
            {
                if (keySocket.trackerRot > -45)
                {
                    keyObj.transform.rotation = Quaternion.Lerp(this.transform.rotation, keyObj.socketObj.transform.rotation, Time.deltaTime * 5);
                    print("unlocked!");
                    keyObj.Unlock();
                }
            }
        }
	}

    public void CloseLock ()
    {
        lockHook.lockedStatus = true;
        Vector3 closedPosition = new Vector3(0, -0.0275f, 0);
        //Destroy(lockHook.GetComponent<ConfigurableJoint>());
        //Destroy(lockHook.GetComponent<Rigidbody>());
        lockHook.transform.localRotation = Quaternion.identity;
        lockHook.transform.localPosition = Vector3.Lerp(lockHook.transform.localPosition, closedPosition, Time.deltaTime * 5);
        lockHook.GetComponent<ConfigurableJoint>().xMotion = ConfigurableJointMotion.Locked;
        lockHook.GetComponent<ConfigurableJoint>().angularXMotion = ConfigurableJointMotion.Locked;
        lockClosed = true;
        closeReset = false;
    }

    public void OpenLock ()
    {
        ConfigurableJoint joint = GetComponent<ConfigurableJoint>();
        SoftJointLimit jointLimit = new SoftJointLimit();
        jointLimit.limit = 0.03f;
        /**lockHook.gameObject.AddComponent<Rigidbody>();
        lockHook.GetComponent<Rigidbody>().isKinematic = true;
        lockHook.GetComponent<Rigidbody>().useGravity = false;
        lockHook.GetComponent<Rigidbody>().mass = lockHook.rbMass;
        lockHook.GetComponent<Rigidbody>().drag = lockHook.rbDrag;
        lockHook.GetComponent<Rigidbody>().angularDrag = lockHook.rbADrag;
        lockHook.rigidBody = GetComponent<Rigidbody>();**/
        lockHook.GetComponent<ConfigurableJoint>().xMotion = ConfigurableJointMotion.Limited;
        lockHook.GetComponent<ConfigurableJoint>().linearLimit = jointLimit;
        lockHook.GetComponent<ConfigurableJoint>().angularXMotion = ConfigurableJointMotion.Free;
        lockClosed = false;
        lockHook.lockedStatus = false;
        lockHook.GetComponent<Rigidbody>().AddForce(0, thrust, 0, ForceMode.Impulse);
    }


}
