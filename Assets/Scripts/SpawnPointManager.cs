using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointManager : MonoBehaviour
{
    private List<SpawnPoint> spawns;
    private List<SpawnPoint> active;
    // Start is called before the first frame update
    void Awake()
    {
        spawns = new List<SpawnPoint>(GetComponentsInChildren<SpawnPoint>());
        active = new List<SpawnPoint>(spawns);
    }

    public void Reset()
    {
        active = new List<SpawnPoint>(spawns);
        foreach(var point in active)
        {
            point.active = true;
        }
    }
    public int getNumSpawns()
    {
        if(spawns != null)
        {
            return spawns.Count;
        }
        else
        {
            Awake();
            return spawns.Count;
        }
    }
    public Vector3 getSpawnPoint()
    {
        int rnd = Random.Range(0, active.Count);
        Vector3 pos = active[rnd].transform.position;
        active[rnd].active = false;
        active.Remove(active[rnd]);
        return pos;
    }
}
