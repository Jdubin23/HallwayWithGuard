using UnityEngine;

public class BatteryItem : MonoBehaviour, Interactable
{
    public void Interact()
    {
        // Find the inventory and tell it we have the battery
        Object.FindFirstObjectByType<InventoryManager>().AddBattery();
        
        //Debug.Log("Battery Picked Up!");
        //make noise for this?
        
        // Make the physical item in the world disappear
        gameObject.SetActive(false);
    }
}