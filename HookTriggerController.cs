using UnityEngine;
using System.Collections;

public class HookTriggerController : MonoBehaviour
{
    public LockController parent;
    public string soughtAfter = "BronzeLock_Key";
    // Use this for initialization
    void Start()
    {
        parent = GetComponentInParent<LockController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (parent == null)
        {
            parent = GetComponentInParent<LockController>();
        }
    }
    void OnTriggerEnter(Collider collided)
    {
        if (collided.name.Contains("BronzeLock_Hook") && parent.lockClosed == false && parent.closeReset == true)
        {
            parent.CloseLock();
        }
    }

}
