using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuManager : MonoBehaviour
{
    public void StartSingleplayer()
    {
        // Load the scene named "Single Player"
        SceneManager.LoadScene("Single Player");
    }

    public void MultiplayerWIP()
    {
        Debug.Log("Multiplayer is still being worked on!");
    }
}
