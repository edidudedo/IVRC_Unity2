using UnityEngine;

public class StateManager : MonoBehaviour
{
    public string stateNumber; 
    private string previousStateNumber;

    public TextManager textManager;

    void Start()
    {
        textManager = FindObjectOfType<TextManager>();

        previousStateNumber = stateNumber;

        if (textManager != null)
        {
            textManager.DisplayContentForScene(stateNumber);
        }
    }

    void Update()
    {
        if (stateNumber != previousStateNumber)
        {
            if (textManager != null)
            {
                textManager.DisplayContentForScene(stateNumber);
            }

            previousStateNumber = stateNumber;
        }
    }
}
