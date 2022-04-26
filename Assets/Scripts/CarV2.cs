using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarV2 : MonoBehaviour
{
    public InputController input;
    public Rigidbody motorSphere;
    public Rigidbody carCollider;
    private TrailRenderer[] trails;

    [Header("Handling")]
    public float forwardAccel;
    public float reverseAccel;
    public float turnSpeed;
    public float turnCutoffSpeed;
    [Header("Boost")]
    public float boostStrength;
    public float boostTime;
    public float boostCooldown;
    public float boostTimer {get; private set;}
    public float boostCooldownTimer {get; private set;}

    [Header("Boost Push")]
    public float pushBack;
    public float pushDirectional;
    private CarHurtbox[] hurtboxes; 
    private Transform hitbox;
    public float pushDuration;
    private float pushTimer = 0;
    public bool pushEnabled = true;

    //Variable to boost up the force applied on the ball to avoid having to input stupid large numbers in the front end
    private float forceConstant = 100f;
    private SpawnPointManager spawnPointManager;
    private EmissionHandler antenna;

    // Start is called before the first frame update
    void Awake()
    {
        motorSphere.transform.parent = null;
        carCollider.transform.parent = null;
        trails = GetComponentsInChildren<TrailRenderer>();

        spawnPointManager = FindObjectOfType<SpawnPointManager>();
        foreach(var trail in trails)
        {
            trail.emitting = false;
        }

        hurtboxes = GetComponentsInChildren<CarHurtbox>();
        foreach(var hurtbox in hurtboxes)
        {
            hurtbox.setForceBack(pushBack);
            hurtbox.setForceDirectional(pushDirectional);
        }

        hitbox = transform.Find("Hitbox");
        antenna = GetComponentInChildren<EmissionHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        var turnToggle = (motorSphere.velocity.magnitude > turnCutoffSpeed ? 1 : 0);
        transform.position = motorSphere.transform.position;
        hitbox.transform.position = transform.position;
        if(input.getSteerDecision() != 0)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnSpeed * turnToggle * input.getSteerDecision() * Time.deltaTime, 0f));
        }
        BoostTimerUpdates();
        if(input.getBoostDecision() == 1 && boostCooldownTimer <= 0)
        {
            Boost();
        }

    }

    void FixedUpdate() {
        if(input.getPowerDecision() > 0 || boostTimer > 0)
        {
            motorSphere.AddForce(forceConstant * transform.forward * (forwardAccel + (boostTimer > 0 ? boostStrength : 0)) * (boostTimer > 0 ? 1 : input.getPowerDecision()));
        }
        else if(input.getPowerDecision() < 0 && boostTimer <= 0)
        {
            motorSphere.AddForce(forceConstant * transform.forward * reverseAccel * input.getPowerDecision());
        }
        
        carCollider.MoveRotation(transform.rotation);
    }

    private void Boost(){
        boostTimer = boostTime;
        boostCooldownTimer = boostCooldown;
        pushTimer = pushDuration;
        foreach(var trail in trails)
        {
            trail.emitting = true;
        }
    }

    private void BoostTimerUpdates()
    {
        if(boostTimer > 0)
        {
            boostTimer -= Time.deltaTime;
        }
        if(boostCooldownTimer > 0){
            boostCooldownTimer -= Time.deltaTime;
        }
        if(pushTimer > 0){
            pushTimer -= Time.deltaTime;
        }
        if(boostTimer <=0 && trails[0].emitting == true){
            foreach(var trail in trails){
                trail.emitting = false;
            }
        }
    }

    public void onHit(Vector3 push)
    {
        if(pushTimer <= 0 && pushEnabled)
        {
            motorSphere.AddForce(push * forceConstant);
        }
    }

    public float getPushTimer()
    {
        return pushTimer;
    }
    public void setAntennaColor(string color)
    {
        if(antenna != null)
        {
            antenna.setColor(color);
        }
    }
}
