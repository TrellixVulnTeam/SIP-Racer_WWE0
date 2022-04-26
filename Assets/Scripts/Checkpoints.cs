using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoints : MonoBehaviour
{
    public List<Checkpoint> checkpoints {get;private set;}
    public int finishers;
    // Start is called before the first frame update
    void Awake()
    {
        checkpoints = new List<Checkpoint>(GetComponentsInChildren<Checkpoint>());
        checkpoints[checkpoints.Count-1].finishline = true;
    }
}
