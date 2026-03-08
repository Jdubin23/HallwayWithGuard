using UnityEngine;

public class BatteryItem : MonoBehaviour, Interactable
{
        public AudioSource PickupAudio;
        public GameObject BatteryObject;
    public void Interact()
    {
        // Find the inventory and tell it we have the battery
        Object.FindFirstObjectByType<InventoryManager>().AddBattery();
        
        //Debug.Log("Battery Picked Up!");
        //make noise for this?
        if(PickupAudio != null && !PickupAudio.isPlaying)
        {
            PickupAudio.Play();
        }
                ObjectiveManager.Instance.hasFoundBattery = true;

        // Make the physical item in the world disappear
        BatteryObject.SetActive(false);
    }
}