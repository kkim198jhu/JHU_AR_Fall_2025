using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuManager : MonoBehaviour
{
    public void StartSingleplayer()
    {
        // Load the scene named "Single Player"
        SceneManager.LoadScene("Single Player");
    }

    public void ControlsMenu()
    {
        // Load the scene named "Controls Scene"
        SceneManager.LoadScene("Controls Scene");
    }

    public void backToStart()
    {
        // Load the scene named "Start Screen"
        SceneManager.LoadScene("Start Screen");
    }

    public void MultiplayerWIP()
    {
        Debug.Log("Multiplayer is still being worked on!");
    }
}
