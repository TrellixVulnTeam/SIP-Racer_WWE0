using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class CarAgent : Agent
{
    private InputController inputController = null;
    private  CheckpointManager checkpointManager = null;
    private Rigidbody motorSphere = null;
    private CarV2 carController = null;
    public bool isSubModel = false;
    private float lastDistance;
    // Start is called before the first frame update
    public override void Initialize()
    {
        carController = GetComponentInParent<CarV2>();
        inputController = carController.input;
        motorSphere = carController.motorSphere;
        checkpointManager = GetComponentInParent<CheckpointManager>();
        inputController.mode = 1;
        lastDistance = 100000000000f;

        if(isSubModel)
        {
            Destroy(transform.gameObject.GetComponentInChildren<DecisionRequester>());
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 vectorToNextCheckpoint = checkpointManager.nextCheckpoint.transform.position - transform.position;
        sensor.AddObservation(vectorToNextCheckpoint/100);
        sensor.AddObservation(motorSphere.velocity.magnitude);
        sensor.AddObservation(carController.boostCooldownTimer / carController.boostCooldown);

        if(Vector3.Distance(transform.position, vectorToNextCheckpoint) < lastDistance)
        {
            if(!isSubModel)
            {
                AddReward(0.00001f);
            }
            else
            {
                checkpointManager.agent.AddReward(0.00001f);
            }
        }
        lastDistance = Vector3.Distance(transform.position, vectorToNextCheckpoint);

        //The longer the agent takes, the less its reward
        if(!isSubModel)
            AddReward(-0.0000001f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;
        switch((int)Mathf.Round(Input.GetAxis("Vertical")))
        {
            case 0:
                discreteActions[0] = 0;
            break;
            case 1:
                discreteActions[0] = 1;
            break;
            case -1:
                discreteActions[0] = 2;
            break;
        }

        switch((int)Mathf.Round(Input.GetAxis("Horizontal")))
        {
            case 0:
                discreteActions[1] = 0;
            break;
            case 1:
                discreteActions[1] = 1;
            break;
            case -1:
                discreteActions[1] = 2;
            break;
        }
        discreteActions[2] = (int)Mathf.Round(Input.GetAxis("Jump"));
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
            inputController.setPowerDecision(0);
            break;
            case 1:
            inputController.setPowerDecision(1);
            break;
            case 2:
            inputController.setPowerDecision(-1);
            break;
        }

        switch(actions.DiscreteActions[1])
        {
            case 0:
            inputController.setSteerDecision(0);
            break;
            case 1:
            inputController.setSteerDecision(1);
            break;
            case 2:
            inputController.setSteerDecision(-1);
            break;
        }

        inputController.setBoostDecision(actions.DiscreteActions[2]);

        string actionslog = "";
        for(var i = 0; i < actions.DiscreteActions.Length; i++)
        {
            actionslog = actionslog + "Action[" + i + "]: " + actions.DiscreteActions[i];
        }
        //Incentivise forward action
        if(actions.DiscreteActions[0] == 1)
        {
            if(isSubModel)
                checkpointManager.agent.AddReward(0.000007f);
            else
                AddReward(0.000007f);
        }
    }
}
