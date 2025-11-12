using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    public void StartGame()
    {
        Debug.Log("Loading Game Scene");
        SceneManager.LoadScene("GameScene");
    }
}