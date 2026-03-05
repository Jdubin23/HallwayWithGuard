using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public bool hasBattery = false;

    public void AddBattery() => hasBattery = true;
    public void RemoveBattery() => hasBattery = false;




}