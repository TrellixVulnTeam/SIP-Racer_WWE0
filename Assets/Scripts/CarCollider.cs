using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCollider : MonoBehaviour
{
    public CheckpointManager checkpointManager;
    private void OnCollisionStay(Collision other) {
        if(other.gameObject.tag == "Ground")
        {
            checkpointManager.wallPenalty();
        }
    }
}
