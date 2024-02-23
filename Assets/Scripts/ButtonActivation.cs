using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonActivation : MonoBehaviour
{
    public List<GameObject> agentsToActivate;
    public Button startButton;

    void Start()
    {
        // a method to the button's onClick event
        startButton.onClick.AddListener(ActivateAgents);
    }

    void ActivateAgents()
    {
        // Activate each GameObject in the list
        foreach (GameObject agent in agentsToActivate)
        {
            agent.SetActive(true);
        }
    }
}
