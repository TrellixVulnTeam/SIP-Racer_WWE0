using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Checkpoints checkpointsParent;
    public bool finishline;
    public Camera cam;
    private void Awake()
    {
        checkpointsParent = GetComponentInParent<Checkpoints>();
    }
    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.tag == "Car")
        {
            if(other.gameObject.GetComponentInParent<CheckpointManager>() != null)
            {
                other.gameObject.GetComponentInParent<CheckpointManager>().checkpointReached(this);
                if(finishline)
                {
                    checkpointsParent.finishers++;
                    other.GetComponent<CheckpointManager>().finishlineReached(checkpointsParent.finishers);
                }
                if(cam != null && !cam.transform.gameObject.activeInHierarchy)
                {
                    cam.transform.gameObject.SetActive(true);
                }
            }
        }
        
    }
}
