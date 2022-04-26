using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class CheckpointManager : MonoBehaviour
{
    public int currentCheckpoint;
    public List<Checkpoint> checkpoints;
    private float maxTime = 30f;
    public float timeLeft = 0f;
    public float maxAirTime = 2f;
    private float airTime;
    public Agent agent;
    public Checkpoint nextCheckpoint;
    private bool racing;
    private GameMaster gm;
    private bool grounded = true;

    void Awake()
    {
        airTime = maxAirTime;
        checkpoints = FindObjectOfType<Checkpoints>().checkpoints;
        gm = FindObjectOfType<GameMaster>();
        maxTime = gm.maxTime;
        racing = gm.racing;
        reset();  
    }

    public void reset()
    {
        airTime = maxAirTime;
        timeLeft = maxTime;
        currentCheckpoint = 0;
        getNextCheckpoint(true);
    }
    void Update()
    {
        timeLeft -= Time.deltaTime;
        if(timeLeft <= 0)
        {
            agent.AddReward(-.1f/(checkpoints.Count - currentCheckpoint));
            gm.finish(transform.root.gameObject);
        }

        if(!grounded)
        {
            airTime -= Time.deltaTime;
            if(airTime <= 0)
            {
                print("Fell to your death, didn't ya?");
                agent.AddReward(-1f);
                gm.finish(transform.root.gameObject);
            }
        }
    }

    public void checkpointReached(Checkpoint checkpoint)
    {
        if(checkpoint == nextCheckpoint)
        {
            timeLeft = maxTime;
            agent.AddReward(1f/checkpoints.Count);
            getNextCheckpoint(false);
        }
    }

    public void finishlineReached(int place)
    {
        agent.AddReward(1f);
        if(racing)
        {
            agent.AddReward(1f/place);
        }
        gm.finish(transform.root.gameObject);
    }

    public void wallPenalty()
    {
        agent.AddReward(-0.000001f);
    }
    private void getNextCheckpoint(bool initializing)
    {
        if(!initializing)
        {
            currentCheckpoint++;
            
        }
        nextCheckpoint = checkpoints[currentCheckpoint];
    }

    public void setGrounded(bool val)
    {
        grounded = val;
        if(val)
        {
            airTime = maxAirTime;
        }
    }

    public bool getGrounded()
    {
        return grounded;
    }
}
