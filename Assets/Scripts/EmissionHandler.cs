using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissionHandler : MonoBehaviour
{
    public Material emissiveMaterial;
    public Renderer objectRenderer;
    public float intensity;
    // Start is called before the first frame update
    void Start()
    {
        emissiveMaterial = objectRenderer.GetComponent<Renderer>().material;
    }

    public void setColor(string color)
    {
        Color temp;
        switch(color)
        {
            case "white":
                temp = Color.white * intensity;
                emissiveMaterial.SetColor("_EmissionColor", temp);
            break;
            case "red":
                temp = Color.red * intensity;
                emissiveMaterial.SetColor("_EmissionColor", temp);
            break;
            case "blue":
                temp = Color.blue * intensity;
                emissiveMaterial.SetColor("_EmissionColor", temp);
            break;
            case "green":
                temp = Color.green * intensity;
                emissiveMaterial.SetColor("_EmissionColor", temp);
            break;
        }
    }
}
