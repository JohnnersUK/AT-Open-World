using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public float PlayerHealth = 10.0f;
    public float PlayerScore = 0.0f;

	// Use this for initialization
	void Start ()
    {
        // Destroy all other managers and set the instance
        GameManager[] tempManagers = FindObjectsOfType<GameManager>();
        foreach (GameManager manager in tempManagers)
        {
            if (manager != this)
            {
                Destroy(manager);
            }
        }
        Instance = this;
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.Escape))
        {
            Exit("Player pressed escape");
        }

        if (PlayerHealth <= 0)
        {
            Exit("Player Died");
        }
    }

    // Print the exit code and exit
    private void Exit(string exitCode)
    {
        Debug.Log("Game Exited: " + exitCode);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
         Application.OpenURL(webplayerQuitURL);
#else
         Application.Quit();
#endif
    }
}
