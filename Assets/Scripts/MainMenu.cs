using UnityEngine;
using UnityEngine.SceneManagement;// Need this for scene loading

public class MainMenu : MonoBehaviour
{
    public string loadNewGame;
    // For now, until we decide on the menu's UX, this script is going to be VERY simple. 
    public void NewGame()
    {
        SceneManager.LoadScene(loadNewGame);
    }

}
