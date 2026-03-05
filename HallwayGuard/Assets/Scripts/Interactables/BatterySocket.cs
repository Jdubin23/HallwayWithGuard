using UnityEngine;

public class BatterySocket : MonoBehaviour, Interactable
{
    [Header("Light Indicator Settings")]
    [SerializeField] private Renderer FruitLights; // The light that changes color
    [SerializeField] private Color glowColor = Color.gold; // Pick your glow color
    [SerializeField] private float glowIntensity = 6f; // How bright it is

    [Header("Battery Settings")]
    [SerializeField] private GameObject visualBattery; // The battery model already on the tree
    public bool IsPowered { get; private set; } = false;

    void Start()
    {
        // Ensure the battery on the tree is hidden at the start
        if(visualBattery != null) visualBattery.SetActive(false);
    }

    public void Interact()
    {
        InventoryManager inv = Object.FindFirstObjectByType<InventoryManager>();

        if (inv != null && inv.hasBattery && !IsPowered)
        {
            // 1. Take it from inventory
            inv.RemoveBattery();

            // 2. Make it appear on the tree
            if(visualBattery != null) visualBattery.SetActive(true);

            // 3. Power the system
            Debug.Log("Battery placed! Power restored.");
            ActivatePower();
        }
        else if (!IsPowered)
        {
            Debug.Log("You need a battery for this tree.");
        }
    }


    private void ActivatePower()
    {
            IsPowered = true;
            if (FruitLights != null)
            {
                // Create a new material instance to avoid changing the original
                Material mat = FruitLights.material;
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", glowColor * glowIntensity);
            }


    }
}