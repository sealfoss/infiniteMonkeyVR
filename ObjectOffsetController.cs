using UnityEngine;
using System.Collections;

public class ObjectOffsetController : MonoBehaviour {

	void Awake () {
    
	}

	void Update () {
        InteractiveObjectController parent = GetComponentInParent<InteractiveObjectController>();
        parent.SetOffset(this);
    }
}
