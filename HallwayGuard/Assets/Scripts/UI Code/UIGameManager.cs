using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
public class UIGameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
{
    // If you came from the Pause Menu, time might still be 0!
    Time.timeScale = 1f; 
    Cursor.visible = true;
    Cursor.lockState = CursorLockMode.None;
}


}
