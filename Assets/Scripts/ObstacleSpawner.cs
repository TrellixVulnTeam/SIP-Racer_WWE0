using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstacle;
    public float spawnZoneSize;
    private Vector3 obstacleSize;
    private Vector3 leftBound;
    private Vector3 rightBound;
    private Vector3 distance;
    public void SpawnObstacle()
    {
        obstacleSize = obstacle.GetComponent<MeshRenderer>().bounds.extents;
        rightBound = transform.position + transform.right * (spawnZoneSize - obstacleSize.x);
        leftBound = transform.position + -transform.right * (spawnZoneSize - obstacleSize.x);
        Object.Instantiate(obstacle, leftBound + (Random.value * (rightBound - leftBound)), transform.rotation);
    }

    public void RemoveObstacle()
    {
        //var obj = GetComponentInChildren();
    }
    private void OnDrawGizmosSelected() {
        obstacleSize = obstacle.GetComponent<MeshRenderer>().bounds.extents;
        var temprightBound = transform.position + transform.right * (spawnZoneSize);
        var templeftBound = transform.position + -transform.right * (spawnZoneSize);
        rightBound = transform.position + transform.right * (spawnZoneSize - obstacleSize.x);
        leftBound = transform.position + -transform.right * (spawnZoneSize - obstacleSize.x);
        Gizmos.color= Color.green;
        Gizmos.DrawWireSphere(temprightBound, .5f);
        Gizmos.DrawWireSphere(templeftBound, .5f);
        Gizmos.DrawWireCube(rightBound, new Vector3(.2f,.2f, .2f));
        Gizmos.DrawWireCube(leftBound, new Vector3(.2f,.2f, .2f));
    }
}
