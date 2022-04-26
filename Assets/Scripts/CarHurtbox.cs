using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CarHurtbox : MonoBehaviour
{
    public string side;
    private float forceBack;
    private float forceDirectional;
    private CarV2 parent;
    
    void Awake()
    {
        parent = GetComponentInParent<CarV2>();
    }
    // Start is called before the first frame update
    private void OnTriggerStay(Collider other) {
        if(other.gameObject.tag == "Car")
        {
            if(parent.getPushTimer() > 0)
            {
                var target = other.gameObject.GetComponentInParent<CarV2>();
                Vector3 force = new Vector3();
                switch(side)
                {
                    case "Left":
                    force = (-transform.right * forceDirectional) + (-transform.forward * forceBack);
                    break;

                    case "Right":
                    force = (transform.right * forceDirectional) + (-transform.forward * forceBack);
                    break;

                    case "Back":
                    force =(-transform.forward * forceBack);
                    break;

                    default:
                    force = (-transform.forward * forceBack);
                    break;
                }

                target.onHit(force);
            }
        }
    }
    public float getForceBack()
    {
        return forceBack;
    }
    public void setForceBack(float f)
    {
        forceBack = f;
    }

    public float getForceDirectional()
    {
        return forceDirectional;
    }
    public void setForceDirectional(float f)
    {
        forceDirectional = f;
    }
}
