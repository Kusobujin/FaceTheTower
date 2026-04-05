using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void GoToMageSelect()
    {
        SceneManager.LoadScene("MageSelect");
    }
}