using UnityEngine;
using TMPro; // Required for TextMeshPro

public class ObjectiveManager : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI objectiveText;
    public static ObjectiveManager Instance;

    [Header("Game States")]
    public bool hasSteppedInside = false;
    public bool hasFoundBattery = false;
    public bool hasFlippedLevers = false;

    void Update()
    {
        UpdateObjectiveUI();
    }
private void Awake()
    {
        // This 'plugs in' the instance as soon as the game starts
        if (Instance == null) 
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Prevents having two managers
        }
    }
    private void UpdateObjectiveUI()
    {
        // 1. STARTING TEXT (Show this until hasSteppedInside is true)
        if (!hasSteppedInside)
        {
            objectiveText.text = "Objective: Search for the Chalice";
            objectiveText.color = Color.white;
        }
        // 2. SECOND TASK (Only shows after stepping inside)
        else if (!hasFoundBattery)
        {
            objectiveText.text = "Objective: The Tree is powered off, Find the Battery";
            objectiveText.color = Color.yellow; 
        }
        // 3. THIRD TASK (Only shows after finding battery)
        else if (!hasFlippedLevers)
        {
            objectiveText.text = "Objective: Use the Battery to Charge the Tree and Flip the Levers";
            objectiveText.color = Color.cyan; 
        }
        // 4. FINAL TASK / COMPLETE
        else
        {
            objectiveText.text = "Objective: The Door's Open, Grab the Chalice.";
            objectiveText.color = Color.green;
        }
    }
}