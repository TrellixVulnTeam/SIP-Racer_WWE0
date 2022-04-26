using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CarModalAgent : Agent
{
    private InputController inputController = null;
    private  CheckpointManager checkpointManager = null;
    private Rigidbody motorSphere = null;
    private CarV2 carController = null;
    public CarAgent subModelA;
    public CarAgent subModelB;
    public CarAgent subModelC;
    // Start is called before the first frame update
    public override void Initialize()
    {
        carController = GetComponentInParent<CarV2>();
        inputController = carController.input;
        motorSphere = carController.motorSphere;
        checkpointManager = GetComponentInParent<CheckpointManager>();
        inputController.mode = 1;

        subModelA.isSubModel = true;
        subModelB.isSubModel = true;
        subModelC.isSubModel = true;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 vectorToNextCheckpoint = checkpointManager.nextCheckpoint.transform.position - transform.position;
        sensor.AddObservation(vectorToNextCheckpoint/20);
        sensor.AddObservation(motorSphere.velocity.magnitude);
        sensor.AddObservation(carController.boostCooldownTimer);
        //The longer the agent takes, the less its reward
        AddReward(-0.000001f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = (int)Mathf.Round(Input.GetAxis("Horizontal") + 1);
    }
    public override void OnEpisodeBegin()
    {
        checkpointManager.reset();
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        switch(actions.DiscreteActions[0])
        {
            case 0:
                carController.setAntennaColor("blue");
                subModelA.RequestDecision();
            break;

            case 1:
                carController.setAntennaColor("green");
                subModelB.RequestDecision();
            break;

            case 2:
                carController.setAntennaColor("red");
                subModelC.RequestDecision();
            break;
        }
    }
}
