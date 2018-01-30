using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToMenu : MonoBehaviour
{
    public bool fromGame = false;
    public GameObject levelDesign;

    private void Awake()
    {
        if (Application.isEditor && (GameObject.Find("_debugMenu_") == null))
        {
            if (fromGame)
                levelDesign.SetActive(false);
            SceneManager.LoadScene("1_MainMenu");
        }
    }
}
