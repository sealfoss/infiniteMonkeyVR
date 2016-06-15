using UnityEngine;
using System.Collections;

public class ObjectOffsetController : MonoBehaviour {

	void Awake () {
        InteractiveObjectController parent = GetComponentInParent<InteractiveObjectController>();
        parent.offsetPoint = this.transform;
        parent.offsetStatus = true;
    }

	void Update () {

    }
}
