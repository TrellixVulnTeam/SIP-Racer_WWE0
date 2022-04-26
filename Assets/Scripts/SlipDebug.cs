using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using TMPro;

public class SlipDebug : MonoBehaviour
{
    private GameMaster gm;
    private TMP_Text textbox;
    // Start is called before the first frame update
    void Start()
    {
        textbox = GetComponentInChildren<TMP_Text>();
        gm = FindObjectOfType<GameMaster>();
    }

    // Update is called once per frame
    void Update()
    {
        var episodeCount = gm.episodes;
        var stepCount = Academy.Instance.StepCount;
        //motorSphere.velocity.magnitude;
        textbox.text = "Step#: " + stepCount + "\nEpisode#: " + episodeCount;
    }
}
