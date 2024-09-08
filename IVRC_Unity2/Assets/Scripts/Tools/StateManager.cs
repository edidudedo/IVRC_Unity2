using UnityEngine;

public class StateManager : MonoBehaviour
{
    public string stateNumber = "0"; 
    private string previousStateNumber;
    GameObject vrWorldObject;
    public GameObject cheese;
    public GameObject wine;
    public GameObject baguette;

    public TextManager textManager;

    void Start()
    {
        textManager = FindObjectOfType<TextManager>();
        vrWorldObject = GameObject.Find("World/vrWorld");

        previousStateNumber = stateNumber;

        if (textManager != null)
        {
            textManager.DisplayContentForScene(stateNumber);
        }
    }

    void Update()
    {
        Debug.Log(vrWorldObject.layer);
        if (stateNumber != previousStateNumber)
        {
            if (textManager != null)
            {
                textManager.DisplayContentForScene(stateNumber);
            }

            previousStateNumber = stateNumber;
        }
        ChangeStateNumber();
    }

    void ChangeStateNumber()
    {
        if(stateNumber == "0")
        {
            if (Input.GetKeyDown(KeyCode.Return)) {
                stateNumber = "1";
            }
        }
        else if(stateNumber == "1")
        {
            if(vrWorldObject.layer == 0)
            {
                stateNumber = "2";
            }
        }
        else if(stateNumber == "2")
        {
            if (cheese.GetComponent<ObjectManager>().isIdle || baguette.GetComponent<ObjectManager>().isIdle || wine.GetComponent<WineManager>().isIdle)
            {
                stateNumber = "3";
            }
        }
        else if (stateNumber == "3")
        {
            //When User starts coloring, change to state 4
            if (FlagManager.startColoring)
            {
                stateNumber = "4";
            }
        }
        else if (stateNumber == "4")
        {
            //When user finishes coloring (80%), change to state 5
            if (FlagManager.uncoloredObjectNum == 0) 
            {
                stateNumber = "5";
            }
        }
        else if (stateNumber == "5")
        {
            if (vrWorldObject.layer == 6)
            {
                stateNumber = "6";
            }
        }
    }
}
