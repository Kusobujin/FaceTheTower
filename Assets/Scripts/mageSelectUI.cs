using UnityEngine;
using UnityEngine.SceneManagement;

public class MageSelectUI : MonoBehaviour
{
    public void SelectFireMage()
    {
        GameData.Instance.selectedMage = "Fire";
        SceneManager.LoadScene("summonSelect");
    }

    public void SelectIceMage()
    {
        GameData.Instance.selectedMage = "Ice";
        SceneManager.LoadScene("summonSelect");
    }

    public void SelectDarkMage()
    {
        GameData.Instance.selectedMage = "Dark";
        SceneManager.LoadScene("summonSelect");
    }
}