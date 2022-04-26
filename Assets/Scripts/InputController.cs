using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public int mode = 0;

    public string UserSteerInput = "Horizontal";
    public string UserPowerInput = "Vertical";
    public string UserBoostInput = "Jump";

    private float steerDecision = 0f;
    private float powerDecision = 0f;
    private float boostDecision = 0f;

    public float getSteerDecision(){
        return steerDecision;
    }
    public void setSteerDecision(float decision)
    {
        steerDecision = decision;
    }
    public float getPowerDecision(){
        return powerDecision;
    }
    public void setPowerDecision(float decision)
    {
        powerDecision = decision;
    }
    public float getBoostDecision(){
        return boostDecision;
    }
    public void setBoostDecision(float decision)
    {
        boostDecision = decision;
    }
    // Update is called once per frame
    void Update()
    {
        if(mode == 0)
        {
            steerDecision = Input.GetAxis(UserSteerInput);
            powerDecision = Input.GetAxis(UserPowerInput);
            boostDecision = Input.GetAxis(UserBoostInput);
        }
    }
}
