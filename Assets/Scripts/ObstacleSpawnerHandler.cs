using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawnerHandler : MonoBehaviour
{
    public List<ObstacleSpawner> spawners;
    void Awake()
    {
        spawners = new List<ObstacleSpawner>(GetComponentsInChildren<ObstacleSpawner>());
    }

    private void RemoveObstacles()
    {
        var obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        if(obstacles != null)
        {
            foreach(var obstacle in obstacles)
            {
                Object.Destroy(obstacle);
            }
        }
    }
    public void resetObstacles() {
        RemoveObstacles();
        foreach(var spawner in spawners)
        {
            spawner.SpawnObstacle();
        }
    }
}
