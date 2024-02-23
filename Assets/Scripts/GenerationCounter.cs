using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GenerationCounter : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    private AgentMovement AgentMovement;

    void Start(){
        AgentMovement = GetComponent<AgentMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        textMeshPro.text = "Generation: " + AgentMovement.generation.ToString();
    }
}
