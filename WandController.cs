using UnityEngine;
using System.Collections;

public class WandController : MonoBehaviour {
    ManipulatorController manipulator;
    InteractiveObjectController objectController;
    bool sparkleStatus;

    public ParticleSystem sparks;
    ParticleSystem.EmissionModule sparksEM;
    public Light sparksLight;
    Rigidbody rigidbody;

    public Transform head;
    public Transform wandTip;
    float maxX;
    float maxY;
    float maxZ;

    public GameObject grid;
    public GameObject[] slots = new GameObject[16];
    GameObject newGrid;
    //bool[,] gridSwitches = new bool[4,4];
    int lastRow;
    int lastColumn;
    int lastQuad;
    bool gridStatus;
    int direction;
    int lastDirection;
    public int sequenceLength;
    int[] sequence;
    public bool directionUp;
    public bool directionDown ;
    public bool directionRight;
    public bool directionLeft;


    // Use this for initialization
    void Start () {
        rigidbody = this.GetComponent<Rigidbody>();

        sparksEM = sparks.emission;
        sparksEM.enabled = false;
        sparksLight.enabled = false;
        objectController = GetComponent<InteractiveObjectController>();
        maxX = 0;
        maxY = 0;
        maxZ = 0;
        gridStatus = false;

        if (sequenceLength == 0)
        {
            sequenceLength = 4;
        }

        sequence = new int[sequenceLength];

        for (int i = 0; i < sequenceLength; i++)
        {
            sequence[i] = 0;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (objectController.grabbedStatus)
        {
            if (!manipulator)
            {
                manipulator = objectController.attachedManipulator;
            }

            if(manipulator)
            {
                if(manipulator.actionStatus)
                {
                    Locate();
                    Sparkle();
                }

                if(!manipulator.actionStatus && sparkleStatus)
                {
                    EndSparkle();
                    DestroyGrid();
                }
            }
        }

        if (!objectController.grabbedStatus)
        {
            if (manipulator)
            {
                manipulator = null;
            }

            if (sparkleStatus)
            {
                EndSparkle();
                DestroyGrid();
            }
        }
	}

    void Sparkle()
    {
        sparksEM.enabled = true;
        sparksLight.enabled = true;
        sparkleStatus = true;
    }

    void EndSparkle()
    {
        sparksEM.enabled = false;
        sparksLight.enabled = false;
        sparkleStatus = false;

        maxX = 0;
        maxY = 0;
        maxZ = 0;
    }

    void ActivateFire()
    {
        print("FIRE!");
    }

    void AddToSequence(int newDirection)
    {
        // go through sequence, and put newDirection at the first open slot
        for(int i = 0; i < sequenceLength; i++)
        {
            if (sequence[i] == 0)
            {
                sequence[i] = newDirection;
                break;
            }

            //if you get to the end of sequence, but there are no empty slots, zero out the entire array and start over
            if (sequence[sequenceLength - 1] != 0 && sequence[sequenceLength - 1] != newDirection)
            {
                for (int j = 0; j < sequenceLength; j++)
                {
                    sequence[j] = 0;
                }

                sequence[0] = newDirection;
            }
        }
    }

    public void DetectDirectionBad (int row, int column)
    {
        int quad = 0;

        if (row < 3 && column < 3)
        {
            quad = 1;
        }

        if (row < 3 && column > 2)
        {
            quad = 2;
        }

        if (row > 2 && column < 3)
        {
            quad = 3;
        }

        if (row > 2 && column > 2)
        {
            quad = 4;
        }

        if (lastQuad != 0 && quad != lastQuad)
        {
            //print("row = " + row + ", last row = " + lastRow);
            directionUp = false;
            directionDown = false;
            directionRight = false;
            directionLeft = false;

            if (row < lastRow)
            {
                directionUp = true;
                //directionDown = false;
            }

            if (row > lastRow)
            {
                directionDown = true;
                directionUp = false;
            }

            if (column > lastColumn)
            {
                //print("Direction right");
                directionRight = true;
                //directionLeft = false;
            }

            if (column < lastColumn)
            {
                //print("Direction left");
                directionLeft = true;
                //directionRight = false;
            }

            if (directionUp) //1 is up, 2 is up/right, 3 is right, 4 is down/right, 5 is down, 6 is down/left, 7 is left, 8 is up/left
            {
                direction = 1;
                {
                    if (directionRight)
                    {
                        direction = 2;
                    }

                    if (directionLeft)
                    {
                        direction = 8;
                    }
                }
            }

            if (directionDown)
            {
                direction = 5;
                {
                    if (directionRight)
                    {
                        direction = 4;
                    }

                    if (directionLeft)
                    {
                        direction = 6;
                    }
                }
            }

            if (directionRight && (!directionUp && !directionDown))
            {
                direction = 3;
            }

            if (directionLeft && (!directionUp && !directionDown))
            {
                direction = 7;
            }

            if (direction != lastDirection && direction != 0)
            {
                print("new direction = " + direction);
                AddToSequence(direction);
            }
        }
        
        if (lastQuad != quad)
        {
            lastQuad = quad;
        }

        lastRow = row;
        lastColumn = column;
        lastDirection = direction;
    }

    void DestroyGrid()
    {
        //Destroy(newGrid.gameObject);
        gridStatus = false;
    }

    void BuildGridBad()
    {
        Transform buildLocation = new GameObject().transform;
        buildLocation.parent = head;
        buildLocation.localPosition = new Vector3(0, 0, 0.75f);
        buildLocation.localRotation = Quaternion.identity;
        newGrid = (GameObject)GameObject.Instantiate(grid);
        newGrid.transform.position = buildLocation.position;
        newGrid.transform.rotation = buildLocation.rotation;
        newGrid.transform.localEulerAngles = new Vector3(0, newGrid.transform.localEulerAngles.y, 0);
        //newGrid.transform.parent = head;
        gridStatus = true;
    }

    void Locate()
    {
        /**if (!gridStatus)
        {
            BuildGrid();
        }**/

        var localVelocity = transform.InverseTransformDirection(rigidbody.velocity);
        print("localVelocity = " + localVelocity);

        Transform tracker = new GameObject().transform;
        tracker.parent = head;
        tracker.position = wandTip.position;

        if (Mathf.Abs(tracker.localPosition.x) > Mathf.Abs(maxX))
        {
            maxX = tracker.localPosition.x;
        }

        if (Mathf.Abs(tracker.localPosition.y) > Mathf.Abs(maxY))
        {
            maxY = tracker.localPosition.y;
        }

        if (Mathf.Abs(tracker.localPosition.z) > Mathf.Abs(maxZ))
        {
            maxZ = tracker.localPosition.z;
        }

        //print("max X = " + maxX + ", max y = " + maxY + ", max z = " + maxZ);
        Destroy(tracker.gameObject);
    }
}
