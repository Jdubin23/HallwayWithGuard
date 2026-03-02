using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.Events;

public class ResumeButton : MonoBehaviour
{
    public PauseMenu Pause; //reference pause menu to be able to use resume() function
    public Button Resume; //reference to the button component of the resume button

    void Start()
    {
        Resume.onClick.AddListener(OnClick); //adds the on click listener to the button, so that when the button is clicked it will call the on button click function
        Debug.Log(" button was clicked!");
    }

    public void OnClick()
    {
        Pause.Resume(); //calls the resume function in the pause menu script to unpause the game
    }
}
