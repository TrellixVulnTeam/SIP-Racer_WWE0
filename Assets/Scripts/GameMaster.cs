using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.Barracuda;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;

public class GameMaster : MonoBehaviour
{
    [Header("Agent Settings")]
    public GameObject RlAgentPrefab;
    public GameObject MmAgentPrefab;
    private int numAgents;
    private int finishedAgents;
    public NNModel RLBrain;
    public NNModel ModalBrain;
    public NNModel ModalSubABrain;
    public NNModel ModalSubBBrain;
    public NNModel ModalSubCBrain;
    /*
        Spawn Modes:
        0: Default, no model and RL
        1: RL with RLBrain as model
        2: No model with Modal setup
        3: Training Sub-A(Turning)
        4: Training Sub-B(Obstacles)
        5: Training Sub-C(Players)
        6: Modal model with ModalBrain as model as well as models for sub
        7: Mixes both 
    */
    public int spawnMode;
    public int episodes = 0;
    [Header("SET TO MODE 0 OR 2 FIRST")]
    public bool heuristic = false;
    [Header("Checkpoint Controller Tweaks")]
    public float maxTime = 30f;
    public bool racing = true;
    [Header("Track Settings")]
    public GameObject simpleTrack;
    public GameObject fullTrack;
    private string startingColor;
    void Awake() {
        //If this hits, the agent needs to train on a simplified environment
        if(spawnMode == 4 || spawnMode == 5)
        {
            fullTrack.SetActive(false);
            simpleTrack.SetActive(true);
        }

        //If agent is Reinforcement Learning Model
        if(spawnMode == 0 || spawnMode == 1)
        {
            if(spawnMode == 1)
            {
                var behavior = RlAgentPrefab.transform.GetComponentInChildren<BehaviorParameters>();
                behavior.Model = RLBrain;
                behavior.BehaviorType = BehaviorType.InferenceOnly;
                startingColor = "white";
            }
            else
            {
                var behavior = RlAgentPrefab.transform.GetComponentInChildren<BehaviorParameters>();
                behavior.Model = null;
                if(heuristic)
                    behavior.BehaviorType = BehaviorType.HeuristicOnly;
                else
                    behavior.BehaviorType = BehaviorType.Default;
            }
        }
        
        //If agent is Modal Model or a sub model
        if(spawnMode >= 2 && spawnMode != 7)
        {
            CarModalAgent mmAgent = MmAgentPrefab.GetComponentInChildren<CarModalAgent>(true);
            CarAgent[] subAgents = MmAgentPrefab.GetComponentsInChildren<CarAgent>(true);

            //If a modal model
            if(spawnMode == 2 || spawnMode == 6)
            {
                startingColor = "white";
                mmAgent.transform.gameObject.SetActive(true);
                if(spawnMode == 6)
                {
                    var behavior = mmAgent.transform.GetComponentInChildren<BehaviorParameters>();
                    behavior.Model = ModalBrain;
                    behavior.BehaviorType = BehaviorType.InferenceOnly;
                }
                else
                {
                    var behavior = mmAgent.transform.GetComponentInChildren<BehaviorParameters>();
                    behavior.Model = null;
                    if(heuristic)
                        behavior.BehaviorType = BehaviorType.HeuristicOnly;
                    else
                        behavior.BehaviorType = BehaviorType.Default;
                }

                foreach(var agent in subAgents)
                {
                    agent.isSubModel = true;
                    agent.transform.gameObject.SetActive(true);
                    agent.transform.GetComponentInChildren<BehaviorParameters>().BehaviorType = BehaviorType.InferenceOnly;
                }
                subAgents[0].transform.GetComponentInChildren<BehaviorParameters>().Model = ModalSubABrain;
                subAgents[1].transform.GetComponentInChildren<BehaviorParameters>().Model = ModalSubBBrain;
                subAgents[2].transform.GetComponentInChildren<BehaviorParameters>().Model = ModalSubCBrain;
                MmAgentPrefab.GetComponentInChildren<CheckpointManager>().agent = mmAgent;
            }
            //If sub model a
            if(spawnMode == 3)
            {
                subAgents[0].transform.GetComponentInChildren<BehaviorParameters>().Model = null;
                subAgents[0].transform.GetComponentInChildren<BehaviorParameters>().BehaviorType = BehaviorType.Default;
                subAgents[0].isSubModel = false;

                subAgents[0].transform.gameObject.SetActive(true);
                subAgents[1].transform.gameObject.SetActive(false);
                subAgents[2].transform.gameObject.SetActive(false);
                mmAgent.transform.gameObject.SetActive(false);
                MmAgentPrefab.GetComponentInChildren<CheckpointManager>().agent = subAgents[0];
                MmAgentPrefab.GetComponentInChildren<CarV2>().pushEnabled = false;
                Physics.IgnoreLayerCollision(7,7);
                Physics.IgnoreLayerCollision(8,8);
                var sensors = subAgents[0].transform.GetComponentsInChildren<RayPerceptionSensorComponent3D>();
                sensors[2].RayLayerMask = LayerMask.GetMask("Nothing");
                startingColor = "blue";

            }
            //If sub model b
            if(spawnMode == 4)
            {
                subAgents[1].transform.GetComponentInChildren<BehaviorParameters>().Model = null;
                subAgents[1].transform.GetComponentInChildren<BehaviorParameters>().BehaviorType = BehaviorType.Default;
                subAgents[1].isSubModel = false;

                subAgents[0].transform.gameObject.SetActive(false);
                subAgents[1].transform.gameObject.SetActive(true);
                subAgents[2].transform.gameObject.SetActive(false);
                mmAgent.transform.gameObject.SetActive(false);
                MmAgentPrefab.GetComponentInChildren<CheckpointManager>().agent = subAgents[1];
                MmAgentPrefab.GetComponentInChildren<CarV2>().pushEnabled = false;
                Physics.IgnoreLayerCollision(7,7);
                Physics.IgnoreLayerCollision(8,8);
                var sensors = subAgents[1].transform.GetComponentsInChildren<RayPerceptionSensorComponent3D>();
                sensors[2].RayLayerMask = LayerMask.GetMask("Nothing");
                startingColor = "green";
            }
            //If sub model c
            if(spawnMode == 5)
            {
                subAgents[2].transform.GetComponentInChildren<BehaviorParameters>().Model = null;
                subAgents[2].transform.GetComponentInChildren<BehaviorParameters>().BehaviorType = BehaviorType.Default;
                subAgents[2].isSubModel = false;

                subAgents[0].transform.gameObject.SetActive(false);
                subAgents[1].transform.gameObject.SetActive(false);
                subAgents[2].transform.gameObject.SetActive(true);
                mmAgent.transform.gameObject.SetActive(false);
                MmAgentPrefab.GetComponentInChildren<CheckpointManager>().agent = subAgents[2];
                startingColor = "red";
            }
        }
    }
    void Start()
    {
        handleReset();
    }
    public void finish(GameObject obj)
    {
        finishedAgents++;
        var carController = obj.GetComponentInChildren<CarV2>();
        var motorSphere = carController.motorSphere.transform.gameObject;
        var carCollider = carController.carCollider.transform.gameObject;
        UnityEngine.Object.Destroy(obj);
        UnityEngine.Object.Destroy(motorSphere);
        UnityEngine.Object.Destroy(carCollider);
        if(finishedAgents == numAgents)
        {
            episodes++;
            handleReset();
        }
    }
    void handleReset()
    {
        resetCameras();
        var spawnManager = GetComponentInChildren<SpawnPointManager>();
        spawnManager.Reset();
        numAgents = spawnManager.getNumSpawns();
        finishedAgents = 0;
        if(spawnMode != 7)
        {
            for(int i = 0; i < numAgents; i++)
            {
                var spawnLocation = spawnManager.getSpawnPoint();
                if(spawnMode == 0||spawnMode == 1)
                {
                    UnityEngine.Object.Instantiate(RlAgentPrefab, spawnLocation, Quaternion.identity);
                }
                else if(spawnMode != 0 && spawnMode != 1 && spawnMode != 7)
                {
                    var car = UnityEngine.Object.Instantiate(MmAgentPrefab, spawnLocation, Quaternion.identity);
                    car.GetComponentInChildren<CarV2>().setAntennaColor(startingColor);
                }
            }
        }
        else
        {
            print("You forgot to build the competition mode");
        }
        
        if(!(spawnMode == 3 || spawnMode == 5))
        {
            var obstacleManager = GetComponentInChildren<ObstacleSpawnerHandler>();
            obstacleManager.resetObstacles();
        }
    }

    public void resetCameras()
    {
        var cameras = GetComponentsInChildren<Camera>();
        foreach(var cam in cameras)
        {
            cam.transform.gameObject.SetActive(false);
        }
        if(cameras.Length != 0)
            cameras[0].transform.gameObject.SetActive(true);
    }
}
