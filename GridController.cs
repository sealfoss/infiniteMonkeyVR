using UnityEngine;
using System.Collections;

public class GridController : MonoBehaviour
{
    public int row;
    public int column;
    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        WandController wand = other.GetComponent<WandController>();

        if (wand)
        {
            //wand.DetectDirection(row, column);
        }
    }
}
